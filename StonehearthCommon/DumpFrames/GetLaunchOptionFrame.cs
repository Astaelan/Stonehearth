using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GetLaunchOptionFrame
    {
        public string Return = null;
        public string Key = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Return);
            pWriter.Write(Key);
        }

        public GetLaunchOptionFrame Read(BinaryReader pReader)
        {
            Return = pReader.ReadString();
            Key = pReader.ReadString();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GetLaunchOption");
            pWriter.Indent++;
            pWriter.WriteLine("Return: {0}", Return);
            pWriter.WriteLine("Key: {0}", Key);
            pWriter.Indent--;
        }
    }
}
