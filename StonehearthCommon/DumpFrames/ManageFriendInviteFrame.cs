using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class ManageFriendInviteFrame
    {
        public int Action = 0;
        public ulong InviteID = 0;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Action);
            pWriter.Write(InviteID);
        }

        public ManageFriendInviteFrame Read(BinaryReader pReader)
        {
            Action = pReader.ReadInt32();
            InviteID = pReader.ReadUInt64();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("ManageFriendInvite");
            pWriter.Indent++;
            pWriter.WriteLine("Action: {0}", Action);
            pWriter.WriteLine("InviteID: {0}", InviteID);
            pWriter.Indent--;
        }
    }
}
