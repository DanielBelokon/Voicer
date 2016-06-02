using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Voicer.Common.Data;

namespace Voicer
{
    public class Client
    {

        public delegate void UserListUpdateDelegate(List<Channel> channels, bool ClearList);
        public delegate void StringMessageDelegate(string message);

            //Event Handlers
        public event UserListUpdateDelegate UserListUpdated;
        public event StringMessageDelegate ChatMessageRecieved;
        public event StringMessageDelegate StatusChanged;
        public event StringMessageDelegate ServerMessageRecieved;

        // List of channels on the server
        protected List<Channel> channelList;
        // The client ID used by the server and client to distinguish this user from others.
        protected short clientID;
        // The channel the user is currently recieving and sending messages on.
        public short channelID;

        // Client-related Members (Local client)
        // The nickname used by the client that is shown to other users
        protected string nickname;

        const int ServerPort = 9999;

        // Server-related Members
        protected IPAddress serverAddress;
        protected IPEndPoint endPoint;
        protected Thread listenThread;
        protected AutoResetEvent packetRecieved;
        // Stops the user list from updating while it's already being updated on a different thread.
        private bool blockUserUpdatePacket = false;

        // A thread used to check the amount of packets recieved in the last x seconds, to make sure there is a stable connection.
        protected Thread connectionTimer;
        protected int packetCount;

        // Socket for sending
        protected Socket socket;
        // Listening UDP client
        protected UdpClient listenSocket;

        protected bool isConnected = false;
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }

