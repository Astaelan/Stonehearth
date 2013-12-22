using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class InitFrame
    {
        public bool Return = false;
        public bool FromEditor = false;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Return);
            pWriter.Write(FromEditor);
        }

        public InitFrame Read(BinaryReader pReader)
        {
            Return = pReader.ReadBoolean();
            FromEditor = pReader.ReadBoolean();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("Init");
            pWriter.Indent++;
            pWriter.WriteLine("Return: {0}", Return);
            pWriter.WriteLine("FromEditor: {0}", FromEditor);
            pWriter.Indent--;
        }
    }
}
