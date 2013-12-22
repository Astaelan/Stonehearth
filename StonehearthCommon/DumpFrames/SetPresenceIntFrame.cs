using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class SetPresenceIntFrame
    {
        public uint Field = 0;
        public long Val = 0;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Field);
            pWriter.Write(Val);
        }

        public SetPresenceIntFrame Read(BinaryReader pReader)
        {
            Field = pReader.ReadUInt32();
            Val = pReader.ReadInt64();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("SetPresenceInt");
            pWriter.Indent++;
            pWriter.WriteLine("Field: {0}", Field);
            pWriter.WriteLine("Val: {0}", Val);
            pWriter.Indent--;
        }
    }
}
