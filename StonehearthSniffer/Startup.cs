using System;
using System.IO;
using System.Reflection;

namespace StonehearthSniffer
{
    public static class Startup
    {
        public static void Main()
        {
            BattleNet.Log.LogDebug(string.Empty); // Required to ensure static constructor is called before replacing the static field
            typeof(BattleNet).GetField("s_impl", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, new Proxy());
        }
    }
}
