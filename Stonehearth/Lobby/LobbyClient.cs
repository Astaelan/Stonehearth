using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stonehearth.Lobby
{
    public sealed class LobbyClient : Client
    {
        public LobbyClientProtocolState ProtocolState = LobbyClientProtocolState.Handshake;
        public Guid AuthenticateSalt = Guid.NewGuid();
        public Guid AuthenticateSession = Guid.NewGuid();
        public Data.Account Account = null;
        public long ClientHandle = 0;

        public void SendUtilPacket(int pPacketID, int pContext, byte[] pData, bool pFlushThenDisconnect = false)
        {
            Packet packet = new Packet((int)InternalPacketID.UtilPacket);
            packet.WriteInt(pPacketID);
            packet.WriteInt(pContext);
            packet.WriteBytes(pData);
            SendPacket(packet, pFlushThenDisconnect);
        }
    }
}