            private set
            {
                isConnected = value;
            }
        }

        // A Message handler for handling the packets and invoking the needed functions to proccess them.
        MessageHandler dataHandler;

        // The unique 16 character key of the server.
        private string serverKey;
        // The unique 32 character key of the client
        private string clientKey;

        private bool isAdmin;

        
        public Client()
        {
            dataHandler = new MessageHandler();

            // Add message handlers to all packet types.
            dataHandler.AddMessageHandler(MessageHandler.Messages.CHAT, new Action<byte[]>(HandleChatPacket));
            dataHandler.AddMessageHandler(MessageHandler.Messages.GETUSERS, new Action<byte[]>(HandleUserListPacket));
            dataHandler.AddMessageHandler(MessageHandler.Messages.KEEPALIVE, new Action<byte[]>(HandleKeepAlivePacket));
            dataHandler.AddMessageHandler(MessageHandler.Messages.SHUTDOWN, new Action<byte[]>(HandleShutdownPacket));
            dataHandler.AddMessageHandler(MessageHandler.Messages.VOICE, new Action<byte[]>(HandleSoundPacket));
            dataHandler.AddMessageHandler(MessageHandler.Messages.CONNECTED, new Action<byte[]>(HandleConnectedPacket));
            dataHandler.AddMessageHandler(MessageHandler.Messages.GETKEY, new Action<byte[]>(HandleGetKeyPacket));
            dataHandler.AddMessageHandler(MessageHandler.Messages.SETKEY, new Action<byte[]>(HandleSetKeyPacket));
            dataHandler.AddMessageHandler(MessageHandler.Messages.JOINCHANNEL, new Action<byte[]>(HandleSwitchChannelPacket));
            dataHandler.AddMessageHandler(MessageHandler.Messages.SETADMIN, new Action<byte[]>(HandleSetAdminPacket));
            dataHandler.AddMessageHandler(MessageHandler.Messages.SERVERMESSAGE, new Action<byte[]>(HandleServerMessagePacket));
        }

        public int Connect(string serverIP, string nick)
        {
            if (isConnected)
            {
                Disconnect();
                Thread.Sleep(100);
            }

            ChangeStatus("Connecting...");
            try
            {
                serverAddress = IPAddress.Parse(serverIP);
                endPoint = new IPEndPoint(serverAddress, ServerPort);
                // Remove any unwanted characters that could disrupt the functions of the server.
                nickname = Data.MakeSafe(nick);

                // Start counting the amount of packets recieved every few seconds to determin if there is a stable connection.
                connectionTimer = new Thread(new ThreadStart(CheckTimeout));
                connectionTimer.Start();

                byte[] message = Encoding.ASCII.GetBytes(nickname);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                socket.Connect(endPoint);


                isConnected = true;

                // Start Listening
                listenThread = new Thread(new ThreadStart(StartListen));
                listenThread.IsBackground = true;
                packetRecieved = new AutoResetEvent(true);
                Console.WriteLine("Starting listen thread...");
                listenThread.Start();

                Console.WriteLine("Initializing connection...");
                // Send message informing the server the client is connecting, including the client's nickname.
                SendMessage(MessageHandler.Messages.CONNECT, message);
                return 0;
            }

            catch (FormatException)
            {
                return 1;
            }

            catch (SocketException)
            {
                return 2;
            }
        }

        // Check if client still recieving packets
        private void CheckTimeout()
        {
            do
            {
                Thread.Sleep(5000);
                if (packetCount == 0)
                {
                    Disconnect();
                }

                packetCount = 0;

            } while (isConnected);
        }

        public void Disconnect()
        {
            string serverString = serverAddress.ToString();
            serverAddress = null;
            endPoint = null;
            nickname = null;
            HandleUserListPacket(null);
            blockUserUpdatePacket = false;
            ChangeStatus("Offline");

            if (isConnected)
            {
                SendMessage(MessageHandler.Messages.DISCONNECT, BitConverter.GetBytes(this.clientID));
                listenSocket.Close();

                Console.WriteLine("Disconnected from " + serverString);
                isConnected = false;
                listenThread.Abort();
                connectionTimer.Abort();
            }
        }

        public void SendMessage(MessageHandler.Messages message, byte[] data = null)
        {
            if (!isConnected)
                return;

            byte[] buffer = BitConverter.GetBytes((short)message);

            if (data != null)
                buffer = buffer.Concat(data).ToArray<byte>();

            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.SetBuffer(buffer, 0, buffer.Length);

            socket.SendAsync(e);
        }

        public void SendChatMessage(string message)
        {
            byte[] buffer = BitConverter.GetBytes(this.clientID).Concat(BitConverter.GetBytes(channelID)).Concat(Encoding.ASCII.GetBytes(message)).ToArray();
            SendMessage(MessageHandler.Messages.CHAT, buffer);
        }

        public void SendVoiceMessage(byte[] data)
        {
            byte[] buffer = BitConverter.GetBytes(clientID).Concat(BitConverter.GetBytes(channelID)).Concat(data).ToArray();
            SendMessage(MessageHandler.Messages.VOICE ,buffer);
        }

        // Recieve data from server
        public void StartListen()
        {
            listenSocket = new UdpClient(new IPEndPoint(IPAddress.Any, 9998));
            try
            {
                while (isConnected)
                {
                    packetRecieved.WaitOne();
                    listenSocket.BeginReceive(new AsyncCallback(OnReceived), null);
                }
            }
            catch (SocketException)
            {
                Disconnect();
            }
        }

        public void OnReceived(IAsyncResult ar)
        {
            IPEndPoint remoteEP = null;

            try
            {
                byte[] data;

                if (isConnected)
                {
                    data = listenSocket.EndReceive(ar, ref remoteEP);
                    packetCount++;
                    packetRecieved.Set();
                }
                else return;
                // Process buffer

                byte[] cleanMessage = data.Skip(2).ToArray();

                dataHandler.HandleMessage((MessageHandler.Messages)BitConverter.ToInt16(data, 0), new object[] { cleanMessage });
            }

            catch (SocketException)
            {
                Disconnect();
            }

            catch (ObjectDisposedException)
            {
            }
        }

        #region packet handeling

        public void HandleSwitchChannelPacket(byte[] data)
        {
            channelID = BitConverter.ToInt16(data, 0);

            ChatMessageRecieved("--- Now speaking on channel " + this.channelID.ToString());
        }

        public void HandleKeepAlivePacket(byte[] data)
        {
            SendMessage(MessageHandler.Messages.KEEPALIVE, BitConverter.GetBytes(this.clientID));
        }

        public void HandleShutdownPacket(byte[] data)
        {
            Console.WriteLine("Server Closed: " + Encoding.ASCII.GetString(data));
            Disconnect();
        }

        public void HandleConnectedPacket(byte[] data)
        {
            clientID = BitConverter.ToInt16(data, 0);
            channelID = 1;// BitConverter.ToInt16(data, 2);
            Console.WriteLine("Connected - " + endPoint.ToString());
            ChangeStatus("Connected to " + endPoint.ToString());

            SendMessage(MessageHandler.Messages.CONNECTED, BitConverter.GetBytes(this.clientID));
        }

        public void HandleSoundPacket(byte[] data)
        {
            short clientId = BitConverter.ToInt16(data, 0);
            short channelId = BitConverter.ToInt16(data, 2);

            byte[] soundData = data.Skip(4).ToArray();
            if (this.channelID != channelId)
                return;

            User user = FindClient(clientId);
            if (user.ID != -1)
            {
                user.clientAudio.AddSound(soundData);
            }
        }

        public void HandleChatPacket(byte[] data)
        {
            short clientId = BitConverter.ToInt16(data, 0);
            string senderName = "";

            if (clientId == 0)
            {
                senderName = "Console";
            }

            else
            {
                User user = FindClient(clientId);
                if (user.ID != -1)
                {
                    senderName = user.nickname;
                }
            }

            OnChatMessageRecieved(senderName, Encoding.ASCII.GetString(data.Skip(4).ToArray()));
        }

        // Update the UI with a new user list.
        public void HandleUserListPacket(byte[] data)
        {
            while (blockUserUpdatePacket)
            {
                Thread.Sleep(50);
            }
            blockUserUpdatePacket = true;
            string users = "";

            if (data != null)
                users = Encoding.ASCII.GetString(data);

            if (UserListUpdated != null)
            {
                if (users == "")
                {
                    UserListUpdated(new List<Channel>(), true);
                    return;
                }

                char[] channelSplitters = { '|' };
                char[] clientSplitters = { ',' };

                string[] channelArray = users.Split(channelSplitters, StringSplitOptions.RemoveEmptyEntries);
                channelList = new List<Channel>();
                foreach (string channelString in channelArray)
                {
                    string[] userArray = channelString.Split(clientSplitters, StringSplitOptions.RemoveEmptyEntries);
                    Channel channel = new Channel(userArray[0], short.Parse(userArray[1]));
                    if (userArray.Count() > 2)
                    {
                        for (int i = 2; i <= (userArray.Length +2) / 2; i += 2)
                        {
                            // Create a new user, set ID and nickname.
                            channel.users.Add(new User(Data.DeSerialize(userArray[i]), short.Parse(userArray[i + 1])));
                        }
                    }

                    channelList.Add(channel);
                }
                blockUserUpdatePacket = false;
                UserListUpdated(channelList, false);
            }
        }

        public void HandleGetKeyPacket(byte[] data)
        {
            serverKey = Encoding.ASCII.GetString(data);
            try
            {
                List<string> serverKeys = File.ReadAllLines(Environment.CurrentDirectory + "/client.txt").ToList();
                int serverArrayIndex = serverKeys.IndexOf(serverKey);
                if (serverArrayIndex < 0)
                {
                    RequetKey();
                    return;
                }

                clientKey = serverKeys[serverKeys.IndexOf(serverKey) + 1];
                this.SendMessage(MessageHandler.Messages.SETKEY, BitConverter.GetBytes(this.clientID).Concat(Encoding.ASCII.GetBytes(clientKey)).ToArray());
                Console.WriteLine("Using Client Key: " + clientKey);
                
            }
            catch (ArgumentOutOfRangeException)
            {
                RequetKey();
            }
            catch (FileNotFoundException)
            {
                RequetKey();
            }
        }

        public void HandleSetKeyPacket(byte[] data)
        {
            clientKey = Encoding.ASCII.GetString(data);
            if (clientKey != "")
            {
                List<string> serverKeys;
                try
                {
                    serverKeys = File.ReadAllLines(Environment.CurrentDirectory + "/client.txt").ToList();
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("client.txt not found -- creating new server key list.");
                    serverKeys = new List<string>();
                }

                int serverArrayIndex = serverKeys.IndexOf(serverKey);
                    if (serverArrayIndex < 0)
                        serverKeys.Add(serverKey);

                    serverKeys.Insert(serverKeys.IndexOf(serverKey) + 1, clientKey);

                File.WriteAllLines(Environment.CurrentDirectory + "/client.txt", serverKeys);
            }
            else
            {
                Console.WriteLine("Not matching client key on server, requesting new one");
                RequetKey();
            }
        }

        public void HandleSetAdminPacket(byte[] data)
        {
            this.isAdmin = BitConverter.ToBoolean(data, 0);
            Console.WriteLine("IsAdmin = " + this.isAdmin);
        }

        public void HandleServerMessagePacket(byte[] data)
        {
            if(ServerMessageRecieved != null)
                ServerMessageRecieved(Encoding.ASCII.GetString(data.Skip(2).ToArray()));
        }

        #endregion packet handeling

        public void RequetKey()
        {
            Console.WriteLine("Requesting new key from server...");
            SendMessage(MessageHandler.Messages.NEWKEY, BitConverter.GetBytes(this.clientID));
        }

        public User FindClient(short id)
        {
            foreach (Channel channel in channelList)
            {
                foreach (User client in channel.users)
                {
                    if (client.ID == id)
                    {
                        return client;
                    }
                }
            }
            return User.Empty;
        }

        // Called when a chat message is recieved
        public void OnChatMessageRecieved(string name, string message)
        {
            if (ChatMessageRecieved != null)
            {
                Console.WriteLine(name + ": " + message);
                ChatMessageRecieved(name + ": " + message);
            }
        }

        public void ChangeStatus(string status)
        {
            StatusChanged?.Invoke(status);
        }

        public void SendServerMessage(string message)
        {
            if (isAdmin)
            {
                byte[] buffer = BitConverter.GetBytes(this.clientID).Concat(Encoding.ASCII.GetBytes(message)).ToArray();
                SendMessage(MessageHandler.Messages.SERVERMESSAGE, buffer);
            }
        }

        public void JoinChannel(short channelID)
        {
            byte[] buffer = BitConverter.GetBytes(this.clientID).Concat(BitConverter.GetBytes(channelID)).ToArray();
            SendMessage(MessageHandler.Messages.JOINCHANNEL, buffer);
        }
    }
}
