using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StonehearthCommon
{
    public sealed class IndentedStreamWriter
    {
        private Stream mStream = null;
        private StreamWriter mWriter = null;
        private int mIndent = 0;

        public IndentedStreamWriter(string pPath)
        {
            mStream = new FileStream(pPath, FileMode.Create, FileAccess.Write);
            mWriter = new StreamWriter(mStream, Encoding.UTF8) { AutoFlush = true };
        }

        public Stream Stream { get { return mStream; } }

        public int Indent { get { return mIndent; } set { mIndent = value; } }

        public void Close() { mWriter.Close(); }

        public void WriteLine()
        {
            mWriter.WriteLine(new string('\t', mIndent));
        }
        public void WriteLine(string pFormat, params object[] pArgs)
        {
            //mWriter.WriteLine(new string('\t', mIndent) + ((pFormat == "}" || pFormat == "{") ? pFormat : string.Format(pFormat, pArgs)));
            mWriter.WriteLine(new string('\t', mIndent) + pFormat, pArgs);
        }
    }
}
