using System;
using System.Collections.Generic;
using System.Text;

namespace SpeakingLanguage.Library
{
    public static class BitConverter
    {
        public static void GetBytes(byte[] buffer, ref int offset, int value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value);
            offset += sizeof(int);
        }

        public static void GetBytes(byte[] buffer, ref int offset, long value)
        {
            buffer[offset + 0] = (byte)(value >> 56);
            buffer[offset + 1] = (byte)(value >> 48);
            buffer[offset + 2] = (byte)(value >> 40);
            buffer[offset + 3] = (byte)(value >> 32);
            buffer[offset + 4] = (byte)(value >> 24);
            buffer[offset + 5] = (byte)(value >> 16);
            buffer[offset + 6] = (byte)(value >> 8);
            buffer[offset + 7] = (byte)(value);
            offset += sizeof(long);
        }

        public static void GetBytes(byte[] buffer, ref int offset, float value)
        {
            var bytes = System.BitConverter.GetBytes(value);
            for (int i = 0; i < bytes.Length; i++)
                buffer[offset + i] = bytes[i];
            offset += bytes.Length;
        }

        public static void GetBytes(byte[] buffer, ref int offset, bool value)
        {
            buffer[offset + 0] = value == true ? (byte)1 : (byte)0;
            offset += 1;
        }

        public static void GetBytes(byte[] buffer, ref int offset, string value)
        {
            var length = value.Length;
            buffer[offset + 0] = (byte)(length >> 24);
            buffer[offset + 1] = (byte)(length >> 16);
            buffer[offset + 2] = (byte)(length >> 8);
            buffer[offset + 3] = (byte)(length);

            for (int i = 0; i < length; i++)
                buffer[offset + 4 + i] = (byte)value[i];

            offset += (sizeof(int) + sizeof(byte) * length);
        }

        public static void GetBytes(byte[] buffer, ref int offset, IList<int> value)
        {
            GetBytes(buffer, ref offset, value.Count);
            for (int i = 0; i != value.Count; i++)
            {
                GetBytes(buffer, ref offset, value[i]);
            }
        }

        public static void GetBytes(byte[] buffer, ref int offset, IDictionary<int, int> value)
        {
            GetBytes(buffer, ref offset, value.Count);
            foreach (var pair in value)
            {
                GetBytes(buffer, ref offset, pair.Key);
                GetBytes(buffer, ref offset, pair.Value);
            }
        }

        public static void GetBytes(byte[] buffer, ref int offset, IDictionary<string, int> value)
        {
            GetBytes(buffer, ref offset, value.Count);
            foreach (var pair in value)
            {
                GetBytes(buffer, ref offset, pair.Key);
                GetBytes(buffer, ref offset, pair.Value);
            }
        }

        public static int ToInt(byte[] buffer, ref int offset)
        {
            int value = buffer[offset + 3];
            value |= buffer[offset + 2] << 8;
            value |= buffer[offset + 1] << 16;
            value |= buffer[offset + 0] << 24;
            offset += sizeof(int);

            return value;
        }

        public static long ToLong(byte[] buffer, ref int offset)
        {
            long value = buffer[offset + 7];
            value |= (long)buffer[offset + 6] << 8;
            value |= (long)buffer[offset + 5] << 16;
            value |= (long)buffer[offset + 4] << 24;
            value |= (long)buffer[offset + 3] << 32;
            value |= (long)buffer[offset + 2] << 40;
            value |= (long)buffer[offset + 1] << 48;
            value |= (long)buffer[offset + 0] << 56;
            offset += sizeof(long);

            return value;
        }

        public static float ToSingle(byte[] buffer, ref int offset)
        {
            var value = System.BitConverter.ToSingle(buffer, offset);
            offset += sizeof(float);

            return value;
        }

        public static bool ToBoolean(byte[] buffer, ref int offset)
        {
            var value = buffer[offset] == 1 ? true : false;
            offset++;

            return value;
        }

        public static string ToString(byte[] buffer, ref int offset, int length)
        {
            var builder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                builder.Append((char)buffer[offset + i]);
            var value = string.Intern(builder.ToString());

            offset += sizeof(byte) * length;

            return value;
        }

        public static IList<int> ToList(byte[] buffer, ref int offset)
        {
            var count = ToInt(buffer, ref offset);
            var ret = new List<int>(count);
            for (int i = 0; i != count; i++)
            {
                ret.Add(ToInt(buffer, ref offset));
            }

            return ret;
        }

        public static IDictionary<int, int> ToDictionary(byte[] buffer, ref int offset)
        {
            var count = ToInt(buffer, ref offset);
            var ret = new Dictionary<int, int>(count);
            for (int i = 0; i != count; i++)
            {
                ret.Add(ToInt(buffer, ref offset), ToInt(buffer, ref offset));
            }

            return ret;
        }

        public static void PassInt(byte[] buffer, ref int offset)
        {
            offset += sizeof(int);
        }

        public static void PassString(byte[] buffer, ref int offset)
        {
            int length = buffer[offset + 3];
            length |= buffer[offset + 2] << 8;
            length |= buffer[offset + 1] << 16;
            length |= buffer[offset + 0] << 24;
            
            offset += (sizeof(int) + sizeof(byte) * length);
            offset += sizeof(int);
        }
    }
}
