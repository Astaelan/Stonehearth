using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class ClearWhispersFrame
    {
        public void Write(BinaryWriter pWriter)
        {
        }

        public ClearWhispersFrame Read(BinaryReader pReader)
        {
            return this;
        }
        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("ClearWhispers");
        }

    }
}
