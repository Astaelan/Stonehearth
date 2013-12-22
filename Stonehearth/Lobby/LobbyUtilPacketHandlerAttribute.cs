using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Lobby
{
    public delegate void LobbyUtilPacketProcessor(LobbyClient pClient, int pContext, byte[] pData);
    public sealed class LobbyUtilPacketHandlerAttribute : Attribute
    {
        public readonly int Opcode;
        public LobbyUtilPacketProcessor Processor;

        public LobbyUtilPacketHandlerAttribute(int pOpcode) { Opcode = pOpcode; }
    }
}
