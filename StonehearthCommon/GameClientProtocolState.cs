using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StonehearthCommon
{
    [Flags]
    public enum GameClientProtocolState
    {
        None = 0 << 0,
        AuroraHandshake = 1 << 0,
        GetGameState = 1 << 1,
        Online = 1 << 2
    }
}
