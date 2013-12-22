using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class SetPartyDeckFrame
    {
        public BattleNet.DllEntityId PartyID = new BattleNet.DllEntityId();
        public long DeckID = 0;

        public void Write(BinaryWriter pWriter)
        {
            DumpFrameExternals.Write(pWriter, PartyID);
            pWriter.Write(DeckID);
        }

        public SetPartyDeckFrame Read(BinaryReader pReader)
        {
            DumpFrameExternals.Read(pReader, ref PartyID);
            DeckID = pReader.ReadInt64();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("SetPartyDeck");
            pWriter.Indent++;
            pWriter.WriteLine("PartyID");
            pWriter.Indent++;
            DumpFrameExternals.Dump(pWriter, PartyID);
            pWriter.Indent--;
            pWriter.WriteLine("DeckID: {0}", DeckID);
            pWriter.Indent--;
        }
    }
}
