using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Voicer.Common;

namespace VoiceServer
{
    public class ServerClient
    {
        // Max KeepAlive packets a user can miss before he times out & forced to disconnect.
        const int MAX_UPDATES_MISSED = 5;

        const int ClientPort = 9998;

        protected Thread thread;

        public int UpdatesMissed;

        public List<short> PacketsAwaitingConfirmation;

        protected bool isConnected = false;
        protected IPAddress clientAdress;
        protected Socket socket;

        public delegate void ClientDisconnectedDelegate(ServerClient client, ServerClient.ClosingReason reason);
        public event ClientDisconnectedDelegate ClientDisconnected;

        public delegate void ClientRequestUpdateDelegate(ServerClient client, short packetId);
        public event ClientRequestUpdateDelegate ClientRequestPacket;

        public string name;
        public short ID;

        public int joinPower;
        private bool admin;
        private string key;

        public bool IsAdmin
        {
            get
            {
                return this.admin;
            }
            private set { }
        }

        public string Key
        {
            get
            {
                return this.key;
            }
            private set {}
        }


        protected Channel curChannel;

        public enum ClosingReason { TimedOut, ClientDisconnect, Kicked, Banned };

        // Constructor
        public ServerClient(IPAddress addrs, string name, short id)
        {
            joinPower = 1;
            this.name = name;
            this.ID = id;
            this.clientAdress = addrs;
            UpdatesMissed = 0;

            PacketsAwaitingConfirmation = new List<short>();

            thread = new Thread(new ThreadStart(Init));
            thread.Start();
        }

        private void Init()
        {
            IPEndPoint endP = new IPEndPoint(this.clientAdress, ClientPort);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // Connect the socket to the remote endpoint (the client)
            socket.Connect(endP);
            isConnected = true;

            // Inform the client that he has connected so normal communication can start
            this.Send(MessageHandler.Messages.CONNECTED, BitConverter.GetBytes(this.ID));

            Thread.Sleep(2000);

            while (isConnected)
            {
                Send(MessageHandler.Messages.KEEPALIVE);
                UpdatesMissed++;

                if (UpdatesMissed > MAX_UPDATES_MISSED)
                {
                    Disconnect(ClosingReason.TimedOut);
                }
                
                foreach (short packetId in PacketsAwaitingConfirmation.ToList())
                {
                    RequestPacket(packetId);
                }
                PacketsAwaitingConfirmation.Clear();

                Thread.Sleep(2000);
            }
        }

        private void RequestPacket(short id)
        {
            if (ClientRequestPacket != null)
                ClientRequestPacket(this, id);
        }

        public void Disconnect(ClosingReason reason = ClosingReason.TimedOut)
        {
            if (socket != null)
            {
                try
                {
                    socket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("an error occured while closing sockets.\n" + e.Message);
                }
            }

            clientAdress = null;
            isConnected = false;
            OnClientDisconnected(reason);
        }

        // Send packet to client. Return: True on sent, false on error.
        public bool Send(MessageHandler.Messages message, byte[] data = null)
        {
            try
            {
                byte[] buffer = BitConverter.GetBytes((short)message);
                if (data != null)
                    buffer = buffer.Concat(data).ToArray<byte>();

                SocketAsyncEventArgs e = new SocketAsyncEventArgs();

                e.SetBuffer(buffer, 0, buffer.Length);
                e.Completed += new EventHandler<SocketAsyncEventArgs>(MessageSent);

                this.PacketsAwaitingConfirmation.Add((short)message);
                socket.SendAsync(e);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void MessageSent(object sender, SocketAsyncEventArgs e)
        {
            
        }

        public override string ToString()
        {
            StringBuilder serialized = new StringBuilder();

            // 'Encode' the seperators that are used so they don't interfire with the program
            serialized.Append(Data.Serialize(name));
            serialized.Append("," + this.ID);

            return serialized.ToString();
        }

        // Called when the client disconnects from the server (or times out)
        private void OnClientDisconnected(ClosingReason reason)
        {
            if (ClientDisconnected != null)
                ClientDisconnected(this, reason);
        }

        public void SwitchChannel(Channel newChannel)
        {
            if (curChannel != null)
                curChannel.Leave(this);

            newChannel.Join(this);
            curChannel = newChannel;
        }

        public void SetAdmin(bool state)
        {
            if (state)
                Administration.SetAdmin(this.key);
            else Administration.RemoveAdmin(this.key);

            this.Send(MessageHandler.Messages.SETADMIN, BitConverter.GetBytes(state));
            this.admin = true;
        }

        public void SetKey(string key)
        {
            if (Administration.KeyExists(key))
            {
                this.key = key;
                this.admin = Administration.IsAdmin(key);
                this.Send(MessageHandler.Messages.SETADMIN, BitConverter.GetBytes(this.admin));
                Console.WriteLine("SETKEY: " + this.name + ", KEY: " + this.key + ", ISADMIN: " + this.admin.ToString());
                //this.Send(MessageHandler.Messages.SETKEY, Encoding.ASCII.GetBytes(this.key));
            }
            else
                this.NewKey();
        }

        public void NewKey()
        {
            this.key = Administration.AddUserKey();
            Console.WriteLine("SETKEY: " + this.name + ", KEY: " + this.key + ", ISADMIN: " + this.admin.ToString());
            this.Send(MessageHandler.Messages.SETKEY, Encoding.ASCII.GetBytes(this.key));
        }

        public void RequestKey()
        {
            this.Send(MessageHandler.Messages.GETKEY, Encoding.ASCII.GetBytes(Administration.ServerKey));
        }
    }
}
