using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GetAccountCountryFrame
    {
        public string Return = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Return);
        }

        public GetAccountCountryFrame Read(BinaryReader pReader)
        {
            Return = pReader.ReadString();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GetAccountCountry");
            pWriter.Indent++;
            pWriter.WriteLine("Return: {0}", Return);
            pWriter.Indent--;
        }
    }
}
