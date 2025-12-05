
namespace Aoc25.Day2A
{
    /// <summary>
    /// Stores valid solutions to different puzzle inputs.
    /// </summary>
    internal static class PuzzleResult
    {
        /// <summary>
        /// The correct result when solving for the example puzzle input.
        /// </summary>
        public const ulong Example = 1227775554;

        /// <summary>
        /// The correct result when solving for the personalized puzzle input.
        /// </summary>
        public const ulong Personalized = 28846518423;

        /// <summary>
        /// Determines whether the answer to a puzzle is valid.
        /// </summary>
        /// <param name="puzzleInput"> The input to the puzzle. </param>
        /// <param name="answer"> The answer to the puzzle. </param>
        /// <returns> True if the answer was correct for the input, otherwise false. </returns>
        public static bool IsValid(string puzzleInput, ulong answer)
        {
            if((puzzleInput == PuzzleInput.Example) && (answer == PuzzleResult.Example)){
                return true;
            }
            if((puzzleInput == PuzzleInput.Personalized) && (answer == PuzzleResult.Personalized)){
                return true;
            }
            return false;
        }
    }
}