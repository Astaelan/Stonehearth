using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class SetRichPresenceFrame
    {
        public BattleNet.DllRichPresenceUpdate[] Updates = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Updates.Length);
            Array.ForEach(Updates, w => DumpFrameExternals.Write(pWriter, w));
        }

        public SetRichPresenceFrame Read(BinaryReader pReader)
        {
            int updatesLength = pReader.ReadInt32();
            Updates = new BattleNet.DllRichPresenceUpdate[updatesLength];
            for (int updatesIndex = 0; updatesIndex < updatesLength; ++updatesIndex) DumpFrameExternals.Read(pReader, ref Updates[updatesIndex]);
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("SetRichPresence");
            pWriter.Indent++;
            pWriter.WriteLine("Updates: {0}", Updates.Length);
            if (Updates.Length > 0)
            {
                pWriter.Indent++;
                foreach (BattleNet.DllRichPresenceUpdate value in Updates)
                {
                    pWriter.WriteLine("Update");
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
