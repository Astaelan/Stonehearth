using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class StartScenarioFrame
    {
        public int Scenario = 0;
        public long DeckID = 0;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Scenario);
            pWriter.Write(DeckID);
        }

        public StartScenarioFrame Read(BinaryReader pReader)
        {
            Scenario = pReader.ReadInt32();
            DeckID = pReader.ReadInt64();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("StartScenario");
            pWriter.Indent++;
            pWriter.WriteLine("Scenario: {0}", Scenario);
            pWriter.WriteLine("DeckID: {0}", DeckID);
            pWriter.Indent--;
        }
    }
}
