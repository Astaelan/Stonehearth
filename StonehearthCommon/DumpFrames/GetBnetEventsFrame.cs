using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GetBnetEventsFrame
    {
        public BattleNet.BnetEvent[] Events = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Events.Length);
            Array.ForEach(Events, e => pWriter.Write((int)e));
        }

        public GetBnetEventsFrame Read(BinaryReader pReader)
        {
            int eventsLength = pReader.ReadInt32();
            Events = new BattleNet.BnetEvent[eventsLength];
            for (int eventsIndex = 0; eventsIndex < eventsLength; ++eventsIndex) Events[eventsIndex] = (BattleNet.BnetEvent)pReader.ReadInt32();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GetBnetEvents");
            pWriter.Indent++;
            pWriter.WriteLine("Events: {0}", Events.Length);
            if (Events.Length > 0)
            {
                pWriter.Indent++;
                Array.ForEach(Events, v => pWriter.WriteLine("Event: {0}", v));
                pWriter.Indent--;
            }
            pWriter.Indent--;
        }
    }
}
