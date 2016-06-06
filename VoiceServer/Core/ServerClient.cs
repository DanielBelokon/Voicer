using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Voicer.Common.Data;
using Voicer.Common.Net;

namespace VoiceServer
{
    public class ServerClient : NetworkClient
    {
        // Max KeepAlive packets a user can miss before he times out & forced to disconnect.
        const int MAX_UPDATES_MISSED = 5;

        const int ClientPort = 9998;

        public int UpdatesMissed;

        public List<short> PacketsAwaitingConfirmation;

        protected IPAddress clientAdress;

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
        public ServerClient(IPAddress addrs, string name, short id) : base()
        {
            joinPower = 1;
            this.name = name;
            ID = id;
            clientAdress = addrs;
            UpdatesMissed = 0;
            IPEndPoint clientEndpoint = new IPEndPoint(clientAdress, ClientPort);
            PacketsAwaitingConfirmation = new List<short>();
            Connect(clientEndpoint);
            Send(new Packet(Packet.Messages.CONNECTED, BitConverter.GetBytes(ID)));
            StartTick(4);
        }

        protected override void Tick()
        {
            Send(new Packet(Packet.Messages.KEEPALIVE));
            UpdatesMissed++;

            if (UpdatesMissed > MAX_UPDATES_MISSED)
            {
                Disconnect();
            }

            foreach (short packetId in PacketsAwaitingConfirmation.ToList())
            {
                //RequestPacket(packetId);
            }
            PacketsAwaitingConfirmation.Clear();
        }

        public override void MessageSending(Packet.Messages message)
        {
            PacketsAwaitingConfirmation.Add((short)message);
        }

        private void RequestPacket(short id)
        {
            if (ClientRequestPacket != null)
                ClientRequestPacket(this, id);
        }

        public override void Disconnecting()
        {
            clientAdress = null;
            OnClientDisconnected(ClosingReason.ClientDisconnect);
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
                Administration.SetAdmin(key);
            else Administration.RemoveAdmin(key);

            Send(new Packet(Packet.Messages.SETADMIN, BitConverter.GetBytes(state)));
            admin = true;
        }

        public void SetKey(string key)
        {
            if (Administration.KeyExists(key))
            {
                this.key = key;
                admin = Administration.IsAdmin(key);
                Send(new Packet(Packet.Messages.SETADMIN, BitConverter.GetBytes(admin)));
                Console.WriteLine("SETKEY: " + this.name + ", KEY: " + this.key + ", ISADMIN: " + this.admin.ToString());
            }
            else
                NewKey();
        }

        public void NewKey()
        {
            key = Administration.AddUserKey();
            Console.WriteLine("SETKEY: " + this.name + ", KEY: " + this.key + ", ISADMIN: " + this.admin.ToString());
            Send(new Packet(Packet.Messages.SETKEY, Encoding.ASCII.GetBytes(key)));
        }

        public void RequestKey()
        {
            Send(new Packet(Packet.Messages.GETKEY, Encoding.ASCII.GetBytes(Administration.ServerKey)));
        }
    }
}
