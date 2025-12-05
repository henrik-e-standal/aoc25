
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day3B
{
    public class Program
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

        public static void Main(string[] args)
        {
            var puzzleInput = GetPuzzleInput(args);

            BenchmarkTimer.Tick();

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

            BenchmarkTimer.Tock();

            Console.WriteLine(totalOutputJoltage);
        }
    }
}