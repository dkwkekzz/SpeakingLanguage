using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Library
{
    public static class Utils
    {
        public static TimeSpan MeasureTime(Action action, int loop = 1)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i != loop; i++)
            {
                action();
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}
