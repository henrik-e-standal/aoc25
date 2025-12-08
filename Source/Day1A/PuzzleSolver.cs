
using System.Runtime.CompilerServices;

namespace Aoc25.Day1A
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
            const int totalDialPositions = 100;

            return (totalDialPositions + dialPosition % totalDialPositions) % totalDialPositions; 
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
                        continue;
                }

                uint dialRotationAmount = 0;
                
                while(TryGetCharacterNumericValue(puzzleInput[++i], out uint parsedValue))
                {
                    dialRotationAmount = (dialRotationAmount * 10) + parsedValue;
                }

                dialPosition = WrapToValidDialPosition(dialPosition + ((int)dialRotationAmount * (int)dialRotateDirection));
                
                if(dialPosition == 0) {
                    dialZeroPositionCounter++;
                }
            }

            return (ulong)dialZeroPositionCounter;
        }
    }
}