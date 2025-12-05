
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day1A
{
    public class Program
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

        public static void Main(string[] args)
        {
            var puzzleInput = GetPuzzleInput(args);

            BenchmarkTimer.Tick();

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

            BenchmarkTimer.Tock();

            Console.WriteLine(dialZeroPositionCounter);
        }
    }
}