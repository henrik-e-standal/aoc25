
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day6A
{
    /// <summary>
    /// Contains the puzzle solving code.
    /// </summary>
    internal static class PuzzleSolver
    {
        /// <summary>
        /// The maximum number of math problems this solver can keep track of.
        /// </summary>
        private const int MaxSupportedMathProblemCalculations = 1024;

        /// <summary>
        /// Represents a math problem calculation.
        /// </summary>
        [DebuggerDisplay("{OperationCharacter} {Result}")]
        private struct MathProblemCalculation
        {
            /// <summary>
            /// The result of math problem calculation.
            /// </summary>
            public ulong Result;

            /// <summary>
            /// The character that specifies the math operation to perform in this calculation.
            /// </summary>
            public char OperationCharacter;
        }

        /// <summary>
        /// The character used to represent the separator between two math problem numbers.
        /// </summary>
        private const char MathProblemNumberSeparator = ' ';

        /// <summary>
        /// The character used to represent the separator between two lines of math problem numbers.
        /// </summary>
        private const char MathProblemNumberLineSeparator = '\n';

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
        /// Determines whether a character represents a numeric value.
        /// </summary>
        /// <param name="character"> The character to check. </param>
        /// <returns> True if the character was a numeric value, otherwise false. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CharacterIsNumericValue(char character)
        {
            return (character >= '0') && (character <= '9');
        }

        /// <summary>
        /// Determines which math operation to perform for each math problem in the puzzle and stores 
        /// the result in <see cref="mathProblemCalculations"/>.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input containing the math problems. </param>
        /// <param name="mathProblemCalculations"> Array containing the puzzle math problem calculations. </param>
        private static void DetermineMathProblemOperations(string puzzleInput, MathProblemCalculation[] mathProblemCalculations)
        {
            int mathProblemCount = 0;

            for(int i = (puzzleInput.Length - 1); i >= 0; i--)
            {
                char currentChar = puzzleInput[i];

                if(currentChar == '*')
                {
                    mathProblemCalculations[mathProblemCount].OperationCharacter = currentChar;
                    mathProblemCalculations[mathProblemCount].Result = 1;
                    mathProblemCount++;
                }
                else if (currentChar ==  '+')
                {
                    mathProblemCalculations[mathProblemCount].OperationCharacter = currentChar;
                    mathProblemCount++;
                }
                else if(currentChar == MathProblemNumberLineSeparator)
                {
                    break;
                }
            }

            Array.Reverse(mathProblemCalculations, 0, mathProblemCount);
        }

        /// <summary>
        /// Computes the result of each math problem in the puzzle and stores the result in <see cref="mathProblemCalculations"/>.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input containing the math problems. </param>
        /// <param name="mathProblemCalculations"> Array containing the puzzle math problem calculations. </param>
        private static void ComputeMathProblemSolutions(string puzzleInput, MathProblemCalculation[] mathProblemCalculations)
        {
            int mathProblemOffset = 0;

            for(int i = 0; i <puzzleInput.Length; i++)
            {
                if(CharacterIsNumericValue(puzzleInput[i]))
                {
                    uint mathProblemNumber = 0;
                    while(TryGetCharacterNumericValue(puzzleInput[i], out uint parsedValue))
                    {
                        mathProblemNumber = (mathProblemNumber * 10) + parsedValue;
                        i++;
                    }

                    if(mathProblemCalculations[mathProblemOffset].OperationCharacter == '*') {
                        mathProblemCalculations[mathProblemOffset].Result *= mathProblemNumber;
                    }
                    else {
                        mathProblemCalculations[mathProblemOffset].Result += mathProblemNumber;
                    }

                    mathProblemOffset++;
                } 
                
                if(puzzleInput[i] == MathProblemNumberLineSeparator)
                {
                    mathProblemOffset = 0;
                }
                else if(puzzleInput[i] != MathProblemNumberSeparator)
                {
                    break;
                }
            }    
        }

        /// <summary>
        /// Computes the sum of the result of each math problem in the puzzle.
        /// </summary>
        /// <param name="mathProblemCalculations"> Array containing the solved math problems. </param>
        /// <returns> The sum of all the math problems in the puzzle. </returns>
        private static ulong SumAllMathProblemSolutions(MathProblemCalculation[] mathProblemCalculations)
        {
            ulong result = 0;

            for(int i = 0; i < mathProblemCalculations.Length; i++)
            {
                result += mathProblemCalculations[i].Result;
            }

            return result;
        }

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            var mathProblemCalculations = new MathProblemCalculation[MaxSupportedMathProblemCalculations];
            
            // Determine which math operation to perform for each math problem in the puzzle input.
            DetermineMathProblemOperations(puzzleInput, mathProblemCalculations);

            // Computes the result of each math problem in the puzzle input.
            ComputeMathProblemSolutions(puzzleInput, mathProblemCalculations);
 
            // Computes the sum of the result of each math problem in the puzzle input.
            return SumAllMathProblemSolutions(mathProblemCalculations);
        }
    }
}