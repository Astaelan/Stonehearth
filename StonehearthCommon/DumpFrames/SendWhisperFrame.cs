using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class SendWhisperFrame
    {
        public BnetGameAccountId GameAccount = null;
        public string Message = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(GameAccount != null);
            if (GameAccount != null) DumpFrameExternals.Write(pWriter, GameAccount);
            pWriter.Write(Message != null);
            if (Message != null) pWriter.Write(Message);
        }

        public SendWhisperFrame Read(BinaryReader pReader)
        {
            if (pReader.ReadBoolean())
            {
                GameAccount = new BnetGameAccountId();
                DumpFrameExternals.Read(pReader, GameAccount);
            }
            if (pReader.ReadBoolean()) Message = pReader.ReadString();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("SendWhisper");
            pWriter.Indent++;
            if (GameAccount != null)
            {
                pWriter.WriteLine("GameAccount");
                pWriter.Indent++;
                DumpFrameExternals.Dump(pWriter, GameAccount);
                pWriter.Indent--;
            }
            if (Message != null) pWriter.WriteLine("Message: {0}", Message);
            pWriter.Indent--;
        }
    }
}
