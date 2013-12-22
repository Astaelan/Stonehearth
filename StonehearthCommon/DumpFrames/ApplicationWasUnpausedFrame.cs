using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class ApplicationWasUnpausedFrame
    {
        public void Write(BinaryWriter pWriter)
        {
        }

        public ApplicationWasUnpausedFrame Read(BinaryReader pReader)
        {
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("ApplicationWasUnpaused");
        }
    }
}
