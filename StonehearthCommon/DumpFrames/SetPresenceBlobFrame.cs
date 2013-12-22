using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class SetPresenceBlobFrame
    {
        public uint Field = 0;
        public byte[] Val = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Field);
            pWriter.Write(Val != null);
            if (Val != null)
            {
                pWriter.Write(Val.Length);
                pWriter.Write(Val);
            }
        }

        public SetPresenceBlobFrame Read(BinaryReader pReader)
        {
            Field = pReader.ReadUInt32();
            if (pReader.ReadBoolean())
            {
                int valLength = pReader.ReadInt32();
                Val = pReader.ReadBytes(valLength);
            }
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("SetPresenceBlob");
            pWriter.Indent++;
            pWriter.WriteLine("Field: {0}", Field);
            if (Val != null) pWriter.WriteLine("Val: {0}", BitConverter.ToString(Val).Replace('-', ' '));
            pWriter.Indent--;
        }
    }
}
