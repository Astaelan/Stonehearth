using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StonehearthRuntime
{
    public delegate void PacketProcessor(Connector pClient, Packet pPacket);
    public sealed class PacketHandlerAttribute : Attribute
    {
        public readonly int Opcode;
        public readonly LobbyClientProtocolState ProtocolState;
        public PacketProcessor Processor;

        public PacketHandlerAttribute(int pOpcode) { Opcode = pOpcode; ProtocolState = LobbyClientProtocolState.Online; }
        public PacketHandlerAttribute(int pOpcode, LobbyClientProtocolState pProtocolState) { Opcode = pOpcode; ProtocolState = pProtocolState; }
    }
}
