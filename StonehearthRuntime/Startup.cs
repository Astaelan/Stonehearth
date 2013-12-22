using System;
using System.Reflection;

namespace StonehearthRuntime
{
    public static class Startup
    {
        public static void Main()
        {
            BattleNet.Log.LogDebug(string.Empty); // Required to ensure static constructor is called before replacing the static field
            typeof(BattleNet).GetField("s_impl", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, new Connector());
            
        }
    }
}
