using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    public class KeyGenerator
    {
        private static int _handleNum;
        private static int _keyNum;
        public int NewHandle => _handleNum++;
        public long NewUniqueKey => (Library.Ticker.GlobalTicks << 4) + (_keyNum & 0xF);
    }

    public class WriteHolder
    {
        private byte[] _buffer = new byte[1 << 10];

        public Library.Writer NewWriter => new Library.Writer(_buffer, 0);
    }
}
