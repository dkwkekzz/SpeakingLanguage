using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal static class Extensions
    {
        public static List<int> ToKeyList(this Type[] arr)
        {
            var list = new List<int>(arr.Length);
            for (int i = 0; i != arr.Length; i++)
            {
                var typeKey = TypeManager.GetStateHandle(arr[i]).key;
                list.Add(typeKey);
            }

            return list;
        }
    }
}
