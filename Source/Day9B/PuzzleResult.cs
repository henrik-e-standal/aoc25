
namespace Aoc25.Day9B
{
#if DEBUG
    /// <summary>
    /// Stores valid solutions to different puzzle inputs.
    /// </summary>
    public static class PuzzleResult
    {
        /// <summary>
        /// The correct result when solving for the example puzzle input.
        /// </summary>
        public const ulong ExampleInput = 24;

        /// <summary>
        /// The correct result when solving for the personalized puzzle input.
        /// </summary>
        public const ulong PersonalizedInput = 4771532800;

        /// <summary>
        /// Gets whether the solution for the specified puzzle input can be validated for correctness.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input. </param>
        /// <returns> True if the solution for the specified puzzle input can be validated, otherwise false. </returns>
        public static bool IsValidatable(string? puzzleInput)
        {
            return (puzzleInput == PuzzleInput.Example) || (puzzleInput == PuzzleInput.Personalized);
        }

        /// <summary>
        /// Determines whether the answer to a puzzle is valid.
        /// </summary>
        /// <param name="puzzleInput"> The input to the puzzle. </param>
        /// <param name="answer"> The answer to the puzzle. </param>
        /// <returns> True if the answer was correct for the input, otherwise false. </returns>
        public static bool IsValid(string? puzzleInput, ulong answer)
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
#endif // DEBUG
}