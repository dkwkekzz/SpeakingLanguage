using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Library
{
    public static class Extensions
    {
        public static void CopyTo<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> src, IDictionary<TKey, TValue> dst)
        {
            foreach (var pair in src)
                dst[pair.Key] = pair.Value;
        }

        public static bool HasAttribute(this Type classType, Type attrType) => classType.GetCustomAttributes(attrType, true).Length > 0;

        public static bool HasInterface(this Type classType, string iName) => classType.GetInterface(iName) != null;
    }
}
