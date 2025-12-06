
using System.Runtime.CompilerServices;

namespace Aoc25.Day1B
{
    /// <summary>
    /// Contains the puzzle solving code.
    /// </summary>
    public static class PuzzleSolver
    {
        /// <summary>
        /// Represents different directions the dial can be rotated.
        /// </summary>
        private enum DialRotateDirection : int
        {
            Left = -1, 
            Right = 1,
            Invalid = 0
        }

        /// <summary>
        /// The total number of positions on the dial.
        /// </summary>
        private const int TotalDialPositions = 100;

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
        /// Wraps the passed dial position value to wrap to the valid dial position range.
        /// </summary>
        /// <param name="dialPosition"> The dial position value to wrap to the valid dial position range. </param>
        /// <returns> The passed dial position value to wrapped to the valid dial position range. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int WrapToValidDialPosition(int dialPosition)
        { 
            return (TotalDialPositions + dialPosition % TotalDialPositions) % TotalDialPositions; 
        }

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            int dialZeroPositionCounter = 0;

            int dialPosition = 50;
            var dialRotateDirection = default(DialRotateDirection);

            for(int i = 0; i < puzzleInput.Length; i++)
            {
                switch (puzzleInput[i])
                {
                    case 'L': 
                        dialRotateDirection = DialRotateDirection.Left;
                        break;
                    case 'R':
                        dialRotateDirection = DialRotateDirection.Right;
                        break;
                    default:
                        // Skip new lines, carriage returns, etc.
                        continue;
                }

                uint dialRotationAmount = 0;
                
                while(TryGetCharacterNumericValue(puzzleInput[++i], out uint parsedValue))
                {
                    dialRotationAmount = (dialRotationAmount * 10) + parsedValue;
                }
 
                int unwrappedDialPosition = dialPosition + ((int)dialRotationAmount * (int)dialRotateDirection);

                if(unwrappedDialPosition > 0) 
                {
                    dialZeroPositionCounter += (unwrappedDialPosition / TotalDialPositions);
                } 
                else  if(unwrappedDialPosition < 0)
                {                     
                    dialZeroPositionCounter += (-(unwrappedDialPosition - TotalDialPositions) / TotalDialPositions);
                }
                else if (unwrappedDialPosition == 0)
                { 
                    dialZeroPositionCounter++;
                }

                // Correct for edge-cases in calculations above.
                // In the case of new dial position = zero (i.e. no change from last dial position), the increment 
                // above would give a false zero detection if the previous dial position was also zero (going from 
                // zero -> zero should not count as passing zero). In the case of a new negative dial position, 
                // the calculation above result in one extra zero detection in the case where previous dial position
                // is zero, specifically. Going from 0 to -100 (0) gives: -(-100 - 100) / 100 = 2, but the correct
                // answer should be one. Going from a positive value (1-99) to -100 (0) is the same calculation, 
                // but in this case 2 is correct (once going past 0, and once past -99). 
                if((dialPosition == 0) && (unwrappedDialPosition <= 0))
                {
                    dialZeroPositionCounter--;
                }

                dialPosition = WrapToValidDialPosition(unwrappedDialPosition);
            }

            return (ulong)dialZeroPositionCounter;
        }
    }
}