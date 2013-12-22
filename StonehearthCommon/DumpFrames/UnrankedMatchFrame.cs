using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class UnrankedMatchFrame
    {
        public long DeckID = 0;
        public bool Newbie = false;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(DeckID);
            pWriter.Write(Newbie);
        }

        public UnrankedMatchFrame Read(BinaryReader pReader)
        {
            DeckID = pReader.ReadInt64();
            Newbie = pReader.ReadBoolean();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("UnrankedMatch");
            pWriter.Indent++;
            pWriter.WriteLine("DeckID: {0}", DeckID);
            pWriter.WriteLine("Newbie: {0}", Newbie);
            pWriter.Indent--;
        }
    }
}
