using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SpeakingLanguage.Library
{
    public static class BitConverter
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct ConverterHelperDouble
        {
            [FieldOffset(0)]
            public ulong Along;

            [FieldOffset(0)]
            public double Adouble;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ConverterHelperFloat
        {
            [FieldOffset(0)]
            public int Aint;

            [FieldOffset(0)]
            public float Afloat;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteLittleEndian(byte[] buffer, int offset, ulong data)
        {
#if BIGENDIAN
            buffer[offset + 7] = (byte)(data);
            buffer[offset + 6] = (byte)(data >> 8);
            buffer[offset + 5] = (byte)(data >> 16);
            buffer[offset + 4] = (byte)(data >> 24);
            buffer[offset + 3] = (byte)(data >> 32);
            buffer[offset + 2] = (byte)(data >> 40);
            buffer[offset + 1] = (byte)(data >> 48);
            buffer[offset    ] = (byte)(data >> 56);
#else
            buffer[offset] = (byte)(data);
            buffer[offset + 1] = (byte)(data >> 8);
            buffer[offset + 2] = (byte)(data >> 16);
            buffer[offset + 3] = (byte)(data >> 24);
            buffer[offset + 4] = (byte)(data >> 32);
            buffer[offset + 5] = (byte)(data >> 40);
            buffer[offset + 6] = (byte)(data >> 48);
            buffer[offset + 7] = (byte)(data >> 56);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteLittleEndian(byte[] buffer, int offset, int data)
        {
#if BIGENDIAN
            buffer[offset + 3] = (byte)(data);
            buffer[offset + 2] = (byte)(data >> 8);
            buffer[offset + 1] = (byte)(data >> 16);
            buffer[offset    ] = (byte)(data >> 24);
#else
            buffer[offset] = (byte)(data);
            buffer[offset + 1] = (byte)(data >> 8);
            buffer[offset + 2] = (byte)(data >> 16);
            buffer[offset + 3] = (byte)(data >> 24);
#endif
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteLittleEndian(byte[] buffer, int offset, short data)
        {
#if BIGENDIAN
            buffer[offset + 1] = (byte)(data);
            buffer[offset    ] = (byte)(data >> 8);
#else
            buffer[offset] = (byte)(data);
            buffer[offset + 1] = (byte)(data >> 8);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadLittleEndian(byte[] buffer, int offset, out long value)
        {
#if BIGENDIAN
            value = buffer[offset + 0];
            value |= (long)buffer[offset + 1] << 8;
            value |= (long)buffer[offset + 2] << 16;
            value |= (long)buffer[offset + 3] << 24;
            value |= (long)buffer[offset + 4] << 32;
            value |= (long)buffer[offset + 5] << 40;
            value |= (long)buffer[offset + 6] << 48;
            value |= (long)buffer[offset + 7] << 56;
#else
            value = buffer[offset + 7];
            value |= (long)buffer[offset + 6] << 8;
            value |= (long)buffer[offset + 5] << 16;
            value |= (long)buffer[offset + 4] << 24;
            value |= (long)buffer[offset + 3] << 32;
            value |= (long)buffer[offset + 2] << 40;
            value |= (long)buffer[offset + 1] << 48;
            value |= (long)buffer[offset + 0] << 56;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadLittleEndian(byte[] buffer, int offset, out int value)
        {
#if BIGENDIAN
            value = buffer[offset + 0];
            value |= buffer[offset + 1] << 8;
            value |= buffer[offset + 2] << 16;
            value |= buffer[offset + 3] << 24;
#else
            value = buffer[offset + 3];
            value |= buffer[offset + 2] << 8;
            value |= buffer[offset + 1] << 16;
            value |= buffer[offset + 0] << 24;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReadLittleEndian(byte[] buffer, int offset, out short value)
        {
#if BIGENDIAN
            int tempVal = buffer[offset + 0];
            tempVal |= buffer[offset + 1] << 8;
            value = (short)tempVal;
#else
            int tempVal = buffer[offset + 1];
            tempVal |= buffer[offset + 0] << 8;
            value = (short)tempVal;
#endif
        }

        public static void GetBytes(byte[] bytes, ref int offset, double value)
        {
            ConverterHelperDouble ch = new ConverterHelperDouble { Adouble = value };
            WriteLittleEndian(bytes, offset, ch.Along);
            offset += sizeof(double);
        }

        public static void GetBytes(byte[] bytes, ref int offset, float value)
        {
            ConverterHelperFloat ch = new ConverterHelperFloat { Afloat = value };
            WriteLittleEndian(bytes, offset, ch.Aint);
            offset += sizeof(float);
        }

        public static void GetBytes(byte[] bytes, ref int offset, int value)
        {
            WriteLittleEndian(bytes, offset, value);
            offset += sizeof(int);
        }

        public static void GetBytes(byte[] bytes, ref int offset, long value)
        {
            WriteLittleEndian(bytes, offset, (ulong)value);
            offset += sizeof(long);
        }

        public static void GetBytes(byte[] bytes, ref int offset, bool value)
        {
            bytes[offset + 0] = value == true ? (byte)1 : (byte)0;
            offset += 1;
        }

        public static void GetBytes(byte[] bytes, ref int offset, string value)
        {
            int length = value.Length;
            GetBytes(bytes, ref offset, length);

            for (int i = 0; i < length; i++)
                bytes[offset + 4 + i] = (byte)value[i];
            offset += sizeof(byte) * length;
        }

        public static int ToInt(byte[] bytes, ref int offset)
        {
            ReadLittleEndian(bytes, offset, out int value);
            offset += sizeof(int);
            return value;
        }

        public static long ToLong(byte[] bytes, ref int offset)
        {
            ReadLittleEndian(bytes, offset, out long value);
            offset += sizeof(long);
            return value;
        }

        public static float ToSingle(byte[] bytes, ref int offset)
        {
            ReadLittleEndian(bytes, offset, out int value);
            offset += sizeof(float);

            ConverterHelperFloat ch = new ConverterHelperFloat { Aint = value };
            return ch.Afloat;
        }

        public static bool ToBoolean(byte[] bytes, ref int offset)
        {
            var value = bytes[offset] == 1 ? true : false;
            offset++;
            return value;
        }

        public static string ToString(byte[] bytes, ref int offset)
        {
            var length = ToInt(bytes, ref offset);
            if (length == 0) return null;

            var builder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                builder.Append((char)bytes[offset + i]);
            offset += sizeof(byte) * length;

            return builder.ToString();
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
