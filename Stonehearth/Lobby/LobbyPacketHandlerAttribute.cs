using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Lobby
{
    public delegate void LobbyPacketProcessor(LobbyClient pClient, Packet pPacket);
    public sealed class LobbyPacketHandlerAttribute : Attribute
    {
        public readonly int Opcode;
        public readonly LobbyClientProtocolState ProtocolState;
        public LobbyPacketProcessor Processor;

        public LobbyPacketHandlerAttribute(int pOpcode, LobbyClientProtocolState pProtocolState) { Opcode = pOpcode; ProtocolState = pProtocolState; }
    }
}
