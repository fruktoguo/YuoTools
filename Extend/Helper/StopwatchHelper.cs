using System.Diagnostics;

namespace YuoTools.Extend.Helper
{
    public class StopwatchHelper
    {
        private static readonly Stopwatch stopwatch = new();

        public static void Start()
        {
            stopwatch.Restart();
        }

        public static double Stop()
        {
            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds;
        }
    }
}