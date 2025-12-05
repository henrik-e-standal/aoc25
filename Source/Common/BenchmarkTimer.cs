
using System.Diagnostics;

namespace Aoc25.Common
{
    /// <summary>
    /// Helper class used measure execution time.
    /// </summary>
    public static class BenchmarkTimer
    {
#if DEBUG
        private static readonly Stopwatch stopwatch = new Stopwatch();
#endif
        /// <summary>
        /// Starts timing.
        /// </summary>
        public static void Tick()
        {
#if DEBUG   
            stopwatch.Restart();
#endif      
        }

        /// <summary>
        /// Stops timing and prints the time elapsed since timing was started.
        /// </summary>
        public static void Tock()
        {
#if DEBUG   
            if (!stopwatch.IsRunning)
            {
                throw new InvalidOperationException($"The '{nameof(Tick)}' function should be called before calling '{nameof(Tock)}'");
            }

            stopwatch.Stop();
            Console.WriteLine($"Time elapsed: {(stopwatch.ElapsedTicks * 1000.0) / Stopwatch.Frequency}ms.");
#endif    
        }   
    }
}