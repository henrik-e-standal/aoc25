
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day6A
{
    /// <summary>
    /// Contains the puzzle solving code.
    /// </summary>
    public static class PuzzleSolver
    {
        /// <summary>
        /// The maximum number of math problems this solver can keep track of.
        /// </summary>
        private const int MaxSupportedMathProblems = 1024;

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
        /// Determines the operator of each math problem in the puzzle.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input containing the math problems. </param>
        /// <returns> The operator of each math problem, and the number of math problems in the puzzle input. </returns>
        private static (char[] mathProblemOperators, int mathProblemCount) DetermineMathProblemOperators(string puzzleInput)
        {
            int mathProblemCount = 0;
            var mathProblemOperators = new char[MaxSupportedMathProblems];

            // Find all math operators, in reverse order.
            for(int i = (puzzleInput.Length - 2); i >= 0; i--)
            {
                char currentChar = puzzleInput[i];

                if((currentChar == '*') || (currentChar ==  '+'))
                {
                    mathProblemOperators[mathProblemCount] = currentChar;
                    mathProblemCount++;
                }
                else if(currentChar == MathProblemNumberLineSeparator)
                {
                    break;
                }
            }

            // Reverse all math operators to get them in correct order (i.e. as given in puzzle input).
            Array.Reverse(mathProblemOperators, 0, mathProblemCount);

            return (mathProblemOperators, mathProblemCount);
        }

        /// <summary>
        /// Computes the result of each math problem in the puzzle.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input containing the math problems. </param>
        /// <param name="mathProblemOperators"> Array containing the operators to use for each math problem. </param>
        /// <param name="mathProblemCount"> The number of math problems in the puzzle input. </param>
        /// <returns> The result of each math problem, stored in an array. </returns>
        private static ulong[] ComputeMathProblemSolutions(string puzzleInput, char[] mathProblemOperators, int mathProblemCount)
        {
            int mathProblemOffset = 0;
            var mathProblemResults = new ulong[MaxSupportedMathProblems];

            // Set the result for each multiply math problem results to 1 initially.
            // This allows us to multiply iteratively when enumerating the puzzle input below.
            for(int i = 0; i < mathProblemCount; i++)
            {
                if(mathProblemOperators[i] == '*') {
                    mathProblemResults[i] = 1;
                }
            }

            // Compute the solution to each math problem in the puzzle.
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

                    if(mathProblemOperators[mathProblemOffset] == '*') {
                        mathProblemResults[mathProblemOffset] *= mathProblemNumber;
                    }
                    else {
                        mathProblemResults[mathProblemOffset] += mathProblemNumber;
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

            return mathProblemResults;
        }

        /// <summary>
        /// Computes the sum of the result of each math problem in the puzzle.
        /// </summary>
        /// <param name="mathProblemResults"> The result of each math problem. </param>
        /// <param name="mathProblemCount"> The number of math problems in the puzzle input. </param>
        /// <returns> The sum of all the math problems in the puzzle. </returns>
        private static ulong SumAllMathProblemSolutions(ulong[] mathProblemResults, int mathProblemCount)
        {
            ulong result = 0;

            for(int i = 0; i < mathProblemCount; i++)
            {
                result += mathProblemResults[i];
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
            // Determine which math operation to perform for each math problem in the puzzle input.
            var (mathProblemOperators, mathProblemCount) = DetermineMathProblemOperators(puzzleInput);

            // Computes the result of each math problem in the puzzle input.
            var mathProblemResults = ComputeMathProblemSolutions(puzzleInput, mathProblemOperators, mathProblemCount);
 
            // Computes the sum of the result of each math problem in the puzzle input.
            return SumAllMathProblemSolutions(mathProblemResults, mathProblemCount);
        }
    }
}