using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class SetPresenceStringFrame
    {
        public uint Field = 0;
        public string Val = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Field);
            pWriter.Write(Val != null);
            if (Val != null) pWriter.Write(Val);
        }

        public SetPresenceStringFrame Read(BinaryReader pReader)
        {
            Field = pReader.ReadUInt32();
            if (pReader.ReadBoolean()) Val = pReader.ReadString();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("SetPresenceString");
            pWriter.Indent++;
            pWriter.WriteLine("Field: {0}", Field);
            if (Val != null) pWriter.WriteLine("Val: {0}", Val);
            pWriter.Indent--;
        }
    }
}
