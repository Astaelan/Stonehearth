using StonehearthCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StonehearthLauncher
{
    public sealed class LobbyClient : Client
    {
        public LobbyClientProtocolState ProtocolState = LobbyClientProtocolState.Handshake;
        private Dictionary<int, LobbyPacketHandlerAttribute> mLobbyPacketHandlers = new Dictionary<int, LobbyPacketHandlerAttribute>();

        public string Email = null;
        public string Password = null;
        public Guid Session = Guid.Empty;

        public LobbyClient()
        {
            Reflector.FindAllMethods<LobbyPacketHandlerAttribute, LobbyPacketProcessor>(Assembly.GetExecutingAssembly()).ForEach(d => { d.Item1.Processor = d.Item2; mLobbyPacketHandlers.Add(d.Item1.Opcode, d.Item1); });
            //OnLog += (c, l, f, a) => Log("<" + l.ToString() + "> " + f, a);
            OnConnect += c =>
            {
                Packet packet = new Packet((int)InternalPacketID.Handshake);
                packet.WriteInt((int)ProtocolVersion.Current);
                packet.WriteBool(false);
                SendPacket(packet);
            };
            //OnDisconnect += c => c.Log(LogLevel.Info, "Disconnected");
            OnPacket += (c, p) =>
            {
                LobbyPacketHandlerAttribute handler = null;
                if (!mLobbyPacketHandlers.TryGetValue(p.Opcode, out handler))
                {
                    Log(LogManagerLevel.Error, "Invalid Opcode: {0}", p.Opcode);
                    Disconnect();
                }
                else if ((handler.ProtocolState & ProtocolState) == LobbyClientProtocolState.None)
                {
                    Log(LogManagerLevel.Error, "Invalid Protocol State: {0}, {1} requires {2}", ProtocolState, p.Opcode, handler.ProtocolState);
                    Disconnect();
                }
                else handler.Processor(this, p);
            };
        }

        [LobbyPacketHandler((int)InternalPacketID.Handshake, LobbyClientProtocolState.Handshake)]
        public static void HandshakeHandler(LobbyClient pClient, Packet pPacket)
        {
            int result = 0;
            Guid salt = Guid.Empty;
            if (!pPacket.ReadInt(out result) ||
                result != 0 ||
                !pPacket.ReadGuid(out salt))
            {
                pClient.Disconnect();
                return;
            }

            pClient.ProtocolState = LobbyClientProtocolState.Authenticate;

            Packet packet = new Packet((int)InternalPacketID.Authenticate);
            packet.WriteString(pClient.Email);
            packet.WriteString(PasswordHasher.HashStrings(salt.ToString(), pClient.Password));
            pClient.SendPacket(packet);
        }

        [LobbyPacketHandler((int)InternalPacketID.Authenticate, LobbyClientProtocolState.Authenticate)]
        public static void AuthenticateHandler(LobbyClient pClient, Packet pPacket)
        {
            int result = 0;
            Guid session = Guid.Empty;
            if (pPacket.ReadInt(out result) &&
                result == 0 &&
                pPacket.ReadGuid(out session)) pClient.Session = session;
            pClient.Disconnect();
        }
    }
}
