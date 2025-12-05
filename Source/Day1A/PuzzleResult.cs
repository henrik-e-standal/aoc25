
namespace Aoc25.Day1A
{
    /// <summary>
    /// Stores valid solutions to different puzzle inputs.
    /// </summary>
    internal static class PuzzleResult
    {
        /// <summary>
        /// The correct result when solving for the example puzzle input.
        /// </summary>
        public const ulong ExampleInput = 3;

        /// <summary>
        /// The correct result when solving for the personalized puzzle input.
        /// </summary>
        public const ulong PersonalizedInput = 1195;

        /// <summary>
        /// Determines whether the answer to a puzzle is valid.
        /// </summary>
        /// <param name="puzzleInput"> The input to the puzzle. </param>
        /// <param name="answer"> The answer to the puzzle. </param>
        /// <returns> True if the answer was correct for the input, otherwise false. </returns>
        public static bool IsValid(string puzzleInput, ulong answer)
        {
            if((puzzleInput == PuzzleInput.Example) && (answer == PuzzleResult.ExampleInput)){
                return true;
            }
            if((puzzleInput == PuzzleInput.Personalized) && (answer == PuzzleResult.PersonalizedInput)){
                return true;
            }
            return false;
        }
    }
}