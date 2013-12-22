using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class FilterProfanityFrame
    {
        public string Return = null;
        public string Unfiltered = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Return);
            pWriter.Write(Unfiltered);
        }

        public FilterProfanityFrame Read(BinaryReader pReader)
        {
            Return = pReader.ReadString();
            Unfiltered = pReader.ReadString();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("FilterProfanity");
            pWriter.Indent++;
            pWriter.WriteLine("Return: {0}", Return);
            pWriter.WriteLine("Unfiltered: {0}", Unfiltered);
            pWriter.Indent--;
        }
    }
}
