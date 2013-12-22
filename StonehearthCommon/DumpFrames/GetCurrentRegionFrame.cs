using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GetCurrentRegionFrame
    {
        public int Return = 0;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Return);
        }

        public GetCurrentRegionFrame Read(BinaryReader pReader)
        {
            Return = pReader.ReadInt32();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GetCurrentRegion");
            pWriter.Indent++;
            pWriter.WriteLine("Return: {0}", Return);
            pWriter.Indent--;
        }
    }
}
