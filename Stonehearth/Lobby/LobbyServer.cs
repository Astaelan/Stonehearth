using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Lobby
{
    public sealed class LobbyServer : Server<LobbyClient>
    {
        public LobbyServer(ushort pPort) : base("LobbyServer", pPort) { }
    }
}
