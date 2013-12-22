using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GetFriendsUpdatesFrame
    {
        public BattleNet.FriendsUpdate[] Updates = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Updates.Length);
            Array.ForEach(Updates, u => DumpFrameExternals.Write(pWriter, u));
        }

        public GetFriendsUpdatesFrame Read(BinaryReader pReader)
        {
            int updatesLength = pReader.ReadInt32();
            Updates = new BattleNet.FriendsUpdate[updatesLength];
            for (int updatesIndex = 0; updatesIndex < updatesLength; ++updatesIndex) DumpFrameExternals.Read(pReader, ref Updates[updatesIndex]);
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GetFriendsUpdates");
            pWriter.Indent++;
            pWriter.WriteLine("Updates: {0}", Updates.Length);
            if (Updates.Length > 0)
            {
                pWriter.Indent++;
                foreach (BattleNet.FriendsUpdate value in Updates)
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
