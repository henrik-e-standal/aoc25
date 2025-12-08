
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day5A
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
        private static (NumberRange[] freshIngredientRanges, int freshIngredientRangeCount, int firstIngredientIndex) 
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

            return (numberRanges, numberRangeOffset, (i + 1));
        }

        /// <summary>
        /// Parse all the ingredient IDs in the puzzle input. 
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input whose ingredient IDs to parse. </param>
        /// <param name="firstIngredientIndex"> The index where the first ingredient is stored in the puzzle input. </param>
        /// <returns>  All the ingredient IDs, the number of IDs found, in the puzzle input. </returns>
        private static (ulong[] ingredients, int ingredientCount) ParseIngredients(
            string puzzleInput, 
            int firstIngredientIndex)
        {
            var ingredients = new ulong[MaxIngredients];

            int ingredientOffset = 0;

            for(int i = firstIngredientIndex; i < puzzleInput.Length;)
            {
                ulong ingredientNumber = 0;

                while(TryGetCharacterNumericValue(puzzleInput[i++], out uint number)) {
                    ingredientNumber = (ingredientNumber * 10) + number;
                } 

                ingredients[ingredientOffset] = ingredientNumber;
                ingredientOffset++;
            }

            return (ingredients, ingredientOffset);
        }

        /// <summary>
        /// Count the number of fresh ingredients.
        /// </summary>
        /// <param name="freshIngredientRanges"> The ranges that specify fresh ingredient IDs. </param>
        /// <param name="freshIngredientRangeCount"> The number of ranges that specify fresh ingredient IDs.</param>
        /// <param name="ingredients"> The ingredient IDs to check for freshness. </param>
        /// <param name="ingredientCount"> The number of ingredient IDs to check. </param>
        /// <returns> The number of fresh ingredients. </returns>
        private static ulong CountFreshIngredients(
            NumberRange[] freshIngredientRanges,
            int freshIngredientRangeCount,
            ulong[] ingredients,
            int ingredientCount)
        {
            uint freshIngredientCount = 0;

            for(int i = 0; i < ingredientCount; i++)
            {
                ulong currentIngredient = ingredients[i];

                for(int j = 0; j < freshIngredientRangeCount; j++)
                {
                    if(currentIngredient >= freshIngredientRanges[j].FirstNumber &&
                       currentIngredient <= freshIngredientRanges[j].LastNumber) {
                        freshIngredientCount++;
                        break;
                    }
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
            var (freshIngredientRanges, freshIngredientRangCount, firstIngredientIndex) = ParseFreshIngredientRanges(
                puzzleInput);

            var (ingredients, ingredientCount) = ParseIngredients(
                puzzleInput, 
                firstIngredientIndex);

            return CountFreshIngredients(
                freshIngredientRanges, 
                freshIngredientRangCount,
                ingredients, 
                ingredientCount);
        }
    }
}