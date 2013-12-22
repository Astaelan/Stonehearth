using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GetMyGameAccountIdFrame
    {
        public BattleNet.DllEntityId Return = new BattleNet.DllEntityId();

        public void Write(BinaryWriter pWriter)
        {
            DumpFrameExternals.Write(pWriter, Return);
        }

        public GetMyGameAccountIdFrame Read(BinaryReader pReader)
        {
            DumpFrameExternals.Read(pReader, ref Return);
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GetMyGameAccountId");
            pWriter.Indent++;
            pWriter.WriteLine("Return");
            pWriter.Indent++;
            DumpFrameExternals.Dump(pWriter, Return);
            pWriter.Indent--;
            pWriter.Indent--;
        }
    }
}
