using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GameFinishedFrame
    {
        public string Filter = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Filter);
        }

        public GameFinishedFrame Read(BinaryReader pReader)
        {
            Filter = pReader.ReadString();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GameFinished");
            pWriter.Indent++;
            pWriter.WriteLine("Filter: {0}", Filter);
            pWriter.Indent--;
        }
    }
}
