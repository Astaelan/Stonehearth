using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon.DumpFrames
{
    public sealed class GetErrorsFrame
    {
        public BnetErrorInfo[] Errors = null;

        public void Write(BinaryWriter pWriter)
        {
            pWriter.Write(Errors.Length);
            Array.ForEach(Errors, e => DumpFrameExternals.Write(pWriter, e));
        }

        public GetErrorsFrame Read(BinaryReader pReader)
        {
            int errorsLength = pReader.ReadInt32();
            Errors = new BnetErrorInfo[errorsLength];
            for (int errorsIndex = 0; errorsIndex < errorsLength; ++errorsIndex)
            {
                Errors[errorsIndex] = new BnetErrorInfo(BnetFeature.Auth, BnetFeatureEvent.Auth_Logon, BattleNetErrors.ERROR_OK);
                DumpFrameExternals.Read(pReader, Errors[errorsIndex]);
            }
            return this;
        }

        public void Dump(IndentedStreamWriter pWriter)
        {
            pWriter.WriteLine("GetErrors");
            pWriter.Indent++;
            pWriter.WriteLine("Errors: {0}", Errors.Length);
            if (Errors.Length > 0)
            {
                pWriter.Indent++;
                foreach (BnetErrorInfo value in Errors)
                {
                    pWriter.WriteLine("Error");
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
