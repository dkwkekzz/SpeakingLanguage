using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SpeakingLanguage.Library
{
    public class Ticker
    {
        private readonly static Stopwatch _timer = Stopwatch.StartNew();
        private readonly static long _startTicks;

        static Ticker()
        {
            var centuryBegin = new DateTime(2001, 1, 1);
            var currentDate = DateTime.Now;
            _startTicks = currentDate.Ticks - centuryBegin.Ticks;
        }

        public static long StartTicks => _startTicks;
        public static long GlobalTicks => _startTicks + _timer.Elapsed.Ticks;
        public static long ElapsedTicks => _timer.ElapsedTicks;
        public static long ElapsedMS => _timer.ElapsedMilliseconds;
        public static TimeSpan Span => _timer.Elapsed;
    }
}
