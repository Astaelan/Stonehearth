using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StonehearthCommon
{
    [Flags]
    public enum LobbyClientProtocolState
    {
        None = 0 << 0,
        Handshake = 1 << 0,
        Authenticate = 1 << 1,
        Authorize = 1 << 2,
        Online = 1 << 3
    }
}
