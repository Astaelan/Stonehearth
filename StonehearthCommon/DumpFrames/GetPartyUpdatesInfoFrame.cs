using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GetPartyUpdatesInfoFrame
    {
        public BattleNet.DllPartyInfo Info = new BattleNet.DllPartyInfo();

        public void Write(BinaryWriter pWriter)
        {
            DumpFrameExternals.Write(pWriter, Info);
        }

        public GetPartyUpdatesInfoFrame Read(BinaryReader pReader)
        {
            DumpFrameExternals.Read(pReader, ref Info);
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GetPartyUpdatesInfo");
            pWriter.Indent++;
            pWriter.WriteLine("Info");
            pWriter.Indent++;
            DumpFrameExternals.Dump(pWriter, Info);
            pWriter.Indent--;
            pWriter.Indent--;
        }
    }
}
