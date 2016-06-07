using System;
using System.Linq;
using System.Net;

namespace Voicer.Common.Net
{
    public class Packet : IPacket
    {
        private IPEndPoint sender;
        public IPEndPoint Sender
        {
            get
            { return sender; }
            private set
            { sender = value; }
        }

        private byte[] data;
        public byte[] Data
        {
            get
            { return data; }
            private set
            { data = value; }
        }

        private Messages type;
        public Messages Type
        {
            get
            { return type; }
            private set
            { type = value; }
        }

        public Packet(Messages type, byte[] data = null)
        {
            this.type = type;
            this.data = data;
        }

        public Packet(byte[] data, IPEndPoint endP)
        {
            sender = endP;
            type = (Messages)BitConverter.ToInt16(data, 0);
            if (data.Length > 2)
                this.data = data.Skip(2).ToArray();
            else this.data = null;
        }

        public virtual byte[] Encode()
        {
            byte[] buffer = BitConverter.GetBytes((short)type);
            if (data != null)
                buffer = buffer.Concat(data).ToArray();

            return buffer;
        }

        public void Dispose()
        {
            data = null;
            type = 0;
            sender = null;
        }
    }
}
