using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class RankedMatchFrame
    {
        public long DeckID = 0;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(DeckID);
        }

        public RankedMatchFrame Read(BinaryReader pReader)
        {
            DeckID = pReader.ReadInt64();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("RankedMatch");
            pWriter.Indent++;
            pWriter.WriteLine("DeckID: {0}", DeckID);
            pWriter.Indent--;
        }
    }
}
