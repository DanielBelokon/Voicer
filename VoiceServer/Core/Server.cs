using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using Voicer.Common.Data;
using Voicer.Common.Net;
using System.IO;

namespace VoiceServer
{
    //public enum PacketType { Sound = 1, Info = 2, Chat = 3 };

    public class Server : NetworkClient
    {
        // List of connected (online) clients
        //public List<ServerClient> clientList = null;
        public List<Channel> channels = null;
        public short defaultChannel = 1;
        
        const int KeyLength = 6;
        //public Dictionary<string, ServerClient> clients = null;

        private short CLIENT_NEXTID = 0;

        private int port;
        protected Thread listenThread;
        protected UdpClient listenSocket;
        protected AutoResetEvent packetRecieved;

        // Constructor
        public Server (int port = 9999) : base()
        {
            channels = new List<Channel>();
            //clientList = new List<ServerClient>();
            try
            {
                Administration.LoadServerKeys();
                StartTick();
                string[] channelString = File.ReadAllLines(Environment.CurrentDirectory + "/serverlayout.txt");
                string defaultChan = channelString.First();
                defaultChannel = short.Parse(defaultChan);

                if (defaultChannel > channelString.Count() - 1 || defaultChannel < 1)
                {
                    Console.WriteLine("Default channel " + defaultChannel + " not found, reverting to first channel.");
                    defaultChannel = 1;
                }

                foreach (string stringChannel in channelString.Skip(1))
                {
                    this.channels.Add(new Channel(stringChannel));
                }

            }

            catch (FileNotFoundException)
            {
                Console.WriteLine("serverlayout.txt file not found -- creating default channels.");
                channels.Add(new Channel("Main"));
                channels.Add(new Channel("Channel 1"));
                channels.Add(new Channel("Channel 2"));
            }

            Console.WriteLine("Packet Handlers");
            packetHandler.AddPacketHandler(Messages.VOICE, new Action<Packet>(HandleVoicePacket));
            packetHandler.AddPacketHandler(Messages.CHAT, new Action<Packet>(HandleChatPacket));
            packetHandler.AddPacketHandler(Messages.CONNECT, new Action<Packet>(HandleConnectPacket));
            packetHandler.AddPacketHandler(Messages.CONNECTED, new Action<Packet>(HandleConnectedPacket));
            packetHandler.AddPacketHandler(Messages.KEEPALIVE, new Action<Packet>(HandleKeepAlivePacket));
            packetHandler.AddPacketHandler(Messages.DISCONNECT, new Action<Packet>(HandleDisconnectPacket));
            packetHandler.AddPacketHandler(Messages.RECIEVED, new Action<Packet>(HandleRecievedPacket));
            packetHandler.AddPacketHandler(Messages.JOINCHANNEL, new Action<Packet>(HandleJoinChannelPacket));
            packetHandler.AddPacketHandler(Messages.NEWKEY, new Action<Packet>(HandleNewKeyPacket));
            packetHandler.AddPacketHandler(Messages.SETKEY, new Action<Packet>(HandleSetKeyPacket));
            packetHandler.AddPacketHandler(Messages.SERVERMESSAGE, new Action<Packet>(HandleServerMessagePacket));

            this.port = port;
            // Start listening for incoming packets and requests
            StartListen(port);
        }

        protected override void Tick()
        {
            Administration.SaveServerKeys();
        }

        #region Packet Handling

        protected void HandleJoinChannelPacket(Packet packet)
        {
            short senderId = BitConverter.ToInt16(packet.Data, 0);
            short channelId = BitConverter.ToInt16(packet.Data, 2);

            Channel channel = FindChannel(channelId);
            
            if (channel == null)
                return;

            ServerClient client = FindClient(senderId);

            if(client == null)
                return;

            client.SwitchChannel(channel);
            SignedPacket sendingPacket = new SignedPacket(Messages.SWAPCHANNEL, client.Id, BitConverter.GetBytes(channel.channelId).ToArray());
            SendToClients(0, sendingPacket, senderId);
        }

        // Proccess and forword voice packets
        protected void HandleVoicePacket(Packet packet)
        {
            short senderId = BitConverter.ToInt16(packet.Data, 0);
            short channelId = BitConverter.ToInt16(packet.Data, 2);
            SendToClients(channelId, packet, senderId);
        }

        protected void HandleChatPacket(Packet packet)
        {
            short senderId = BitConverter.ToInt16(packet.Data, 0);
            short channelId = BitConverter.ToInt16(packet.Data, 2);
            ServerClient client = FindClient(senderId);
            if (client != null)
            {
                SendToClients(channelId, packet);
                Console.WriteLine(client.name + ": " + Encoding.ASCII.GetString(packet.Data.Skip(4).ToArray()));
            }
        }

