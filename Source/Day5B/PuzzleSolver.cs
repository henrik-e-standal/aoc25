
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day5B
{
    /// <summary>
    /// Contains the puzzle solving code.
    /// </summary>
    public static class PuzzleSolver
    { 
        /// <summary>
        /// The maximum number of number ranges this puzzle solver can solve for.
        /// </summary>
        private const uint MaxFreshIngredientRanges = 1024;
        
        /// <summary>
        /// The maximum number of ingredients this puzzle solver can solve for.
        /// </summary>
        private const uint MaxIngredients = 1024;

        /// <summary>
        /// Represents a list of fresh ingredient IDs.
        /// </summary>
        [DebuggerDisplay("{FirstNumber}-{LastNumber}")]
        private struct NumberRange
        {
            /// <summary>
            /// The first ingredient ID in the range.
            /// </summary>
            public ulong FirstNumber;

            /// <summary>
            /// The last ingredient ID in the range.
            /// </summary>
            public ulong LastNumber;
        }

        /// <summary>
        /// A comparer that defines a sorting order for <see cref="NumberRange"/>.
        /// It will sort according to the value of the first number in the ranges.
        /// </summary>
        private class NumberRangeComparer : IComparer<NumberRange>
        {
            public int Compare(NumberRange x, NumberRange y)
            {
                if(x.FirstNumber > y.FirstNumber) return 1;
                if(x.FirstNumber < y.FirstNumber) return -1;
                return 0;
            }
        }

        /// <summary>
        /// Attempts to get the numeric number of a character.
        /// </summary>
        /// <param name="character"> The character whose numeric value to get. </param>
        /// <param name="numericValue"> The numeric value of the character. </param>
        /// <returns> True if the character was a number, otherwise false. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetCharacterNumericValue(char character, out uint numericValue)
        {
            numericValue = (uint)(character - '0');
            return (numericValue <= 9);
        }

        /// <summary>
        /// Parse all the fresh ingredient ID number ranges in the puzzle input. 
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input whose number ranges to parse. </param>
        /// <returns> 
        /// All the fresh ingredient ID number ranges, the number of ranges found, and the index 
        /// where the first ingredient is stored in the puzzle input.
        ///  </returns>
        private static (NumberRange[] freshIngredientRanges, int freshIngredientRangeCount) 
            ParseFreshIngredientRanges(string puzzleInput)
        {
            var numberRanges = new NumberRange[MaxFreshIngredientRanges];

            int numberRangeOffset = 0;
            int i;

            for(i = 0; i < puzzleInput.Length;)
            {
                ulong firstRangeNumber = 0;
                ulong lastRangeNumber = 0;

                while(TryGetCharacterNumericValue(puzzleInput[i++], out uint number)) {
                    firstRangeNumber = (firstRangeNumber * 10) + number;
                } 
                while(TryGetCharacterNumericValue(puzzleInput[i++], out uint number)) {
                    lastRangeNumber = (lastRangeNumber * 10) + number;
                }

                numberRanges[numberRangeOffset].FirstNumber = firstRangeNumber;
                numberRanges[numberRangeOffset].LastNumber = lastRangeNumber;
                numberRangeOffset++;

                if(puzzleInput[i] == '\n')
                {
                    break;
                }
            }

            return (numberRanges, numberRangeOffset);
        }

        /// <summary>
        /// Count the number of fresh ingredients.
        /// </summary>
        /// <param name="freshIngredientRanges"> The ranges that specify fresh ingredient IDs. </param>
        /// <param name="freshIngredientRangeCount"> The number of ranges that specify fresh ingredient IDs.</param>
        /// <returns> The number of fresh ingredients. </returns>
        private static ulong CountFreshIngredientIds(NumberRange[] freshIngredientRanges, int freshIngredientRangeCount)
        {
            ulong freshIngredientCount = 0;

            // Sort the list of fresh ingredient ID ranges by the first number in each range. 
            Array.Sort(freshIngredientRanges, 0, freshIngredientRangeCount, new NumberRangeComparer());

            ulong highestVisitedId = 0;

            for(int i = 0; i < freshIngredientRangeCount; i++)
            {
                var numberRange = freshIngredientRanges[i];

                if(numberRange.FirstNumber > highestVisitedId)
                {
                    freshIngredientCount += (numberRange.LastNumber - numberRange.FirstNumber) + 1;
                    highestVisitedId = numberRange.LastNumber;
                }
                else if(numberRange.LastNumber > highestVisitedId)
                {
                    freshIngredientCount += (numberRange.LastNumber - Math.Max(highestVisitedId, numberRange.FirstNumber));
                    highestVisitedId = numberRange.LastNumber;
                }
            }

            return freshIngredientCount;
        }

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            var (freshIngredientRanges, freshIngredientRangCount) = ParseFreshIngredientRanges(
                puzzleInput);

            return CountFreshIngredientIds(
                freshIngredientRanges, 
                freshIngredientRangCount);
        }
    }
}