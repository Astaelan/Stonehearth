using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class CancelChallengeFrame
    {
        public ulong ChallengeID = 0;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(ChallengeID);
        }

        public CancelChallengeFrame Read(BinaryReader pReader)
        {
            ChallengeID = pReader.ReadUInt64();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("CancelChallenge");
            pWriter.Indent++;
            pWriter.WriteLine("ChallengeID: {0}", ChallengeID);
            pWriter.Indent--;
        }
    }
}
