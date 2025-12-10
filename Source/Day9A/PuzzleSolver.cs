
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day9A
{
    /// <summary>
    /// Contains the puzzle solving code.
    /// </summary>
    public static class PuzzleSolver
    {   
        /// <summary>
        /// The maximum number of red tiles this solver can solve for.
        /// </summary>
        private const int MaxSupportedRedTiles = 512;

        /// <summary>
        /// Represents a point in 2-dimensional space.
        /// </summary>
        [DebuggerDisplay("({X}, {Y})")]
        private struct Coordinate2D
        {
            public int X;
            public int Y;
        }

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
        /// Parse all the red tiles coordinates in the puzzle input. 
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input whose number ranges to parse. </param>
        /// <returns> The coordinates of each red tile, and the total number of tiles, in the puzzle input. </returns>
        private static (Coordinate2D[] redTileCoordinates, int redTileCount) ParseRedTiles(string puzzleInput)
        {
            var redTileCoordinates = new Coordinate2D[MaxSupportedRedTiles];
            int redTileOffset = 0;

            for(int i = 0; i < puzzleInput.Length;)
            {
                uint xCoordinateNumber = 0;
                uint yCoordinateNumber = 0; 

                while(TryGetCharacterNumericValue(puzzleInput[i++], out uint number)) {
                    xCoordinateNumber = (xCoordinateNumber * 10) + number;
                } 
                while(TryGetCharacterNumericValue(puzzleInput[i++], out uint number)) {
                    yCoordinateNumber = (yCoordinateNumber * 10) + number;
                }

                redTileCoordinates[redTileOffset] = new Coordinate2D
                {
                    X = (int)xCoordinateNumber,
                    Y = (int)yCoordinateNumber,
                };
                redTileOffset++;
            }

            return (redTileCoordinates, redTileOffset);
        }

        /// <summary>
        /// Calculate the area of the rectangle defined by two points in opposite corners.
        /// </summary>
        /// <param name="point1"> The point that defines the first rectangle corner. </param>
        /// <param name="point2"> The point that defines the rectangle corner opposite to the first. </param>
        /// <returns> The area of the rectangle defined by the points. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong CalculateRectangleArea(Coordinate2D point1, Coordinate2D point2)
        {
            return (ulong)Math.Abs(point1.X - point2.X + 1) * (ulong)Math.Abs(point1.Y - point2.Y + 1);
        }     

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            var (redTileCoordinates, redTileCount) = ParseRedTiles(puzzleInput);

            ulong largestRedTileArea = 0;

            for(int i = 0; i < redTileCount; i++)
            {
                var firstCornerPoint = redTileCoordinates[i];

                for(int j = (i + 1); j < redTileCount; j++)
                {
                    largestRedTileArea = Math.Max(
                        largestRedTileArea, 
                        CalculateRectangleArea(firstCornerPoint, redTileCoordinates[j]));
                }
            }

            return largestRedTileArea;
        }
    }
}