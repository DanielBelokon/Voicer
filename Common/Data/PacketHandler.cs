using System;
using Voicer.Common.Net;

namespace Voicer.Common.Data
{
    public class PacketHandler : MessageHandler
    {

        public void HandlePacket(Packet packet)
        {
            Handle((short)packet.Type, packet);
        }

        public void AddPacketHandler(Messages messageEnum, Delegate function)
        {
            AddHandler((short)messageEnum, function);
        }
    }
}
