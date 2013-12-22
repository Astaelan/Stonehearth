using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class ClearPresenceFrame
    {
        public void Write(BinaryWriter pWriter)
        {
        }

        public ClearPresenceFrame Read(BinaryReader pReader)
        {
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("ClearPresence");
        }
    }
}
