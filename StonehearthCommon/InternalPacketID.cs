using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StonehearthCommon
{
    public enum InternalPacketID : int
    {
        Handshake = 500,
        Authenticate = 501,
        Authorize = 502,
        UtilPacket = 503,
        StartScenario = 504,
    }
}
