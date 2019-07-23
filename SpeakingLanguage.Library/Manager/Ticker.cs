using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SpeakingLanguage.Library
{
    public class Ticker
    {
        private readonly static Stopwatch _timer = Stopwatch.StartNew();
        private readonly static long _startMS;

        static Ticker()
        {
            var elapsed = DateTime.Now - new DateTime();
            _startMS = (long)elapsed.TotalMilliseconds;
        }

        public static long StartMS => _startMS;
        public static long UniversalMS => _startMS + (long)Span.TotalMilliseconds;
        public static long Tick => _timer.ElapsedTicks;
        public static long MS => _timer.ElapsedMilliseconds;
        public static TimeSpan Span => _timer.Elapsed;
    }
}
