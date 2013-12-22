using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GameStartedFrame
    {
        public string Filter = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Filter);
        }

        public GameStartedFrame Read(BinaryReader pReader)
        {
            Filter = pReader.ReadString();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GameStarted");
            pWriter.Indent++;
            pWriter.WriteLine("Filter: {0}", Filter);
            pWriter.Indent--;
        }
    }
}
