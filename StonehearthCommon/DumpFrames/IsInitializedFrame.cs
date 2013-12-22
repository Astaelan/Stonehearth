using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class IsInitializedFrame
    {
        public bool Return = false;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Return);
        }

        public IsInitializedFrame Read(BinaryReader pReader)
        {
            Return = pReader.ReadBoolean();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("IsInitialized");
            pWriter.Indent++;
            pWriter.WriteLine("Return: {0}", Return);
            pWriter.Indent--;
        }
    }
}
