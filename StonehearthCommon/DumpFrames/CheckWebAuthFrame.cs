using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class CheckWebAuthFrame
    {
        public bool Return = false;
        public string Url = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Return);
            pWriter.Write(Url);
        }

        public CheckWebAuthFrame Read(BinaryReader pReader)
        {
            Return = pReader.ReadBoolean();
            Url = pReader.ReadString();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("CheckWebAuth");
            pWriter.Indent++;
            pWriter.WriteLine("Return: {0}", Return);
            pWriter.WriteLine("Url: {0}", Url);
            pWriter.Indent--;
        }
    }
}
