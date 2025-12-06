
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
        /// 
        /// </summary>
        private enum MathOperation
        {
            Add,
            Multiply
        }

        /// <summary>
        /// 
        /// </summary>
        private struct MathProblem
        {
            public char OperationCharacter;
            public int NumbersStartIndex;
            public int Count;
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

        private static FastList<uint> Parse(string puzzleInput)
        {
            // Stores all the math problem numbers in the puzzle input sequentially.
            var mathProblemNumbers = new FastList<uint>(2048);

            var mathProblems = new FastList<MathProblem>(2048);

            int mathProblemNumberLineStartIndex = 0;

            for(int i = puzzleInput.Length - 1; i >= 0; i++)
            {
                switch(puzzleInput[i])
                {
                    case MathProblemNumberSeparator: {
                        continue;
                    }
                    case MathProblemNumberLineSeparator: {
                        mathProblemNumberLineStartIndex = mathProblemNumbers.Count;
                        continue;
                    }
                }

                if(TryGetCharacterNumericValue(puzzleInput[i], out _))
                {
                    uint mathProblemNumber = 0;
                    while(TryGetCharacterNumericValue(puzzleInput[i], out uint parsedValue))
                    {
                        mathProblemNumber = (mathProblemNumber * 10) + parsedValue;
                        i++;
                    }
                    mathProblemNumbers.Add(mathProblemNumber);
                }
                else 
                {
                    mathProblems.Add(new MathProblem
                    {
                        OperationCharacter = puzzleInput[i],
                        NumbersStartIndex = mathProblemNumberLineStartIndex,
                        Count = 0
                    });
                }
            }

            return mathProblemNumbers;
        }

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            var parse = Parse(puzzleInput);
            return (ulong)0;
        }
    }
}