
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day2B
{
    public class Program
    {  
        [InlineArray(16)]
        public struct FixedArray16<T>
        {
            private T element0;
        }

        /// <summary>
        /// Gets the input to use for the puzzle.
        /// </summary>
        /// <param name="args"> Arguments passed via command line. </param>
        /// <returns> The input to use for the puzzle. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetPuzzleInput(string[] args)
        {
    #if DEBUG
            return PuzzleInput.Personalized;
    #else
            return args[1];
    #endif
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
        /// Calculates the sum of all invalid IDs in a ID range.
        /// </summary>
        /// <param name="rangeStartDigits"> An array that stores the digits of the first value in the range, in order of significance. </param>
        /// <param name="rangeStartDigitCount"> The number of digits in the first value in the range. </param>
        /// <param name="rangeEndDigitCount">The number of digits in the last value in the range. </param>
        /// <param name="rangeLength"> The total number of IDs in the passed range. </param>
        /// <returns> The sum of all invalid IDs in the range. </returns>
        private static ulong SumInvalidIdsInRange2(
            ulong rangeStartValue,
            uint[] rangeStartDigits, 
            uint rangeStartDigitCount,
            uint rangeEndDigitCount,  
            uint rangeLength)
        {
            ulong invalidIdSum = 0;
            var currentIdValue = rangeStartValue;
            var currentIdDigits = rangeStartDigits;
            uint currentIdDigitCount = rangeStartDigitCount;

            // Scan the range for invalid IDs.
            for(int i = 0; i < rangeLength; i++)
            {
                // Skip checking IDs with an odd number of digits - these cannot be invalid.
                if(currentIdDigitCount > 1)
                {
                    for(uint patternLength = 1; patternLength <= (currentIdDigitCount / 2); patternLength++)
                    {
                        if(currentIdDigitCount % patternLength == 0)
                        {
                            bool idInvalid = true;

                            for(uint j = 0; j < (currentIdDigitCount - patternLength); j++)
                            {
                                if(currentIdDigits[j] != currentIdDigits[j + patternLength]) 
                                {
                                    idInvalid = false;
                                    break;
                                }
                            }
                            
                            if(idInvalid)
                            {
                                invalidIdSum += currentIdValue;
                                break;
                            }
                        }
                    }
                }

                currentIdValue++;

                uint digitIndex = 0;
                while (currentIdDigits[digitIndex] == 9)
                {
                    currentIdDigits[digitIndex++] = 0;
                }
                
                if (digitIndex < currentIdDigitCount) {
                    currentIdDigits[digitIndex]++;
                }
                else {
                    currentIdDigits[currentIdDigitCount++] = 1;
                }
            }

            return invalidIdSum;
        }

        private static int[][] a = new int[16][]
        {
            Array.Empty<int>(),
            Array.Empty<int>(),
            new int[] { 1 },
            new int[] { 1 },
            new int[] { 1, 2 }, 
            new int[] { 1 },
            new int[] { 1, 2, 3 },
            new int[] { 1 },
            new int[] { 1, 2, 4 },
            new int[] { 1, 3 },
            new int[] { 1, 2, 5 },
            new int[] { 1 },
            new int[] { 1, 2, 3, 4, 6 },
            new int[] { 1 },
            new int[] { 1, 2, 7 },
            new int[] { 1, 3, 5 },
        };

        private static int[] GetPatternLengthToScan(int digitCount)
        {
            return a[digitCount];
        }

        private static ulong SumInvalidIdsInRange(IdRange range)
        {
            ulong invalidIdSum = 0;

            var currentIdValue = range.FirstValue;
            var currentIdDigits = range.FirstValueDigits;
            int currentDigitCount = (int)range.DigitCount;
            var rangeSize = (int)(range.LastValue - range.FirstValue) + 1;
            var x = GetPatternLengthToScan(currentDigitCount);
            
            for(int i = 0; i < rangeSize; i++)
            {  
                for(int j = 0; j < x.Length; j++)
                {
                    int patternLength = x[j];
      
                    for(int k = 0; k < (currentDigitCount - patternLength); k++)
                    {
                        if(currentIdDigits[k] != currentIdDigits[k + patternLength]) 
                        {
                            goto outer_loop;
                        }
                    }

                    invalidIdSum += currentIdValue;
                    break;
                    outer_loop:{}
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
        
        public static void Main(string[] args)
        {
            var puzzleInput = GetPuzzleInput(args);

            if(puzzleInput == null) {
                return;
            }

            BenchmarkTimer.Tick();

            ulong invalidIdCount = 0;
            
            var idRangesToScan = GetIdRangesToScan(puzzleInput);

            for(int i = 0; i < idRangesToScan.Count; i++)
            {
                /*Console.WriteLine();
                Console.WriteLine($"Range #{i}:");
                Console.WriteLine($"FirstValue = {idRangesToScan[i].FirstValue}:");
                Console.WriteLine($"LastValue = {idRangesToScan[i].LastValue}:");
                uint[] array = new uint[16];
                for(int j = 0; j < 16; j++)
                {
                    array[j] = idRangesToScan[i].FirstValueDigits[j];
                }
                Console.WriteLine($"FirstValueDigits = [{array.Stringify((int)idRangesToScan[i].DigitCount)}]:");
                Console.WriteLine($"DigitCount = {idRangesToScan[i].DigitCount}:");
                Console.WriteLine();*/

                invalidIdCount += SumInvalidIdsInRange(idRangesToScan[i]);
            }
            

            BenchmarkTimer.Tock();

            Console.WriteLine(invalidIdCount);

            //Debug.Assert(PuzzleResult.IsValid(puzzleInput, invalidIdCount));
        }

        public static void Main2(string[] args)
        {
            var puzzleInput = GetPuzzleInput(args);

            BenchmarkTimer.Tick();

            ulong invalidIdCount = 0;
            var idRangeStartDigits = new uint[16];
            
            for(int i = 0; i < puzzleInput.Length;)
            {
                ulong idRangeStartValue = 0;
                uint idRangeStartDigitCount = 0;
                ulong idRangeEndValue = 0;
                uint idRangeEndDigitCount = 0;

                // The following parsing logic assumes that the input is exactly as described
                // on the AoC website; stray symbols of any kind will break it horribly.
                while(TryGetCharacterNumericValue(puzzleInput[i++], out uint numericValue))
                {
                    idRangeStartValue = (idRangeStartValue * 10) + numericValue;
                    idRangeStartDigits[idRangeStartDigitCount] = numericValue;  
                    idRangeStartDigitCount++;
                }
                while(TryGetCharacterNumericValue(puzzleInput[i++], out uint numericValue))
                {
                    idRangeEndValue = (idRangeEndValue * 10) + numericValue;
                    idRangeEndDigitCount++;
                }

                Array.Clear(idRangeStartDigits, (int)idRangeStartDigitCount, idRangeStartDigits.Length - (int)idRangeStartDigitCount);
                Array.Reverse(idRangeStartDigits, 0, (int)idRangeStartDigitCount);

                /*Console.WriteLine($"Range: [{idRangeStartValue}, {idRangeEndValue}]");
                Console.WriteLine($"Start value digits: [{Diagnostics.StringifyArray(idRangeStartDigits, (int)idRangeStartDigitCount)}]");
                Console.WriteLine($"Start value digit count: {idRangeStartDigitCount}");
                Console.WriteLine($"Start value: {idRangeStartValue}");
                Console.WriteLine($"End value digit count: {idRangeEndDigitCount}");
                Console.WriteLine($"End value: {idRangeEndValue}");
                Console.WriteLine();*/

                invalidIdCount += SumInvalidIdsInRange2(
                    idRangeStartValue,
                    idRangeStartDigits, 
                    idRangeStartDigitCount, 
                    idRangeEndDigitCount,
                    (uint)((idRangeEndValue - idRangeStartValue) + 1));
            }

            BenchmarkTimer.Tock();

            Console.WriteLine(invalidIdCount);

            //Debug.Assert(PuzzleResult.IsValid(puzzleInput, invalidIdCount));
        }

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

        private static FastList<IdRange> GetIdRangesToScan(string puzzleInput)
        {
            var idRangeScanJobs = new FastList<IdRange>(1024);

            for(int i = 0; i < puzzleInput.Length;)
            {
                if(TryGetCharacterNumericValue(puzzleInput[i], out _))
                {
                    ulong rangeStartValue = 0;
                    uint rangeStartDigitCount = 0;

                    // Determine the value of, and the number of digits in, the first ID in the current ID range.
                    while(TryGetCharacterNumericValue(puzzleInput[i++], out uint numericValue))
                    {
                        rangeStartValue = (rangeStartValue * 10) + numericValue;
                        rangeStartDigitCount++;
                    }

                    ulong rangeEndValue = 0;
                    uint rangeEndDigitCount = 0;
                    
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
                        if(rangeDigitCount > 1)
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
            }

            return idRangeScanJobs;
        }
    }
}