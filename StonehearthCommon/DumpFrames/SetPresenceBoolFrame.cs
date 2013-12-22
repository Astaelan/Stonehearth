using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class SetPresenceBoolFrame
    {
        public uint Field = 0;
        public bool Val = false;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Field);
            pWriter.Write(Val);
        }

        public SetPresenceBoolFrame Read(BinaryReader pReader)
        {
            Field = pReader.ReadUInt32();
            Val = pReader.ReadBoolean();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("SetPresenceBool");
            pWriter.Indent++;
            pWriter.WriteLine("Field: {0}", Field);
            pWriter.WriteLine("Val: {0}", Val);
            pWriter.Indent--;
        }
    }
}
