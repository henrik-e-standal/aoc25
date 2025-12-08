
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day2A
{
    /// <summary>
    /// Contains the puzzle solving code.
    /// </summary>
    public static class PuzzleSolver
    {
        [InlineArray(16)]
        public struct FixedArray16<T>
        {
            private T element0;
        }

        /// <summary>
        /// 
        /// </summary>
        private struct IdRange
        {
            /// <summary>
            /// 
            /// </summary>
            public ulong FirstValue;

            /// <summary>
            /// 
            /// </summary>
            public ulong LastValue;

            /// <summary>
            /// A fixed-size array that stores the digits in first value belonging to the range to scan. 
            /// </summary>
            public FixedArray16<byte> FirstValueDigits;

            /// <summary>
            /// The number of digits in values belonging to this range.
            /// </summary>
            public uint DigitCount;
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

        private static bool CharacterIsNumericValue(char character)
        {
            return (character >= '0') && (character <= '9');
        }

        private static FastList<IdRange> GetNumberRangesToScan(string puzzleInput)
        {
            var idRangeScanJobs = new FastList<IdRange>(1024);

            for(int i = 0; i < puzzleInput.Length;)
            {
                /*if (!CharacterIsNumericValue(puzzleInput[i])) {
                    continue;
                }*/
                
                ulong rangeStartValue = 0;
                uint rangeStartDigitCount = 0;
                ulong rangeEndValue = 0;
                uint rangeEndDigitCount = 0;

                // Determine the value of, and the number of digits in, the first ID in the current ID range.
                while(TryGetCharacterNumericValue(puzzleInput[i++], out uint numericValue))
                {
                    rangeStartValue = (rangeStartValue * 10) + numericValue;
                    rangeStartDigitCount++;
                }

                // Determine the value of, and the number of digits in, the last ID in the current ID range.
                while(TryGetCharacterNumericValue(puzzleInput[i++], out uint numericValue))
                {
                    rangeEndValue = (rangeEndValue * 10) + numericValue;
                    rangeEndDigitCount++;
                }

                ulong rangeFirstValue = 0;
                ulong rangeLastValue = (rangeStartValue - 1);

                // Split the range [rangeStartValue, rangeEndValue] into ranges of IDs with an equal number of digits.
                for(uint j = 0; j < (rangeEndDigitCount - rangeStartDigitCount) + 1; j++)
                {
                    uint rangeDigitCount = (rangeStartDigitCount + j);
                    
                    rangeFirstValue = (rangeLastValue + 1);
                    rangeLastValue = Math.Min(rangeEndValue, SpecializedMath.Pow10(rangeDigitCount) - 1);
                    
                    // Ranges with an IDs with an off number of digits cannot contain invalid IDs, 
                    // so these ranges do not need to be scanned.
                    if((rangeDigitCount > 1) && (rangeDigitCount % 2) == 0)
                    {
                        var firstValueDigits = new FixedArray16<byte>();
                        ulong x = rangeFirstValue;
                        for(int k = 0; k < rangeDigitCount; k++)
                        {
                            firstValueDigits[k] = (byte)(x % 10);
                            x /= 10;
                        }

                        idRangeScanJobs.Add(new IdRange
                        {   
                            FirstValue = rangeFirstValue,
                            LastValue = rangeLastValue,
                            FirstValueDigits = firstValueDigits,
                            DigitCount = rangeDigitCount,
                        });
                    }
                }
            }

            return idRangeScanJobs;
        }

        /// <summary>
        /// Calculates the sum of every invalid ID in the specified number range.
        /// </summary>
        /// <param name="range"> The range of numbers to scan. </param>
        /// <returns> The sum of every invalid ID in the specified number range. </returns>
        private static ulong SumInvalidIdsInRange(IdRange range)
        {
            ulong invalidIdSum = 0;

            var currentIdValue = range.FirstValue;
            var currentIdDigits = range.FirstValueDigits;
            int idPatternCompareOffset = (int)(range.DigitCount / 2);

            while(currentIdValue <= range.LastValue)
            {
                bool idInvalid = true;

                for(int j = 0; j < idPatternCompareOffset; j++)
                {
                    if(currentIdDigits[j] != currentIdDigits[j + idPatternCompareOffset]) {
                        idInvalid = false;
                        break;
                    }
                }

                if(idInvalid)
                {
                    invalidIdSum += currentIdValue;
                }

                int digitIndex = 0;
                while (currentIdDigits[digitIndex] == 9)
                {
                    currentIdDigits[digitIndex] = 0;
                    digitIndex++;
                }
                currentIdDigits[digitIndex]++;

                currentIdValue++;
            }

            return invalidIdSum;
        } 
        
      

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            ulong invalidIdCount = 0;
            
            var numberRangesToScan = GetNumberRangesToScan(puzzleInput);

            for(int i = 0; i < numberRangesToScan.Count; i++)
            {
                invalidIdCount += SumInvalidIdsInRange(numberRangesToScan[i]);
            }
            
            return invalidIdCount;
        }
    }
}