
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day3A
{
    public class Program
    {
        /// <summary>
        /// Gets the input to use for the puzzle.
        /// </summary>
        /// <param name="args"> Arguments passed via command line. </param>
        /// <returns> The input to use for the puzzle. </returns>
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

        public static void Main(string[] args)
        {
            var puzzleInput = GetPuzzleInput(args);

            BenchmarkTimer.Tick();

            uint totalOutputJoltage = 0;
            uint currentBankBestBatteryJoltage = 0;
            uint currentBankSecondBestBatteryJoltage = 0;

            for(int i = 0; i < puzzleInput.Length; i++)
            {
                if(TryGetCharacterNumericValue(puzzleInput[i], out uint batteryJoltage))
                {
                    if((batteryJoltage > currentBankBestBatteryJoltage) && TryGetCharacterNumericValue(puzzleInput[i + 1], out uint nextBatteryJoltage))
                    {
                        currentBankBestBatteryJoltage = batteryJoltage;
                        currentBankSecondBestBatteryJoltage = nextBatteryJoltage;
                    } 
                    else if(batteryJoltage > currentBankSecondBestBatteryJoltage)
                    {
                        currentBankSecondBestBatteryJoltage = batteryJoltage;
                    }
                }
                else
                {
                    totalOutputJoltage += ((currentBankBestBatteryJoltage * 10) + currentBankSecondBestBatteryJoltage);
                    
                    currentBankBestBatteryJoltage = 0;
                    currentBankSecondBestBatteryJoltage = 0;
                }   
            }

            BenchmarkTimer.Tock();

            Console.WriteLine(totalOutputJoltage);
        }
    }
}