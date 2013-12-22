using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class StartScenarioAIFrame
    {
        public int Scenario = 0;
        public long DeckID = 0;
        public long AIDeckID = 0;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Scenario);
            pWriter.Write(DeckID);
            pWriter.Write(AIDeckID);
        }

        public StartScenarioAIFrame Read(BinaryReader pReader)
        {
            Scenario = pReader.ReadInt32();
            DeckID = pReader.ReadInt64();
            AIDeckID = pReader.ReadInt64();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("StartScenarioAI");
            pWriter.Indent++;
            pWriter.WriteLine("Scenario: {0}", Scenario);
            pWriter.WriteLine("DeckID: {0}", DeckID);
            pWriter.WriteLine("AIDeckID: {0}", AIDeckID);
            pWriter.Indent--;
        }
    }
}
