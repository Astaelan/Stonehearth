using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Game
{
    public sealed class GameClient : Client
    {
        public GameClientProtocolState ProtocolState = GameClientProtocolState.AuroraHandshake;
        public long AccountID = 0;
        public Match Match = null;
        public MatchPlayer MatchPlayer = null;
    }
}
