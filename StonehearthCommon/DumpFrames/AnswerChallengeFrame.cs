using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class AnswerChallengeFrame
    {
        public ulong ChallengeID = 0;
        public string Answer = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(ChallengeID);
            pWriter.Write(Answer != null);
            if (Answer != null) pWriter.Write(Answer);
        }

        public AnswerChallengeFrame Read(BinaryReader pReader)
        {
            ChallengeID = pReader.ReadUInt64();
            if (pReader.ReadBoolean()) Answer = pReader.ReadString();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("AnswerChallenge");
            pWriter.Indent++;
            pWriter.WriteLine("ChallengeID: {0}", ChallengeID);
            if (Answer != null) pWriter.WriteLine("Answer: {0}", Answer);
            pWriter.Indent--;
        }
    }
}
