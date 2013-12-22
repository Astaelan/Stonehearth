using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GetPlayRestrictionsFrame
    {
        public bool Reload = false;
        public BattleNet.DllLockouts Restrictions = new BattleNet.DllLockouts();

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Reload);
            DumpFrameExternals.Write(pWriter, Restrictions);
        }

        public GetPlayRestrictionsFrame Read(BinaryReader pReader)
        {
            Reload = pReader.ReadBoolean();
            DumpFrameExternals.Read(pReader, ref Restrictions);
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GetPlayRestrictions");
            pWriter.Indent++;
            pWriter.WriteLine("Reload: {0}", Reload);
            pWriter.WriteLine("Restrictions");
            pWriter.Indent++;
            DumpFrameExternals.Dump(pWriter, Restrictions);
            pWriter.Indent--;
            pWriter.Indent--;
        }
    }
}