        protected void HandleConnectPacket(Packet packet)
        {
            string name = Data.MakeSafe(Encoding.ASCII.GetString(packet.Data));

            ServerClient newClient = new ServerClient(packet.Sender.Address, name, ++CLIENT_NEXTID);
            newClient.ClientDisconnected += ClientDisconnect;
            newClient.ClientRequestPacket += ClientRequestPacket;
            newClient.SwitchChannel(FindChannel(defaultChannel));
            newClient.Send(new Packet(Messages.GETUSERS, SerializeUsers()));
            SignedPacket newPacket = new SignedPacket(Messages.CONNECTCHANNEL, newClient.Id, BitConverter.GetBytes(defaultChannel).Concat(Encoding.ASCII.GetBytes(newClient.name)).ToArray());
            SendToClients(0, newPacket, newClient.Id);

            Console.WriteLine(newClient.name + " Has Connected.");
        }

        protected void HandleConnectedPacket(Packet packet)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(packet.Data, 0));
            if (user != null)
            {
                user.RequestKey();
            }
        }

        protected void HandleDisconnectPacket(Packet packet)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(packet.Data, 0));
            if (user != null)
            {
                user.Disconnect();
            }
        }

        protected void HandleKeepAlivePacket(Packet packet)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(packet.Data, 0));
            if (user != null)
            {
                user.UpdatesMissed = 0;
            }
        }

        protected void HandleRecievedPacket(Packet packet)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(packet.Data, 0));

            if (user != null)
            {
                short messageId = BitConverter.ToInt16(packet.Data, 2);
                user.PacketsAwaitingConfirmation.Remove(messageId);
                
                if (messageId == (short)Messages.KEEPALIVE)
                    user.UpdatesMissed = 0;
            }
            else Console.WriteLine("NULL USER");
        }

        protected void HandleNewKeyPacket(Packet packet)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(packet.Data, 0));

            if (user != null)
            {
                user.NewKey();
            }
        }

        protected void HandleSetKeyPacket(Packet packet)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(packet.Data, 0));
            if (user != null)
            {
                user.SetKey(Encoding.ASCII.GetString(packet.Data.Skip(2).ToArray()));
            }
        }

        protected void HandleServerMessagePacket(Packet packet)
        {
            ServerClient user = FindClient(BitConverter.ToInt16(packet.Data, 0));
            if (user != null)
            {
                string message = Encoding.ASCII.GetString(packet.Data.Skip(2).ToArray());
                if (user.IsAdmin)
                {
                    Console.WriteLine(user.name + ": " + message);
                    SendToClients(0, packet);
                }
                else
                    user.Send(new Packet(Messages.SERVERMESSAGE, BitConverter.GetBytes(0).Concat(Encoding.ASCII.GetBytes("Insuffciant Permissions.")).ToArray()));
            }
        }

        #endregion Packet Handling

        // Stop the server
        public void Stop (string message = "Shutting Down")
        {
            Console.WriteLine("Server Shutting Down");
            Administration.SaveServerKeys();
            
            SendToClients(0, new Packet(Messages.SHUTDOWN, Encoding.ASCII.GetBytes(message)));
            Disconnect();
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
            client.Disconnect();
        }

        // Recieve any and all packets that come into the listening port, seperate them by type, and forward to appropriate functions

        public void ClientDisconnect (ServerClient client, ServerClient.ClosingReason reason)
        {
            if (reason == ServerClient.ClosingReason.TimedOut)
                Console.WriteLine(client.name + " Timed out.");
            else Console.WriteLine(client.name + " Disconnected.");

            Channel channel = FindChannel(client);
            if (channel != null)
            {
                channel.clients.Remove(client);
                SendToClients(0, new SignedPacket(Messages.DISCONNECT, client.Id));
            }
        }

        public void ClientRequestPacket(ServerClient client, short packetId)
        {
            if (packetId == (short)Messages.GETUSERS)
                client.Send(new Packet(Messages.GETUSERS, SerializeUsers()));
            else if (packetId == (short)Messages.CONNECTED)
            {
                Console.WriteLine("Ressending CONNECT packet! ----------------------------");
                client.Send(new SignedPacket(Messages.CONNECTED, client.Id));
            }
        }

        // Returns client with specified ID from connected client list.
        public ServerClient FindClient(short id)
        {
            foreach (Channel channel in channels)
            {
                foreach (ServerClient client in channel.clients)
                {
                    if (client.Id == id)
                    {
                        return client;
                    }
                }
            }

            return null;
        }

        private Channel FindChannel(ServerClient client)
        {
            foreach (Channel channel in channels)
            {
                if (channel.clients.Contains(client))
                    return channel;
            }
            return null;
        }

        private Channel FindChannel(short channelId)
        {
            foreach (Channel channel in channels)
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

            foreach (Channel channel in channels)
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
        public void SendToClients(short channelId, IPacket packet, short filterID = -1, bool updateConfirmationRequest = false)
        {
            if (channelId == 0)
            {
                foreach (Channel channel in channels)
                {
                    channel.Send(packet, filterID);
                }
                return;
            }
            else
            {
                foreach (Channel channel in channels)
                {
                    if (channel.channelId == channelId)
                    {
                        channel.Send(packet, filterID);
                        return;
                    }
                }
            }
        }

        public void SendChat(short channelId, short clientId, string message)
        {
            SignedPacket packet = new SignedPacket(Messages.CHAT, clientId, Encoding.ASCII.GetBytes(message).ToArray());
            SendToClients(channelId, packet);
        }

        public byte[] SerializeUsers()
        {
            string serialized = "";

            foreach (Channel channel in channels)
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
