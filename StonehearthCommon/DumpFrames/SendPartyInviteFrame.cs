using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class SendPartyInviteFrame
    {
        public BattleNet.DllEntityId GameAccount = new BattleNet.DllEntityId();

        public void Write(BinaryWriter pWriter)
        {
            DumpFrameExternals.Write(pWriter, GameAccount);
        }

        public SendPartyInviteFrame Read(BinaryReader pReader)
        {
            DumpFrameExternals.Read(pReader, ref GameAccount);
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("SendPartyInvite");
            pWriter.Indent++;
            pWriter.WriteLine("GameAccount");
            pWriter.Indent++;
            DumpFrameExternals.Dump(pWriter, GameAccount);
            pWriter.Indent--;
            pWriter.Indent--;
        }
    }
}
