
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day6B
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

        private const int X = 16;

        [InlineArray(X)]
        private struct FixedArray<T>
        {
            T FirstElement;
        }

        /// <summary>
        /// Represents a math problem calculation.
        /// </summary>
        [DebuggerDisplay("{OperationCharacter} {Result}")]
        private struct MathProblem
        {
            /// <summary>
            /// Stores the numbers to perform the math operation on.
            /// </summary>
            public FixedArray<uint> Numbers;

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
        /// Attempts to get the numeric number of a character.
        /// </summary>
        /// <param name="character"> The character whose numeric value to get. </param>
        /// <param name="numericValue"> The numeric value of the character. </param>
        /// <returns> True if the character was a number, otherwise false. </returns>
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
        /// <param name="lastDigitIndex"> The index of the last digit in the puzzle input. </param>
        private static void DetermineMathProblemOperations(
            string puzzleInput, 
            MathProblem[] mathProblems, 
            out int lastDigitIndex)
        {
            int mathProblemCount = 0;
            int i;

            for(i = (puzzleInput.Length - 1); i >= 0; i--)
            {
                char currentChar = puzzleInput[i];

                if((currentChar == '*') || (currentChar ==  '+'))
                {
                    mathProblems[mathProblemCount].OperationCharacter = currentChar;
                    mathProblemCount++;
                }
                else if(currentChar == MathProblemNumberLineSeparator)
                {
                    break;
                }
            }

            lastDigitIndex = (i + 1);
        }

        /// <summary>
        /// Computes the result of each math problem in the puzzle and stores the result in <see cref="mathProblems"/>.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input containing the math problems. </param>
        /// <param name="mathProblems"> Array containing the puzzle math problem calculations. </param>
        /// <param name="lastDigitIndex"> The index of the last digit in the puzzle input. </param>
        private static void DetermineMathProblemNumbers(
            string puzzleInput, 
            MathProblem[] mathProblems,
            int lastDigitIndex)
        {
            int mathProblemOffset = 0;
            
            for(int i = lastDigitIndex; i >= 0; i--)
            {
                if(puzzleInput[i] != MathProblemNumberLineSeparator)
                {
                    int mathProblemNumberOffset = 0;

                    // Ignore trailing whitespaces (i.e. zeros) in the current math problem number.
                    while(puzzleInput[i] == MathProblemNumberSeparator) 
                    {
                        i--;
                        mathProblemNumberOffset++;
                    }

                    // 
                    while(TryGetCharacterNumericValue(puzzleInput[i], out uint parsedValue))
                    {
                        mathProblems[mathProblemOffset].Numbers[mathProblemNumberOffset] += 
                            (x * parsedValue);
                        
                        mathProblemDigitOffset++;
                        i--;
                    }
                    
                    mathProblemOffset++;
                } 

                if(puzzleInput[i] == MathProblemNumberLineSeparator)
                {
                    mathProblemOffset = 0;
                    x *= 10;
                }
            }    

            Console.WriteLine("");
        }

        /// <summary>
        /// Computes the sum of the result of each math problem in the puzzle.
        /// </summary>
        /// <param name="mathProblems"> Array containing the solved math problems. </param>
        /// <returns> The sum of all the math problems in the puzzle. </returns>
        private static ulong CalculateMathProblemSolution(MathProblem[] mathProblems)
        {
            ulong totalResult = 0;

            for(int i = 0; i < mathProblems.Length; i++)
            {
                if(mathProblems[i].OperationCharacter == '+')
                {
                    for(int j = 0; j < X; j++)
                    {
                        totalResult += mathProblems[i].Numbers[j];
                    }
                }
                else
                {
                    ulong problemResult = 0;
                    for(int j = 0; j < X; j++)
                    {
                        problemResult *= mathProblems[i].Numbers[j];
                    }
                    totalResult += problemResult;
                }
            }

            return totalResult;
        }

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            var mathProblemCalculations = new MathProblem[MaxSupportedMathProblemCalculations];
            
            // Determine which math operation to perform for each math problem in the puzzle input.
            DetermineMathProblemOperations(puzzleInput, mathProblemCalculations, out var lastDigitIndex);

            // Computes the result of each math problem in the puzzle input.
            DetermineMathProblemNumbers(puzzleInput, mathProblemCalculations, lastDigitIndex);
 
            // Computes the sum of the result of each math problem in the puzzle input.
            return CalculateMathProblemSolution(mathProblemCalculations);
        }
    }
}