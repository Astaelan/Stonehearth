using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class SendFriendInviteFrame
    {
        public string Inviter = null;
        public string Invitee = null;
        public bool ByEmail = false;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Inviter);
            pWriter.Write(Invitee);
            pWriter.Write(ByEmail);
        }

        public SendFriendInviteFrame Read(BinaryReader pReader)
        {
            Inviter = pReader.ReadString();
            Invitee = pReader.ReadString();
            ByEmail = pReader.ReadBoolean();
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("SendFriendInvite");
            pWriter.Indent++;
            pWriter.WriteLine("Inviter: {0}", Inviter);
            pWriter.WriteLine("Invitee: {0}", Invitee);
            pWriter.WriteLine("ByEmail: {0}", ByEmail);
            pWriter.Indent--;
        }
    }
}
