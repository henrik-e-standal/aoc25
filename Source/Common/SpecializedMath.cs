
using System.Diagnostics;

namespace Aoc25.Common
{
    /// <summary>
    /// Contains specialized
    /// </summary>
    public static class SpecializedMath
    {
        const int MaxDigitsInUlong = 20;

        private static readonly ulong[] pow10LookupTable =
        [
            1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 
            1000000000, 10000000000, 100000000000, 1000000000000, 10000000000000, 
            100000000000000, 1000000000000000, 10000000000000000, 100000000000000000,
            1000000000000000000, 10000000000000000000
        ];

        public static ulong Pow10(uint uintDigits)
        {
            return (uintDigits < MaxDigitsInUlong) ? 
                pow10LookupTable[uintDigits] : 
                0U;
        }
    }
}