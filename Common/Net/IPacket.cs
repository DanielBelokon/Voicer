using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voicer.Common.Net
{
    public interface IPacket : IDisposable
    {
        Messages Type
        {
            get;
        }

        byte[] Encode();
    }
}
