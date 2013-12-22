using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Game
{
    public sealed class GameServer : Server<GameClient>
    {
        public GameServer(ushort pPort) : base("GameServer", pPort) { }
    }
}
