using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GetQueueInfoFrame
    {
        public BattleNet.DllQueueInfo QueueInfo = new BattleNet.DllQueueInfo();

        public void Write(BinaryWriter pWriter)
        {
            DumpFrameExternals.Write(pWriter, QueueInfo);
        }

        public GetQueueInfoFrame Read(BinaryReader pReader)
        {
            DumpFrameExternals.Read(pReader, ref QueueInfo);
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GetQueueInfo");
            pWriter.Indent++;
            pWriter.WriteLine("QueueInfo");
            pWriter.Indent++;
            DumpFrameExternals.Dump(pWriter, QueueInfo);
            pWriter.Indent--;
            pWriter.Indent--;
        }
    }
}
