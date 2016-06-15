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
        private short _id;
        public short Id
        {
            get
            { return _id; }
            private set
            {
                _id = value;
            }
        }

        public int joinPower;
        private bool _isAdmin;
        private string _key;

        public bool IsAdmin
        {
            get
            {
                return this._isAdmin;
            }
            private set { }
        }

        public string Key
        {
            get
            {
                return this._key;
            }
            private set {}
        }


        protected Channel curChannel;
        internal bool initialized;

        public enum ClosingReason { TimedOut, ClientDisconnect, Kicked, Banned };

        // Constructor
        public ServerClient(IPAddress addrs, string name, short id) : base()
        {
            initialized = false;
            joinPower = 1;
            this.name = name;
            _id = id;
            clientAdress = addrs;
            UpdatesMissed = 0;
            IPEndPoint clientEndpoint = new IPEndPoint(clientAdress, ClientPort);
            PacketsAwaitingConfirmation = new List<short>();
            Connect(clientEndpoint);
            Send(new Packet(Messages.CONNECTED, BitConverter.GetBytes(Id)));
            StartTick(4);
            initialized = true;
        }

        protected override void Tick()
        {
            Send(new Packet(Messages.KEEPALIVE));
            UpdatesMissed++;

            if (UpdatesMissed > MAX_UPDATES_MISSED)
            {
                Disconnect();
            }

            foreach (short packetId in PacketsAwaitingConfirmation.ToList())
            {
                RequestPacket(packetId);
            }
            PacketsAwaitingConfirmation.Clear();
        }

        public override void MessageSending(Messages message)
        {
            if (initialized)
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
            serialized.Append("," + this.Id);

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
                Administration.SetAdmin(_key);
            else Administration.RemoveAdmin(_key);

            Send(new Packet(Messages.SETADMIN, BitConverter.GetBytes(state)));
            _isAdmin = true;
        }

        public void SetKey(string key)
        {
            if (Administration.KeyExists(key))
            {
                this._key = key;
                _isAdmin = Administration.IsAdmin(key);
                Send(new Packet(Messages.SETADMIN, BitConverter.GetBytes(_isAdmin)));
                Console.WriteLine("SETKEY: " + this.name + ", KEY: " + this._key + ", ISADMIN: " + this._isAdmin.ToString());
            }
            else
                NewKey();
        }

        public void NewKey()
        {
            _key = Administration.AddUserKey();
            Console.WriteLine("SETKEY: " + this.name + ", KEY: " + this._key + ", ISADMIN: " + this._isAdmin.ToString());
            Send(new Packet(Messages.SETKEY, Encoding.ASCII.GetBytes(_key)));
        }

        public void RequestKey()
        {
            Send(new Packet(Messages.GETKEY, Encoding.ASCII.GetBytes(Administration.ServerKey)));
        }
    }
}
