using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GetQueueEventFrame
    {
        public BattleNet.QueueEvent QueueEvent = new BattleNet.QueueEvent(BattleNet.QueueEvent.Type.UNKNOWN);

        public void Write(BinaryWriter pWriter)
        {
            DumpFrameExternals.Write(pWriter, QueueEvent);
        }

        public GetQueueEventFrame Read(BinaryReader pReader)
        {
            DumpFrameExternals.Read(pReader, QueueEvent);
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GetQueueEvent");
            pWriter.Indent++;
            pWriter.WriteLine("QueueEvent");
            pWriter.Indent++;
            DumpFrameExternals.Dump(pWriter, QueueEvent);
            pWriter.Indent--;
            pWriter.Indent--;
        }
    }
}
