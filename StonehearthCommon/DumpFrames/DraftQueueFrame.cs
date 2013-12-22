using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class DraftQueueFrame
    {
        public bool Join = false;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Join);
        }

        public DraftQueueFrame Read(BinaryReader pReader)
        {
            Join = pReader.ReadBoolean();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("DraftQueue");
            pWriter.Indent++;
            pWriter.WriteLine("Join: {0}", Join);
            pWriter.Indent--;
        }
    }
}
