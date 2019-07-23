using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public static class Math
    {
        // Find the largest non-negative integer x such that pow(2,x) <= n.
        // The exception is n=0, which returns 0.
        public static int Log2le(int n)
        {
            return (int)Log2le((uint)n);
        }

        public static uint Log2le(uint n)
        {
            uint t, log2;

            if (n >= 0x10000)
            {
                log2 = 16;
                t = 0x1000000;
            }
            else
            {
                log2 = 0;
                t = 0x100;
            }

            if (n >= t)
            {
                log2 += 8;
                t <<= 4;
            }
            else
            {
                t >>= 4;
            }

            if (n >= t)
            {
                log2 += 4;
                t <<= 2;
            }
            else
            {
                t >>= 2;
            }

            if (n >= t)
            {
                log2 += 2;
                t <<= 1;
            }
            else
            {
                t >>= 1;
            }

            if (n >= t)
            {
                log2 += 1;
            }

            return log2;
        }

        // Find the smallest non-negative integer x such that pow(2,x) >= n.
        public static int Log2ge(int n)
        {
            return (int)Log2ge((uint)n);
        }

        public static uint Log2ge(uint n)
        {
            if (n > 0x80000000)
                return 32;

            uint t, log2;

            if (n > 0x8000)
            {
                log2 = 16;
                t = 0x800000;
            }
            else
            {
                log2 = 0;
                t = 0x80;
            }

            if (n > t)
            {
                log2 += 8;
                t <<= 4;
            }
            else
            {
                t >>= 4;
            }

            if (n > t)
            {
                log2 += 4;
                t <<= 2;
            }
            else
            {
                t >>= 2;
            }

            if (n > t)
            {
                log2 += 2;
                t <<= 1;
            }
            else
            {
                t >>= 1;
            }

            if (n > t)
            {
                log2 += 1;
            }

            return log2;
        }
    }
}
