using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class RemoveFriendFrame
    {
        public BnetAccountId Account = null;

        public void Write(BinaryWriter pWriter)
        {
            DumpFrameExternals.Write(pWriter, Account);
        }

        public RemoveFriendFrame Read(BinaryReader pReader)
        {
            Account = new BnetAccountId();
            DumpFrameExternals.Read(pReader, Account);
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("RemoveFriend");
            pWriter.Indent++;
            pWriter.WriteLine("Account");
            pWriter.Indent++;
            DumpFrameExternals.Dump(pWriter, Account);
            pWriter.Indent--;
            pWriter.Indent--;
        }
    }
}
