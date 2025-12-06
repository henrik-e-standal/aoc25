
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

        /// <summary>
        /// Stores the last execution time measured.
        /// </summary>
        private static double lastElapsedMilliseconds = 0.0;
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
            lastElapsedMilliseconds = ((stopwatch.ElapsedTicks * 1000.0) / Stopwatch.Frequency);
        }   

 #if DEBUG
        /// <summary>
        /// Gets the last measured execution time, in milliseconds (with decimals).
        /// </summary>
        /// <returns> The last measured execution time, in milliseconds. </returns>
        public static double GetElapsedMilliseconds()
        {
            return lastElapsedMilliseconds;
        }
#endif
        /// <summary>
        /// Prints the last measured execution time, in milliseconds (with decimals), to console.
        /// </summary>
        [Conditional("DEBUG")]
        public static void PrintElapsedMilliseconds()
        {
            FastConsole.WriteLine($"Elapsed: {lastElapsedMilliseconds}ms.");
        }

    }
}