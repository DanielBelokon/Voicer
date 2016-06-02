using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Voicer.Common;
using System.IO;

namespace VoiceServer
{
    //public enum PacketType { Sound = 1, Info = 2, Chat = 3 };

    public class Server
    {
        // List of connected (online) clients
        //public List<ServerClient> clientList = null;
        public List<Channel> channelList = null;
        public short defaultChannel = 1;
        
        const int KeyLength = 6;
        //public Dictionary<string, ServerClient> clients = null;

        private short lastID = 0;

        private int port;
        protected bool online;
        protected Thread listenThread;
        protected UdpClient listenSocket;
        protected AutoResetEvent packetRecieved;

        private Thread tickThread;

        MessageHandler packetHandler;

        public bool IsOnline
        {
            get
            {
                return this.online;
            }
        }

        // Constructor
        public Server (int port = 9999)
        {
            channelList = new List<Channel>();
            //clientList = new List<ServerClient>();
            try
            {
                Administration.LoadServerKeys();
                string[] channels = File.ReadAllLines(Environment.CurrentDirectory + "serverlayout.txt");
                string defaultChan = channels.First();
                defaultChannel = short.Parse(defaultChan);

                if (defaultChannel > channels.Count() - 1 || defaultChannel < 1)
                {
                    Console.WriteLine("Default channel " + defaultChannel + " not found, reverting to first channel.");
                    this.defaultChannel = 1;
                }

                foreach (string stringChannel in channels.Skip(1))
                {
                    channelList.Add(new Channel(stringChannel));
                }

            }

            catch (FileNotFoundException)
            {
                Console.WriteLine("serverlayout.txt file not found -- creating default channels.");
                channelList.Add(new Channel("Main"));
                channelList.Add(new Channel("Channel 1"));
                channelList.Add(new Channel("Channel 2"));
            }

            packetHandler = new MessageHandler();

            packetHandler.AddMessageHandler(MessageHandler.Messages.VOICE, new Action<byte[], IPEndPoint>(HandleVoicePacket));
            packetHandler.AddMessageHandler(MessageHandler.Messages.CHAT, new Action<byte[], IPEndPoint>(HandleChatPacket));
            packetHandler.AddMessageHandler(MessageHandler.Messages.CONNECT, new Action<byte[], IPEndPoint>(HandleConnectPacket));
            packetHandler.AddMessageHandler(MessageHandler.Messages.CONNECTED, new Action<byte[], IPEndPoint>(HandleConnectedPacket));
            packetHandler.AddMessageHandler(MessageHandler.Messages.KEEPALIVE, new Action<byte[], IPEndPoint>(HandleKeepAlivePacket));
            packetHandler.AddMessageHandler(MessageHandler.Messages.DISCONNECT, new Action<byte[], IPEndPoint>(HandleDisconnectPacket));
            packetHandler.AddMessageHandler(MessageHandler.Messages.RECIEVED, new Action<byte[], IPEndPoint>(HandleRecievedPacket));
            packetHandler.AddMessageHandler(MessageHandler.Messages.CHANNEL, new Action<byte[], IPEndPoint>(HandleChannelPacket));
            packetHandler.AddMessageHandler(MessageHandler.Messages.JOINCHANNEL, new Action<byte[], IPEndPoint>(HandleJoinChannelPacket));
            packetHandler.AddMessageHandler(MessageHandler.Messages.NEWKEY, new Action<byte[], IPEndPoint>(HandleNewKeyPacket));
            packetHandler.AddMessageHandler(MessageHandler.Messages.SETKEY, new Action<byte[], IPEndPoint>(HandleSetKeyPacket));
            packetHandler.AddMessageHandler(MessageHandler.Messages.SERVERMESSAGE, new Action<byte[], IPEndPoint>(HandleServerMessagePacket));

            online = true;
            this.port = port;
            // Start listening for incoming packets and requests
            listenThread = new Thread(new ThreadStart(StartListen));
            packetRecieved = new AutoResetEvent(true);
            listenThread.Start();

            tickThread = new Thread(new ThreadStart(Tick));
            tickThread.Start();
        }

        private void Tick()
        {
            while (online)
            {
                Thread.Sleep(1000);

                Administration.SaveServerKeys();
            }
        }

        #region Packet Handling

