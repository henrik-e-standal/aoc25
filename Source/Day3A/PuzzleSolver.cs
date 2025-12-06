
using System.Runtime.CompilerServices;

namespace Aoc25.Day3A
{
    /// <summary>
    /// Contains the puzzle solving code.
    /// </summary>
    public static class PuzzleSolver
    {
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
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
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

            return (ulong)totalOutputJoltage;
        }
    }
}