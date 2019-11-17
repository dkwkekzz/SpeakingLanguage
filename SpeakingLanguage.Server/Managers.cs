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

}
