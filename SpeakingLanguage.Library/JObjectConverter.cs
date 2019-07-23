using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SpeakingLanguage.Library
{
    public static class JObjectConverter
    {
        /// <summary>
        /// jobject byte compact
        /// 1. size(jobject and jarray)
        /// 2. key(only jobject)
        /// 3. type
        /// 4. value
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="jObject"></param>
        public static void GetBytes(byte[] buffer, ref int offset, JObject jObject)
        {
            BitConverter.GetBytes(buffer, ref offset, jObject.Count);
            foreach (var pair in jObject)
            {
                BitConverter.GetBytes(buffer, ref offset, pair.Key);
                JObjectConverter.GetBytes(buffer, ref offset, pair.Value);
            }
        }

        public static void GetBytes(byte[] buffer, ref int offset, JArray jArray)
        {
            BitConverter.GetBytes(buffer, ref offset, jArray.Count);
            foreach (var token in jArray)
            {
                JObjectConverter.GetBytes(buffer, ref offset, token);
            }
        }

        public static void GetBytes(byte[] buffer, ref int offset, JToken token)
        {
            BitConverter.GetBytes(buffer, ref offset, (int)token.Type);
            switch (token.Type)
            {
                case JTokenType.Object:
                    JObjectConverter.GetBytes(buffer, ref offset, (JObject)token);
                    break;
                case JTokenType.Array:
                    JObjectConverter.GetBytes(buffer, ref offset, (JArray)token);
                    break;
                case JTokenType.Integer:
                    BitConverter.GetBytes(buffer, ref offset, (int)token);
                    break;
                case JTokenType.Boolean:
                    BitConverter.GetBytes(buffer, ref offset, (bool)token);
                    break;
                case JTokenType.Float:
                    BitConverter.GetBytes(buffer, ref offset, (float)token);
                    break;
                case JTokenType.String:
                    BitConverter.GetBytes(buffer, ref offset, (string)token);
                    break;
                default:
                    throw new NotSupportedException($"can't match jobject type: {token.ToString()}");
            }
        }
    }
}
