using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GetWhisperInfoFrame
    {
        public BattleNet.DllWhisperInfo Info = new BattleNet.DllWhisperInfo();

        public void Write(BinaryWriter pWriter)
        {
            DumpFrameExternals.Write(pWriter, Info);
        }

        public GetWhisperInfoFrame Read(BinaryReader pReader)
        {
            DumpFrameExternals.Read(pReader, ref Info);
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GetWhisperInfo");
            pWriter.Indent++;
            pWriter.WriteLine("Info");
            pWriter.Indent++;
            DumpFrameExternals.Dump(pWriter, Info);
            pWriter.Indent--;
            pWriter.Indent--;
        }
    }
}
