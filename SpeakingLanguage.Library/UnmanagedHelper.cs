using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Library
{
    public static class UnmanagedHelper
    {
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        private static unsafe extern void _rtlMoveMemory(void* dest, void* src, int size);

        public static unsafe void MoveMemory<T>(void* dest, void* src) where T : unmanaged
        {
            var sz = sizeof(T);
            _rtlMoveMemory(dest, src, sz);
        }

        public static unsafe void MoveMemory(void* dest, void* src, int size)
        {
            _rtlMoveMemory(dest, src, size);
        }

        public static unsafe void Memset(void* ptr, byte value, int count)
        {
            byte* pCur = (byte*)ptr;
            for (int i = 0; i < count; ++i)
            {
                *pCur++ = value;
            }
        }

        public static unsafe void Memset(void* ptr, byte value, long count)
        {
            byte* pCur = (byte*)ptr;
            for (int i = 0; i < count; ++i)
            {
                *pCur++ = value;
            }
        }
    }
}
