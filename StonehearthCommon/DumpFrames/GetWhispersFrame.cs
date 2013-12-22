using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GetWhispersFrame
    {
        public BnetWhisper[] Whispers = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Whispers.Length);
            Array.ForEach(Whispers, w => DumpFrameExternals.Write(pWriter, w));
        }

        public GetWhispersFrame Read(BinaryReader pReader)
        {
            int whispersLength = pReader.ReadInt32();
            Whispers = new BnetWhisper[whispersLength];
            for (int whispersIndex = 0; whispersIndex < whispersLength; ++whispersIndex)
            {
                Whispers[whispersIndex] = new BnetWhisper();
                DumpFrameExternals.Read(pReader, Whispers[whispersIndex]);
            }
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GetWhispers");
            pWriter.Indent++;
            pWriter.WriteLine("Whispers: {0}", Whispers.Length);
            if (Whispers.Length > 0)
            {
                pWriter.Indent++;
                foreach (BnetWhisper value in Whispers)
                {
                    pWriter.WriteLine("Whisper");
                    pWriter.Indent++;
                    DumpFrameExternals.Dump(pWriter, value);
                    pWriter.Indent--;
                }
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }
    }
}
