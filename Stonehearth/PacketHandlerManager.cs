using Stonehearth.Game;
using Stonehearth.Lobby;
using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Stonehearth
{
    public static class PacketHandlerManager
    {
        public static Dictionary<int, LobbyPacketHandlerAttribute> LobbyPacketHandlers = new Dictionary<int, LobbyPacketHandlerAttribute>();
        public static Dictionary<int, LobbyUtilPacketHandlerAttribute> LobbyUtilPacketHandlers = new Dictionary<int, LobbyUtilPacketHandlerAttribute>();
        public static Dictionary<int, GamePacketHandlerAttribute> GamePacketHandlers = new Dictionary<int, GamePacketHandlerAttribute>();

        public static void Initialize()
        {
            Reflector.FindAllMethods<LobbyPacketHandlerAttribute, LobbyPacketProcessor>(Assembly.GetExecutingAssembly()).ForEach(d => { d.Item1.Processor = d.Item2; LobbyPacketHandlers.Add(d.Item1.Opcode, d.Item1); });
            Reflector.FindAllMethods<LobbyUtilPacketHandlerAttribute, LobbyUtilPacketProcessor>(Assembly.GetExecutingAssembly()).ForEach(d => { d.Item1.Processor = d.Item2; LobbyUtilPacketHandlers.Add(d.Item1.Opcode, d.Item1); });
            Reflector.FindAllMethods<GamePacketHandlerAttribute, GamePacketProcessor>(Assembly.GetExecutingAssembly()).ForEach(d => { d.Item1.Processor = d.Item2; GamePacketHandlers.Add(d.Item1.Opcode, d.Item1); });
        }
    }
}
