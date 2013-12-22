using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GetChallengesFrame
    {
        public BattleNet.DllChallengeInfo[] Challenges = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Challenges.Length);
            Array.ForEach(Challenges, w => DumpFrameExternals.Write(pWriter, w));
        }

        public GetChallengesFrame Read(BinaryReader pReader)
        {
            int challengesLength = pReader.ReadInt32();
            Challenges = new BattleNet.DllChallengeInfo[challengesLength];
            for (int challengesIndex = 0; challengesIndex < challengesLength; ++challengesIndex) DumpFrameExternals.Read(pReader, ref Challenges[challengesIndex]);
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GetChallenges");
            pWriter.Indent++;
            pWriter.WriteLine("Challenges: {0}", Challenges.Length);
            if (Challenges.Length > 0)
            {
                pWriter.Indent++;
                foreach (BattleNet.DllChallengeInfo value in Challenges)
                {
                    pWriter.WriteLine("Challenge");
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
