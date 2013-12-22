using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class RescindPartyInviteFrame
    {
        public BattleNet.DllEntityId PartyID = new BattleNet.DllEntityId();

        public void Write(BinaryWriter pWriter)
        {
            DumpFrameExternals.Write(pWriter, PartyID);
        }

        public RescindPartyInviteFrame Read(BinaryReader pReader)
        {
            DumpFrameExternals.Read(pReader, ref PartyID);
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("RescindPartyInvite");
            pWriter.Indent++;
            pWriter.WriteLine("PartyID");
            pWriter.Indent++;
            DumpFrameExternals.Dump(pWriter, PartyID);
            pWriter.Indent--;
            pWriter.Indent--;
        }
    }
}
