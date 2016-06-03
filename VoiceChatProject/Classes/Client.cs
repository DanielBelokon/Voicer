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
using Voicer.Common.Net;

namespace Voicer
{
    public class Client : NetworkClient
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

        // Stops the user list from updating while it's already being updated on a different thread.
        private bool blockUserUpdatePacket = false;

        // A thread used to check the amount of packets recieved in the last x seconds, to make sure there is a stable connection.
        protected int packetCount;

        // A Message handler for handling the packets and invoking the needed functions to proccess them.

        // The unique 16 character key of the server.
        private string serverKey;
        // The unique 32 character key of the client
        private string clientKey;

        private bool isAdmin;
        private IPEndPoint endPoint;
        private IPAddress serverAddress;

        public Client() : base(9998, ServerPort)
        {
            // Add message handlers to all packet types.
            packetHandler.AddPacketHandler(Packet.Messages.CHAT, new Action<Packet>(HandleChatPacket));
            packetHandler.AddPacketHandler(Packet.Messages.GETUSERS, new Action<Packet>(HandleUserListPacket));
            packetHandler.AddPacketHandler(Packet.Messages.KEEPALIVE, new Action<Packet>(HandleKeepAlivePacket));
            packetHandler.AddPacketHandler(Packet.Messages.SHUTDOWN, new Action<Packet>(HandleShutdownPacket));
            packetHandler.AddPacketHandler(Packet.Messages.VOICE, new Action<Packet>(HandleSoundPacket));
            packetHandler.AddPacketHandler(Packet.Messages.CONNECTED, new Action<Packet>(HandleConnectedPacket));
            packetHandler.AddPacketHandler(Packet.Messages.GETKEY, new Action<Packet>(HandleGetKeyPacket));
            packetHandler.AddPacketHandler(Packet.Messages.SETKEY, new Action<Packet>(HandleSetKeyPacket));
            packetHandler.AddPacketHandler(Packet.Messages.JOINCHANNEL, new Action<Packet>(HandleSwitchChannelPacket));
            packetHandler.AddPacketHandler(Packet.Messages.SETADMIN, new Action<Packet>(HandleSetAdminPacket));
            packetHandler.AddPacketHandler(Packet.Messages.SERVERMESSAGE, new Action<Packet>(HandleServerMessagePacket));
        }

