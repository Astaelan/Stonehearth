using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StonehearthCommon
{
    public enum LogManagerLevel
    {
        Info,
        Warn,
        Error,
        Exception,
        Debug
    }
    public static class LogManager
    {
        public delegate void LogHandler(LogManagerLevel pLogLevel, string pOutput);

        public static event LogHandler OnOutput;

        public static void WriteLine(LogManagerLevel pLogLevel, string pFormat, params object[] pArguments) { if (OnOutput != null) OnOutput(pLogLevel, DateTime.Now.ToString() + " <" + pLogLevel.ToSafeString() + "> " + string.Format(pFormat, pArguments)); }
    }
}
