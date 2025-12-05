
using System.Runtime.CompilerServices;

namespace Aoc25.Common
{
    /// <summary>
    /// Contains specialized math functions for solving very specific problems.
    /// </summary>
    public static class SpecializedMath
    {
        /// <summary>
        /// The maximum number of digits a ulong value can contain.
        /// </summary>
        const int MaxDigitsInUlong = 20;

        /// <summary>
        /// Lookup table for the <see cref="Pow10"/> function.
        /// </summary>
        private static readonly ulong[] pow10LookupTable = new ulong[MaxDigitsInUlong + 1]
        {
            1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 
            1000000000, 10000000000, 100000000000, 1000000000000, 10000000000000, 
            100000000000000, 1000000000000000, 10000000000000000, 100000000000000000,
            1000000000000000000, 10000000000000000000, 10000000000000000000
        };

        /// <summary>
        /// Computes 10 raised to the specified power.
        /// </summary>
        /// <remarks>
        /// If the resulting value would overflow the ulong datatype, 
        /// the maximum possible ulong value is returned.
        /// </remarks>
        /// <param name="power"> The power to raise 10 by. </param>
        /// <returns> The result of raising 10 to the specified power. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Pow10(uint power)
        {
            return (power <= MaxDigitsInUlong) ? 
                pow10LookupTable[power] : 
                ulong.MaxValue;
        }
    }
}