        public int Connect(string serverIP, string nick)
        {
            if (IsConnected)
            {
                Disconnect();
                Thread.Sleep(100);
            }

            ChangeStatus("Connecting...");
            try
            {
                serverAddress = IPAddress.Parse(serverIP);
                endPoint = new IPEndPoint(serverAddress, ServerPort);
                Console.WriteLine("Starting up sockets...");
                Connect(endPoint);
                StartListen();
                // Remove any unwanted characters that could disrupt the functions of the server.
                nickname = Data.MakeSafe(nick);

                Console.WriteLine("Initializing tick...");
                StartTick(15);

                byte[] message = Encoding.ASCII.GetBytes(nickname);

                Console.WriteLine("Initializing connection...");
                // Send message informing the server the client is connecting, including the client's nickname.
                Send(new Packet(Packet.Messages.CONNECT, message));
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
        public override void Tick()
        {
            if (packetCount == 0)
            {
                Disconnect();
            }

            packetCount = 0;
        }

        public override void PacketRecieved(Packet packet)
        {
            packetCount++;
            Send(new Packet(Packet.Messages.RECIEVED, BitConverter.GetBytes((short)packet.Type)));
        }

        public override void Disconnecting()
        {
            string serverString = serverAddress.ToString();
            serverAddress = null;
            endPoint = null;
            nickname = null;
            UserListUpdated(new List<Channel>(), true);
            blockUserUpdatePacket = false;
            ChangeStatus("Offline");

            if (IsConnected)
            {
                Send(new Packet(Packet.Messages.DISCONNECT, BitConverter.GetBytes(clientID)));
                Console.WriteLine("Disconnected from " + serverString);
            }
        }

        public void SendChatMessage(string message)
        {
            byte[] buffer = BitConverter.GetBytes(this.clientID).Concat(BitConverter.GetBytes(channelID)).Concat(Encoding.ASCII.GetBytes(message)).ToArray();
            Send(new Packet(Packet.Messages.CHAT, buffer));
        }

        public void SendVoiceMessage(byte[] data)
        {
            byte[] buffer = BitConverter.GetBytes(clientID).Concat(BitConverter.GetBytes(channelID)).Concat(data).ToArray();
            Send(new Packet(Packet.Messages.VOICE, buffer));
        }

        #region packet handeling

        public void HandleSwitchChannelPacket(Packet packet)
        {
            channelID = BitConverter.ToInt16(packet.Data, 0);

            ChatMessageRecieved("--- Now speaking on channel " + this.channelID.ToString());
        }

        public void HandleKeepAlivePacket(Packet packet)
        {
            Send(new Packet(Packet.Messages.KEEPALIVE, BitConverter.GetBytes(clientID)));
        }

        public void HandleShutdownPacket(Packet packet)
        {
            Console.WriteLine("Server Closed: " + Encoding.ASCII.GetString(packet.Data));
            Disconnect();
        }

        public void HandleConnectedPacket(Packet packet)
        {
            clientID = BitConverter.ToInt16(packet.Data, 0);
            channelID = 1;// BitConverter.ToInt16(data, 2);
            Console.WriteLine("Connected - " + endPoint.ToString());
            ChangeStatus("Connected to " + endPoint.ToString());

            Send(new Packet(Packet.Messages.CONNECTED, BitConverter.GetBytes(clientID)));
        }

        public void HandleSoundPacket(Packet packet)
        {
            short clientId = BitConverter.ToInt16(packet.Data, 0);
            short channelId = BitConverter.ToInt16(packet.Data, 2);

            byte[] soundData = packet.Data.Skip(4).ToArray();
            if (channelID != channelId)
                return;

            User user = FindClient(clientId);
            if (user.ID != -1)
            {
                user.clientAudio.AddSound(soundData);
            }
        }

        public void HandleChatPacket(Packet packet)
        {
            short clientId = BitConverter.ToInt16(packet.Data, 0);
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

            OnChatMessageRecieved(senderName, Encoding.ASCII.GetString(packet.Data.Skip(4).ToArray()));
        }

        // Update the UI with a new user list.
        public void HandleUserListPacket(Packet packet)
        {
            while (blockUserUpdatePacket)
            {
                Thread.Sleep(50);
            }
            blockUserUpdatePacket = true;
            string users = "";

            if (packet != null)
                users = Encoding.ASCII.GetString(packet.Data);

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

        public void HandleGetKeyPacket(Packet packet)
        {
            serverKey = Encoding.ASCII.GetString(packet.Data);
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
                Send(new Packet(Packet.Messages.SETKEY, BitConverter.GetBytes(clientID).Concat(Encoding.ASCII.GetBytes(clientKey)).ToArray()));
                Console.WriteLine("Sending Client Key: " + clientKey);
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

        public void HandleSetKeyPacket(Packet packet)
        {
            clientKey = Encoding.ASCII.GetString(packet.Data);
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

                Console.WriteLine("Client key recieved from server: " + clientKey);
                serverKeys.Insert(serverKeys.IndexOf(serverKey) + 1, clientKey);

                File.WriteAllLines(Environment.CurrentDirectory + "/client.txt", serverKeys);
            }
            else
            {
                Console.WriteLine("Not matching client key on server, requesting new one");
                RequetKey();
            }
        }

        public void HandleSetAdminPacket(Packet packet)
        {
            this.isAdmin = BitConverter.ToBoolean(packet.Data, 0);
            Console.WriteLine("IsAdmin = " + this.isAdmin);
        }

        public void HandleServerMessagePacket(Packet packet)
        {
            if(ServerMessageRecieved != null)
                ServerMessageRecieved(Encoding.ASCII.GetString(packet.Data.Skip(2).ToArray()));
        }

        #endregion packet handeling

        public void RequetKey()
        {
            Console.WriteLine("Requesting new key from server...");
            Send(new Packet(Packet.Messages.GETKEY, BitConverter.GetBytes(clientID)));
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
            if (StatusChanged != null)
                StatusChanged.Invoke(status);
        }

        public void SendServerMessage(string message)
        {
            if (isAdmin)
            {
                byte[] buffer = BitConverter.GetBytes(this.clientID).Concat(Encoding.ASCII.GetBytes(message)).ToArray();
                Send(new Packet(Packet.Messages.SERVERMESSAGE, buffer));
            }
        }

        public void JoinChannel(short channelID)
        {
            byte[] buffer = BitConverter.GetBytes(clientID).Concat(BitConverter.GetBytes(channelID)).ToArray();
            Send(new Packet(Packet.Messages.JOINCHANNEL, buffer));
        }
    }
}
