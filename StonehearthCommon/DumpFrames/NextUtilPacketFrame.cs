using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class NextUtilPacketFrame
    {
        public int PacketID = 0;
        public int Context = 0;
        public byte[] Body = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(PacketID);
            pWriter.Write(Context);
            pWriter.Write(Body.Length);
            pWriter.Write(Body);
        }

        public NextUtilPacketFrame Read(BinaryReader pReader)
        {
            PacketID = pReader.ReadInt32();
            Context = pReader.ReadInt32();
            int bodyLength = pReader.ReadInt32();
            Body = pReader.ReadBytes(bodyLength);
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("NextUtilPacket");
            pWriter.Indent++;
            pWriter.WriteLine("PacketID: {0}", PacketID);
            pWriter.WriteLine("Context: {0}", Context);
            pWriter.WriteLine("BodyLength: {0}", Body.Length);
            DumpFrameExternals.DumpUtilPacket(pWriter, PacketID, Body);
            pWriter.Indent--;
        }
    }
}
