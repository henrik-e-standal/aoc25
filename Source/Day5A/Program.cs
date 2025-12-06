
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day5A
{
    public class Program
    {
        /// <summary>
        /// Gets the input to use for the puzzle.
        /// </summary>
        /// <remarks>
        /// See solution readme for documentation on program arguments.
        /// </remarks>
        /// <param name="args"> Arguments passed via command line. </param>
        /// <returns> The input to use for the puzzle. </returns>
        private static bool TryGetPuzzleInput(string[] programArgs, [NotNullWhen(true)] out string? puzzleInput)
        {
            bool dryRunRequested = false;

            if(programArgs.Length > 1) {
                dryRunRequested = string.Equals(programArgs[1], "1");
            }
            if(programArgs.Length > 0) {
                puzzleInput = File.ReadAllText(programArgs[0]);
            }
            else {
#if DEBUG
                puzzleInput = PuzzleInput.Personalized;
#else
                puzzleInput = null;
#endif
            }

            if(dryRunRequested) {
                puzzleInput = null;
            }

            return (!dryRunRequested && (puzzleInput != null));
        }

        /// <summary>
        /// Entry point for the program.
        /// </summary>
        /// <param name="args"> Passed arguments. </param>
        public static void Main(string[] args)
        {
            ulong puzzleResult = 0;

            if(TryGetPuzzleInput(args, out string? puzzleInput))
            {
                BenchmarkTimer.Tick();
                puzzleResult = PuzzleSolver.Solve(puzzleInput);
                BenchmarkTimer.Tock();
            }

            FastConsole.WriteLine(puzzleResult);     
#if DEBUG
            if(PuzzleResult.IsValidatable(puzzleInput)) {
                Debug.Assert(PuzzleResult.IsValid(puzzleInput, puzzleResult));
            }
#endif  
        }
    }
}