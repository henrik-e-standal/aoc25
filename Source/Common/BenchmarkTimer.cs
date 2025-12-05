
using System.Diagnostics;

namespace Aoc25.Common
{
    /// <summary>
    /// Helper class used to measure execution time.
    /// </summary>
    public static class BenchmarkTimer
    {
#if DEBUG
        /// <summary>
        /// Stopwatch used to measure code execution time. 
        /// </summary>
        private static readonly Stopwatch stopwatch = new Stopwatch();
#endif

        /// <summary>
        /// Starts a new measurement of execution time.
        /// </summary>
        [Conditional("DEBUG")]
        public static void Tick()
        { 
            stopwatch.Restart();    
        }

        /// <summary>
        /// Stops a measurement of execution time and prints the total execution time.
        /// </summary>
        [Conditional("DEBUG")]
        public static void Tock()
        {
            stopwatch.Stop();
            Console.WriteLine($"Time elapsed: {(stopwatch.ElapsedTicks * 1000.0) / Stopwatch.Frequency}ms.");
        }   
    }
}