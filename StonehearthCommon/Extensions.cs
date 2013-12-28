using PegasusShared;
using PegasusUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StonehearthCommon
{
    public static class Extensions
    {
        public static int ToUnixTimestamp(this DateTime pThis) { return (int)(pThis - new DateTime(1970, 1, 1)).TotalSeconds; }

        public static void ForEach<T>(this IEnumerable<T> pThis, Action<T> pCallback) { foreach (T value in pThis) pCallback(value); }

        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> pThis, TKey pKey)
        {
            TValue value = default(TValue);
            pThis.TryGetValue(pKey, out value);
            return value;
        }

        public static Date.Builder FromDateTime(this Date.Builder pThis, DateTime pValue)
        {
            pThis.SetYear(pValue.Year);
            pThis.SetMonth(pValue.Month);
            pThis.SetDay(pValue.Day);
            pThis.SetHours(pValue.Hour);
            pThis.SetMin(pValue.Minute);
            pThis.SetSec(pValue.Second);
            return pThis;
        }

        public static string ToSafeString(this Enum pThis)
        {
            return Enum.GetName(pThis.GetType(), pThis);
            //string[] names = Enum.GetNames(pThis.GetType());
            //Array values = Enum.GetValues(pThis.GetType());
            //return names[Array.IndexOf(values, pThis)];
        }
    }
}
