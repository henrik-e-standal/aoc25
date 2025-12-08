
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day7A
{
    /// <summary>
    /// Contains the puzzle solving code.
    /// </summary>
    public static class PuzzleSolver
    {
        /// <summary>
        /// The character used to represent a beam splitter.
        /// </summary>
        private const char BeamSplitterCharacter = '^';

        /// <summary>
        /// The character used to represent the start of the beam.
        /// </summary>
        private const char BeamStartCharacter = 'S';

        /// <summary>
        /// Determines the width and height of the grid passed as puzzle input, as well as the beam start index.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input containing the grid. </param>
        /// <returns> The width and height of the puzzle grid, as well as the beam start index. </returns>
        private static (int gridColumnCount, int gridRowCount, int beamStartIndex) DetermineGridDimensionsAndBeamStartIndex(
            string puzzleInput)
        {
            int gridColumnCount = 0;
            int beamStartIndex = 0;

            // Determine the width of a single grid row of the input.
            while (puzzleInput[gridColumnCount++] != '\n') 
            {
                if(puzzleInput[gridColumnCount] == BeamStartCharacter) {
                    beamStartIndex = gridColumnCount;
                }
            }

            // Determines the height of the grid in the input.
            // Assumption: Each row consists of [gridColumnCount] cells plus a single newline.
            int gridRowCount = (puzzleInput.Length / gridColumnCount);

            return(gridColumnCount, gridRowCount, beamStartIndex);
        }

        /// <summary>
        /// Counts the number of times the beam splits.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input containing the grid. </param>
        /// <param name="gridColumnCount"> The number of columns in the puzzle grid. </param>
        /// <param name="gridRowCount"> The number of rows in the puzzle grid. </param>
        /// <param name="beamStartIndex"> The beam start index in the puzzle input. </param>
        /// <returns> The number of times the beam splits. </returns>
        private static uint CountBeamSplits(
            string puzzleInput, 
            int gridColumnCount, 
            int gridRowCount, 
            int beamStartIndex)
        {
            uint beamSplitCount = 0;

            var beamTrackingGrid = new byte[gridColumnCount * gridRowCount];
            beamTrackingGrid[beamStartIndex] = 1;

            // Begin checking for beam splits on the second row of the puzzle grid.
            for(int i = gridColumnCount; i < puzzleInput.Length; i++)
            {
                if(beamTrackingGrid[i - gridColumnCount] > 0)
                {
                    if(puzzleInput[i] == BeamSplitterCharacter)
                    {
                        beamTrackingGrid[i - 1] = 1;
                        beamTrackingGrid[i + 1] = 1;
                        beamSplitCount++;
                    }
                    else
                    {
                        beamTrackingGrid[i] = 1;
                    }
                }
            }

            return beamSplitCount;
        }

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            var (gridColumnCount, gridRowCount, beamStartIndex) = DetermineGridDimensionsAndBeamStartIndex(puzzleInput);
            
            return CountBeamSplits(puzzleInput, gridColumnCount, gridRowCount, beamStartIndex);
        }
    }
}