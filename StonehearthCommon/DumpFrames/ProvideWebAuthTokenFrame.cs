using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class ProvideWebAuthTokenFrame
    {
        public string Token = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Token);
        }

        public ProvideWebAuthTokenFrame Read(BinaryReader pReader)
        {
            Token = pReader.ReadString();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("ProvideWebAuthToken");
            pWriter.Indent++;
            pWriter.WriteLine("Token: {0}", Token);
            pWriter.Indent--;
        }
    }
}
