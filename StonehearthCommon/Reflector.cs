using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StonehearthCommon
{
    public static class Reflector
    {
        public class Tuple<T1, T2>
        {
            public T1 Item1;
            public T2 Item2;
            public Tuple(T1 item1, T2 item2)
            {
                Item1 = item1;
                Item2 = item2;
            }
        }
        public static List<Tuple<T1, T2>> FindAllMethods<T1, T2>(Assembly pSpecificAssembly)
            where T1 : Attribute
            where T2 : class
        {
            if (!typeof(T2).IsSubclassOf(typeof(Delegate))) return null;
            List<Tuple<T1, T2>> results = new List<Tuple<T1, T2>>();
            Assembly[] assemblies = new Assembly[] { pSpecificAssembly };
            if (pSpecificAssembly == null) assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.GlobalAssemblyCache) continue;
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                    foreach (MethodInfo method in methods)
                    {
                        Attribute[] attributes = Attribute.GetCustomAttributes(method, typeof(T1), false);
                        if (attributes == null) continue;
                        foreach (Attribute attribute in attributes)
                        {
                            T2 callback = Delegate.CreateDelegate(typeof(T2), method, false) as T2;
                            if (callback == null) continue;
                            results.Add(new Tuple<T1, T2>((T1)attribute, callback));
                        }
                    }
                }
            }
            return results;
        }
    }
}
