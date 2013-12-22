using Stonehearth.Lobby;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth
{
    public sealed class MatchManager
    {
        // TODO: Need to make these threadsafe dictionaries
        public static Dictionary<int, Match> MatchByGameHandle = new Dictionary<int, Match>();
        public static Dictionary<long, LobbyClient> LobbyClientByClientHandle = new Dictionary<long, LobbyClient>();

        public static void Initialize()
        {
        }

        public static Match CreateMatch()
        {
            Random random = new Random();
            Match match = new Match();
            match.GameHandle = random.Next();
            while (MatchByGameHandle.ContainsKey(match.GameHandle)) match.GameHandle = random.Next();
            MatchByGameHandle.Add(match.GameHandle, match);
            return match;
        }
    }
}
