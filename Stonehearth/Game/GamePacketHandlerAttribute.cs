using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Game
{
    public delegate void GamePacketProcessor(GameClient pClient, Packet pPacket);
    public sealed class GamePacketHandlerAttribute : Attribute
    {
        public readonly int Opcode;
        public readonly GameClientProtocolState ProtocolState;
        public GamePacketProcessor Processor;

        public GamePacketHandlerAttribute(int pOpcode, GameClientProtocolState pProtocolState) { Opcode = pOpcode; ProtocolState = pProtocolState; }
    }
}