        protected void HandleJoinChannelPacket(byte[] data, IPEndPoint endP)
        {
            short senderId = BitConverter.ToInt16(data, 0);
            short channelId = BitConverter.ToInt16(data, 2);

            Channel channel = FindChannel(channelId);
            
            if (channel == null)
                return;

            ServerClient client = FindClient(senderId);

            if(client == null)
                return;

            client.SwitchChannel(channel);

            UpdateUsers();
        }

        // Proccess and forword voice packets
        protected void HandleVoicePacket(byte[] data, IPEndPoint endP)
        {
            short senderId = BitConverter.ToInt16(data, 0);
            short channelId = BitConverter.ToInt16(data, 2);
            SendToClients(channelId, MessageHandler.Messages.VOICE, data, senderId);
        }

        protected void HandleChatPacket(byte[] data, IPEndPoint endP)
        {
            short senderId = BitConverter.ToInt16(data, 0);
            short channelId = BitConverter.ToInt16(data, 2);
            ServerClient client = FindClient(senderId);
            if (client != null)
            {
                SendToClients(channelId, MessageHandler.Messages.CHAT, data);
                Console.WriteLine(client.name + ": " + Encoding.ASCII.GetString(data.Skip(4).ToArray()));
            }
        }

        protected void HandleChannelPacket(byte[] data, IPEndPoint endP)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(data, 0));

        }

        protected void HandleConnectPacket(byte[] data, IPEndPoint endp)
        {
            string name = Data.MakeSafe(Encoding.ASCII.GetString(data));

            ServerClient newClient = new ServerClient(endp.Address, name, (short)(lastID = ++lastID));
            newClient.ClientDisconnected += ClientDisconnect;
            newClient.ClientRequestPacket += ClientRequestPacket;
            newClient.SwitchChannel(FindChannel(defaultChannel));
            UpdateUsers();
            Console.WriteLine(newClient.name + " Has Connected.");
        }

        protected void HandleConnectedPacket(byte[] data, IPEndPoint endP)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(data, 0));
            if (user != null)
            {
                user.RequestKey();
            }
        }

        protected void HandleDisconnectPacket(byte[] data, IPEndPoint endP)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(data, 0));
            if (user != null)
            {
                user.Disconnect(ServerClient.ClosingReason.ClientDisconnect);
            }
        }

        protected void HandleKeepAlivePacket(byte[] data, IPEndPoint endP)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(data, 0));
            if (user != null)
            {
                user.UpdatesMissed = 0;
            }
        }

        protected void HandleRecievedPacket(byte[] data, IPEndPoint endP)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(data, 0));

            if (user != null)
            {
                short messageId = BitConverter.ToInt16(data, 2);
                user.PacketsAwaitingConfirmation.Remove(messageId);
            }
        }

        protected void HandleNewKeyPacket(byte[] data, IPEndPoint endP)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(data, 0));

            if (user != null)
            {
                user.NewKey();
            }
        }

        protected void HandleSetKeyPacket(byte[] data, IPEndPoint endP)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(data, 0));
            if (user != null)
            {
                user.SetKey(Encoding.ASCII.GetString(data.Skip(2).ToArray()));
            }
        }

        protected void HandleServerMessagePacket(byte[] data, IPEndPoint endP)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(data, 0));
            if (user != null)
            {
                string message = Encoding.ASCII.GetString(data.Skip(2).ToArray());
                if (user.IsAdmin)
                {
                    Console.WriteLine(user.name + ": " + message);
                    SendToClients(0, MessageHandler.Messages.SERVERMESSAGE, data);
                }
                else
                    user.Send(MessageHandler.Messages.SERVERMESSAGE, BitConverter.GetBytes(0).Concat(Encoding.ASCII.GetBytes("Insuffciant Permissions.")).ToArray());
            }
        }

        #endregion Packet Handling

        // Stop the server
        public void Stop (string message = "Shutting Down")
        {
            Console.WriteLine("Server Shutting Down");
            Administration.SaveServerKeys();
            SendToClients(0, MessageHandler.Messages.SHUTDOWN, Encoding.ASCII.GetBytes(message));
            this.online = false;
            listenSocket.Close();
            listenThread.Abort();
            tickThread.Abort();
        }
        

        // Kick a user from the server (using part of their name, ID, or ServerClient object.)
        public bool Kick(short id)
        {
            ServerClient user = FindClient(id);
            if (user != null)
            {
                Kick(user);
                return true;
            }

            Console.WriteLine("No user with ID: " + id);
            return false;
        }
        public bool Kick(string name)
        {
            ServerClient user = FindClient(name);
            if (user != null)
            {
                Kick(user);
                return true;
            }

            Console.WriteLine("User \"" + name + "\" not found.");
            return false;
        }
        public void Kick(ServerClient client)
        {
            client.Disconnect(ServerClient.ClosingReason.Kicked);
        }

        // Recieve any and all packets that come into the listening port, seperate them by type, and forward to appropriate functions
        public void StartListen()
        {
            // Provides the local endpoint (port) for the UDP client to listen on.
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);

            // Listening socket
            listenSocket = new UdpClient(localEndPoint);

            try
            {
                Console.WriteLine("Server Online.");
                while (online)
                {
                    packetRecieved.WaitOne();
                    listenSocket.BeginReceive(new AsyncCallback(OnReceived), null);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("An error occured when trying to start listening socket.");
            }
        }

        public void OnReceived(IAsyncResult ar)
        {
            IPEndPoint remoteEP = null;

            try
            {
                byte[] data;

                if (online)
                {
                    data = listenSocket.EndReceive(ar, ref remoteEP);
                    packetRecieved.Set();
                }
                else return;
                // Process buffer

                byte[] cleanMessage = data.Skip(2).ToArray<byte>();
                packetHandler.HandleMessage((MessageHandler.Messages)BitConverter.ToInt16(data, 0), new object[] { cleanMessage, remoteEP });
            }

            catch (SocketException)
            {
            }
        }

        public void ClientDisconnect (ServerClient client, ServerClient.ClosingReason reason)
        {
            if (reason == ServerClient.ClosingReason.TimedOut)
                Console.WriteLine(client.name + " Timed out.");
            else Console.WriteLine(client.name + " Disconnected.");

            Channel channel = FindChannel(client);
            if (channel != null)
            {
                channel.clients.Remove(client);
                UpdateUsers();
            }
        }

        public void ClientRequestPacket(ServerClient client, short packetId)
        {
            if (packetId == (short)MessageHandler.Messages.GETUSERS)
                client.Send(MessageHandler.Messages.GETUSERS, SerializeUsers());
        }

        // Returns client with specified ID from connected client list.
        public ServerClient FindClient(short id)
        {
            foreach (Channel channel in channelList)
            {
                foreach (ServerClient client in channel.clients)
                {
                    if (client.ID == id)
                    {
                        return client;
                    }
                }
            }

            return null;
        }

        private Channel FindChannel(ServerClient client)
        {
            foreach (Channel channel in channelList)
            {
                if (channel.clients.Contains(client))
                    return channel;
            }
            return null;
        }

        private Channel FindChannel(short channelId)
        {
            foreach (Channel channel in channelList)
            {
                if (channel.channelId == channelId)
                    return channel;
            }

            return null;
        }

        // Returns client with the specified name (or part of the name)
        public ServerClient FindClient(string name)
        {
            string clientName;
            name = name.ToLower();
            name = name.Trim();

            foreach (Channel channel in channelList)
            {
                foreach (ServerClient client in channel.clients)
                {
                    clientName = client.name.ToLower();
                    if (clientName.Contains(name))
                    {
                        return client;
                    }
                }
            }

            return null;
        }

        // Send a packet to all connected clients
        public void SendToClients(short channelId, MessageHandler.Messages message, byte[] data = null, short filterID = -1, bool updateConfirmationRequest = false)
        {
            if (channelId == 0)
            {
                foreach (Channel channel in channelList)
                {
                    channel.Send(message, data, filterID);
                }
                return;
            }
            else
            {
                foreach (Channel channel in channelList)
                {
                    if (channel.channelId == channelId)
                    {
                        channel.Send(message, data, filterID);
                        return;
                    }
                }
            }
        }

        public void SendChat(short channelId, short clientId, string message)
        {
            byte[] data = BitConverter.GetBytes(clientId).Concat(Encoding.ASCII.GetBytes(message)).ToArray();

            SendToClients(channelId, MessageHandler.Messages.CHAT, data);
        }

        // Send an update packet with the current connected users to the connected users
        protected void UpdateUsers()
        {
            SendToClients(0, MessageHandler.Messages.GETUSERS, SerializeUsers()); 
        }

        public byte[] SerializeUsers()
        {
            string serialized = "";

            foreach (Channel channel in channelList)
            {
                serialized += "|" + channel.ToString() + ",";
                foreach (ServerClient client in channel.clients)
                {
                    serialized += client.ToString() + ",";
                }
            }
            return Encoding.ASCII.GetBytes(serialized);
        }
    }
}
