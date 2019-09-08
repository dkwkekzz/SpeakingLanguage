using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage
{
    public static class Extensions
    {
        public static void CopyTo<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> src, IDictionary<TKey, TValue> dst)
        {
            foreach (var pair in src)
                dst[pair.Key] = pair.Value;
        }

        public static bool HasAttribute(this Type classType, Type attrType)
        {
            return classType.GetCustomAttributes(attrType, true).Length > 0;
        }

        public static bool HasInterface(this Type classType, string iName)
        {
            return classType.GetInterface(iName) != null;
        }

        public static List<T> ToList<T>(this T[] arr)
        {
            var list = new List<T>(arr.Length);
            for (int i = 0; i != arr.Length; i++)
                list.Add(arr[i]);
            return list;
        }

        public static bool ParseConfigOrDefault(this string appKey, bool defaultVal)
        {
            if (!bool.TryParse(ConfigurationManager.AppSettings[appKey], out bool val))
                return defaultVal;
            return val;
        }

        public static int ParseConfigOrDefault(this string appKey, int defaultVal)
        {
            if (!int.TryParse(ConfigurationManager.AppSettings[appKey], out int val))
                return defaultVal;
            return val;
        }

        public static int ParseConfig(this string appKey)
        {
            return int.Parse(ConfigurationManager.AppSettings[appKey]);
        }
    }
}
