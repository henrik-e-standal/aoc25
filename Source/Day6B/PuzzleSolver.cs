
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day6B
{
    /// <summary>
    /// Contains the puzzle solving code.
    /// </summary>
    public static class PuzzleSolver
    {
        /// <summary>
        /// A fixed, inline array with a capacity of 16.
        /// </summary>
        /// <typeparam name="T"> The type of item stored in the array. </typeparam>
        [InlineArray(MaxSupportedNumbersPerMathProblem)]
        private struct FixedArray<T>
        {
            /// <summary>
            /// First element in the array.
            /// </summary>
            T FirstElement;
        }

        /// <summary>
        /// Stores information about a math problem operator.
        /// </summary>
        [DebuggerDisplay("{Character} {IndexOffset}")]
        private struct MathProblemOperator
        {
            /// <summary>
            /// The index offset in the puzzle input line where the math operator was found.
            /// </summary>
            public int IndexOffset;

            /// <summary>
            /// The character that represents the math operator.
            /// </summary>
            public char Character;
        }

        /// <summary>
        /// The maximum amount of math problems this solver can keep track of.
        /// </summary>
        private const int MaxSupportedMathProblems = 1024;

        /// <summary>
        /// The maximum amount of numbers each math problem can consist of.
        /// </summary>
        private const int MaxSupportedNumbersPerMathProblem = 15;       
        
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
        /// Determines the operators to use for each math problem in the puzzle.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input containing the math problems. </param>
        /// <returns> The operator for each math problem, and the number of math problems in the puzzle input. </returns>
        private static (MathProblemOperator[] mathProblemOperators, int mathProblemCount) DetermineMathProblemOperators(
            string puzzleInput)
        {
            int mathOperatorCount = 0;
            var mathOperators = new MathProblemOperator[MaxSupportedMathProblems];

            int i;

            // Find all math operators, in reverse order.
            // Also store the puzzle input index at which each operator was found.
            for(i = (puzzleInput.Length - 1); i >= 0; i--)
            {
                char currentChar = puzzleInput[i];

                if((currentChar == '*') || (currentChar ==  '+'))
                {
                    mathOperators[mathOperatorCount].Character = currentChar;
                    mathOperators[mathOperatorCount].IndexOffset = i;
                    mathOperatorCount++;
                }
                else if(currentChar == MathProblemNumberLineSeparator)
                {
                    i++; // Now points to the index of the last math operator found.
                    break;
                }
            }
 
            // Reverse all math operators to get them in correct order (i.e. as given in 
            // puzzle input string). Correct the indices so that they store the offset 
            // at which each operator is found in the puzzle input line.
            for(int j = 0; j < ((mathOperatorCount + 1) / 2); j++)
            {
                int swapIndex = ((mathOperatorCount - 1) - j);
                
                // Swap operators to reverse order.
                var tmp = mathOperators[j];
                mathOperators[j] = mathOperators[swapIndex];
                mathOperators[swapIndex] = tmp; 

                // Correct the operator index offsets.
                mathOperators[j].IndexOffset -= i; 
                mathOperators[swapIndex].IndexOffset -= i;
            }

            return (mathOperators, mathOperatorCount);
        }

        /// <summary>
        /// Computes the result of each math problem in the puzzle.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input containing the math problems. </param>
        /// <param name="mathProblemOperators"> Array containing the math problem operators and their positions. </param>
        /// <param name="mathProblemCount"> The number of math problems in the puzzle input. </param>
        /// <returns> Every number of every math problem, stored in an array "grouped" by math problem. </returns>
        private static FixedArray<uint>[] DetermineMathProblemNumbers(
            string puzzleInput, 
            MathProblemOperator[] mathOperators, 
            int mathProblemCount)
        {
            var mathProblemNumbers = new FixedArray<uint>[mathProblemCount];
            
            int puzzleInputIndex = 0;
            int puzzleInputLineOffset;

            while((puzzleInput[puzzleInputIndex] != '*') && (puzzleInput[puzzleInputIndex] != '+'))
            {
                puzzleInputLineOffset = puzzleInputIndex;

                for(int mathProblemIndex = 0; mathProblemIndex < mathProblemCount; mathProblemIndex++)
                {
                    int mathProblemNumberOffset = 0;
                    puzzleInputIndex = (puzzleInputLineOffset + mathOperators[mathProblemIndex].IndexOffset);

                    // Ignore leading whitespaces (i.e. zeros) in the current math problem number.
                    while(puzzleInput[puzzleInputIndex] == MathProblemNumberSeparator) 
                    {
                        puzzleInputIndex++;
                        mathProblemNumberOffset++;
                    }

                    // Parse every digit of the current math problem number.
                    while(TryGetCharacterNumericValue(puzzleInput[puzzleInputIndex], out uint parsedValue))
                    {
                        ref uint mathProblemNumber = ref mathProblemNumbers[mathProblemIndex][mathProblemNumberOffset];
                        mathProblemNumber = ((mathProblemNumber * 10) + parsedValue);
                        mathProblemNumberOffset++;
                        puzzleInputIndex++;
                    }
                }

                while(puzzleInput[puzzleInputIndex++] != MathProblemNumberLineSeparator);
            }

            return mathProblemNumbers;
        }

        /// <summary>
        /// Computes the sum of the result of each math problem in the puzzle.
        /// </summary>
        /// <param name="mathProblemOperators"> Array containing the math problem operators.  </param>
        /// <param name="mathProblemNumbers"> Array containing the numbers for each math problem. </param>
        /// <param name="mathProblemCount"> The number of math problems in the puzzle input. </param>
        /// <returns> The sum of all the math problems in the puzzle. </returns>
        private static ulong CalculateMathProblemSolution(
            MathProblemOperator[] mathProblemOperators, 
            FixedArray<uint>[] mathProblemNumbers, 
            int mathProblemCount)
        {
            ulong totalResult = 0;

            for(int i = 0; i < mathProblemCount; i++)
            {
                if(mathProblemOperators[i].Character == '+')
                {
                    for(int j = 0; j < MaxSupportedNumbersPerMathProblem; j++)
                    {
                        totalResult += mathProblemNumbers[i][j];
                    }
                }
                else
                {
                    ulong problemResult = 1;

                    for(int j = 0; j < MaxSupportedNumbersPerMathProblem; j++)
                    {
                        uint number = mathProblemNumbers[i][j];

                        if(number == 0) {
                            break;
                        }
                       
                        problemResult *= number;
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
            // Determine which math operation to perform for each math problem in the puzzle input.
            var (mathProblemOperators, mathProblemCount) = DetermineMathProblemOperators(puzzleInput);

            // Determine the numbers for each math problem.
            var mathProblemNumbers = DetermineMathProblemNumbers(puzzleInput, mathProblemOperators, mathProblemCount);
 
            // Computes the sum of the result of each math problem in the puzzle input.
            return CalculateMathProblemSolution(mathProblemOperators, mathProblemNumbers, mathProblemCount);
        }
    }
}