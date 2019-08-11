using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public static class UnmanagedHelper
    {
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
