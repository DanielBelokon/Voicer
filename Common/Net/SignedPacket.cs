using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voicer.Common.Net
{
    public class SignedPacket : IPacket
    {
        private Messages _type;
        private short _senderId;
        private byte[] _data;

        public SignedPacket(Messages type, short senderId, byte[] data = null)
        {
            _type = type;
            _data = data;
            _senderId = senderId;
        }

        public Messages Type
        {
            get
            {
                return _type;
            }
        }

        public void Dispose()
        {
            _data = null;
            _type = 0;
            _senderId = 0;

        }

        public byte[] Encode()
        {
            byte[] buffer = BitConverter.GetBytes((short)_type);

            buffer = buffer.Concat(BitConverter.GetBytes(_senderId)).ToArray();

            if (_data != null)
                buffer = buffer.Concat(_data).ToArray();

            return buffer;
        }
    }
}
