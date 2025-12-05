
using System.Runtime.CompilerServices;

namespace Aoc25.Day3B
{
    /// <summary>
    /// Contains the puzzle solving code.
    /// </summary>
    internal static class PuzzleSolver
    {
        /// <summary>
        /// The maximum number of batteries one bank can contain.
        /// </summary>
        /// <remarks>
        /// This will be the size of the buffer that store the joltage of each battery in a bank.
        /// We don't do any preprocessing to determine how big the banks in the input are, so 
        /// this value must be set big enough to accommodate the largest bank(s) we can receive.
        /// </remarks>
        private const int MaxNumberOfBatteriesPerBank = 256;

        /// <summary>
        /// The number of batteries that should be enabled in each bank.
        /// </summary>
        private const int NumberOfBatteriesToEnablePerBank = 12;

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
        /// Calculates the total joltage output of a battery bank.
        /// </summary>
        /// <param name="bankBatteryJoltageList"> Array containing the joltage of each battery in the bank (in parsed order). </param>
        /// <param name="bankBatteryCount"> The number of batteries in the bank. </param>
        /// <returns> The total joltage output of a battery bank. </returns>
        private static ulong CalculateTotalBankJoltage(uint[] bankBatteryJoltageList, int bankBatteryCount)
        {
            int numberOfBatteriesToEnable = NumberOfBatteriesToEnablePerBank;
            int numberOfBatteriesProcessed = 0;

            while(numberOfBatteriesToEnable > 0)
            {
                uint currentPassBestBatteryJoltage = 0;
                int currentPassBestBatteryIndex = 0;

                for(int i = numberOfBatteriesProcessed; i <= (bankBatteryCount - numberOfBatteriesToEnable); i++)
                {
                    if(bankBatteryJoltageList[i] > currentPassBestBatteryJoltage)
                    {
                        currentPassBestBatteryJoltage = bankBatteryJoltageList[i];
                        currentPassBestBatteryIndex = i;
                    } 
                }

                // Use the passed joltage array to also store the joltage of the enabled battery.
                // We only overwrite battery joltages that we have finished processing. We store
                // enabled battery joltages them in sequential order, starting at index 0.
                bankBatteryJoltageList[(NumberOfBatteriesToEnablePerBank - numberOfBatteriesToEnable)] = currentPassBestBatteryJoltage;
                
                numberOfBatteriesToEnable--;
                numberOfBatteriesProcessed += ((currentPassBestBatteryIndex - numberOfBatteriesProcessed) + 1);
            }
            
            ulong totalBankJoltage = 0;
            for(int j = 0; j < NumberOfBatteriesToEnablePerBank; j++)
            {
                totalBankJoltage = ((totalBankJoltage * 10) + bankBatteryJoltageList[j]); 
            }
            
            return totalBankJoltage;
        }

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            ulong totalOutputJoltage = 0;

            var currentBankJoltageList = new uint[MaxNumberOfBatteriesPerBank];  
            int currentBankBatteryCount = 0;

            for(int i = 0; i < puzzleInput.Length; i++)
            {
                if(TryGetCharacterNumericValue(puzzleInput[i], out uint batteryJoltage))
                {
                    currentBankJoltageList[currentBankBatteryCount] = batteryJoltage;
                    currentBankBatteryCount++;
                }
                else
                {
                    totalOutputJoltage += CalculateTotalBankJoltage(currentBankJoltageList, currentBankBatteryCount);
                    currentBankBatteryCount = 0;
                }
            }

            return totalOutputJoltage;
        }
    }
}