
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day9B
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
        private struct Coordinate2D : IComparable<Coordinate2D>
        {
            public int X;
            public int Y;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int CompareTo(Coordinate2D other)
            {
                return (X != other.X) ? (X - other.X) : (Y - other.Y);
            }
        }

        /// <summary>
        /// Represents a straight vertical line segment in 2-dimensional space.
        /// </summary>
        [DebuggerDisplay("({X}, {YMin}), ({X}, {YMax})")]
        private struct VerticalLineSegment2D : IComparable<VerticalLineSegment2D>
        {
            public int X;
            public int YMin;
            public int YMax;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int CompareTo(VerticalLineSegment2D other)
            {
                return (X - other.X);
            }
        }

        /// <summary>
        /// Represents a straight horizontal line segment in 2-dimensional space.
        /// </summary>
        [DebuggerDisplay("({XMin}, {Y}), ({XMax}, {Y})")]
        private struct HorizontalLineSegment2D : IComparable<HorizontalLineSegment2D>
        {
            public int Y;
            public int XMin;
            public int XMax;
 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int CompareTo(HorizontalLineSegment2D other)
            {
                return (Y - other.Y);
            }
        }

        private struct TileBoundaryData
        {
            public VerticalLineSegment2D[] VerticalBoundaries;
            public HorizontalLineSegment2D[] HorizontalBoundaries;
            public int VerticalBoundaryCount;
            public int HorizontalBoundaryCount;
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
            var redTileCoordinates = new Coordinate2D[MaxSupportedRedTiles + 1];
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

        private static TileBoundaryData DetermineTileBoundaryLines(
            Coordinate2D[] redTileCoordinates, 
            int redTileCount)
        {
            // Arrays that keep track of how the tile boundaries in both cardinal directions.
            var verticalBoundaries = new VerticalLineSegment2D[MaxSupportedRedTiles];
            var horizontalBoundaries = new HorizontalLineSegment2D[MaxSupportedRedTiles];

            int verticalBoundaryCount = 0;
            int horizontalBoundaryCount = 0;

            // Add the first red coordinate to the end to also handle the boundary 
            // between the last -> first wrap.
            redTileCoordinates[redTileCount] = redTileCoordinates[0];

            for(int i = 1; i <= redTileCount; i++)
            {
                // NB! This code assumes that there are no adjacent red tiles
                //     that overlaps on both x- and y- axis!
                if(redTileCoordinates[i - 1].X == redTileCoordinates[i].X)
                {
                    verticalBoundaries[verticalBoundaryCount++] = new VerticalLineSegment2D
                    {
                        X = redTileCoordinates[i].X,
                        YMin = Math.Min(redTileCoordinates[i - 1].Y, redTileCoordinates[i].Y),
                        YMax = Math.Max(redTileCoordinates[i - 1].Y, redTileCoordinates[i].Y)
                    };
                }
                else // if(redTileCoordinates[i - 1].Y == redTileCoordinates[i].Y)
                {
                    horizontalBoundaries[horizontalBoundaryCount++] = new HorizontalLineSegment2D
                    {
                        Y = redTileCoordinates[i].Y,
                        XMin = Math.Min(redTileCoordinates[i - 1].X, redTileCoordinates[i].X),
                        XMax = Math.Max(redTileCoordinates[i - 1].X, redTileCoordinates[i].X)
                    }; 
                }
            }

            Array.Sort(verticalBoundaries, 0, verticalBoundaryCount);
            Array.Sort(horizontalBoundaries, 0, horizontalBoundaryCount);

            for(int i = 0; i < verticalBoundaryCount; i++)
            {
                for(int  j = (i + 1); j < verticalBoundaryCount; j++)
                {
                    var boundary1 = verticalBoundaries[i];
                    var boundary2 = verticalBoundaries[j];
                    if((boundary1.X == boundary2.X) &&
                      ((boundary1.YMax >= boundary2.YMin) || 
                       (boundary2.YMax >= boundary1.YMin)))
                    {
                        Console.WriteLine("Vertical boundary overlap!");
                    }
                }
            }

            return new TileBoundaryData
            {
                VerticalBoundaries = verticalBoundaries,
                HorizontalBoundaries = horizontalBoundaries,
                VerticalBoundaryCount = verticalBoundaryCount,
                HorizontalBoundaryCount = horizontalBoundaryCount
            };
        }

        private static bool RectangleCrossesUncoloredTiles(
            Coordinate2D point1, 
            Coordinate2D point2,
            in TileBoundaryData tileBoundaryData)
        {
            int verticalBoundaryCrosses = 0;
            int horizontalBoundaryCrosses = 0;

            int rectangleTop = Math.Min(point1.Y, point2.Y);
            int rectangleBottom = Math.Max(point1.Y, point2.Y);
            int rectangleLeft = Math.Min(point1.X, point2.X);
            int rectangleRight = Math.Max(point1.X, point2.X);

            if(rectangleTop != rectangleBottom) 
            {
                for(int y = rectangleTop - 1; y < rectangleBottom; y++)
                {
                    for(int j = 0; j < tileBoundaryData.HorizontalBoundaryCount; j++)
                    {
                        var horizontalBoundary = tileBoundaryData.HorizontalBoundaries[j];

                        if((y == horizontalBoundary.Y) && 
                        (rectangleLeft >= horizontalBoundary.XMin) && 
                        (rectangleRight <= horizontalBoundary.XMax))
                        {
                            horizontalBoundaryCrosses++;
                            goto a;
                        }
                    }
                    a: {}
                }
            }
            else
            {
                horizontalBoundaryCrosses = 0;
            }

            if(rectangleLeft != rectangleRight) 
            {
                for(int x = rectangleLeft - 1; x < rectangleRight; x++)
                {
                    for(int j = 0; j < tileBoundaryData.VerticalBoundaryCount; j++)
                    {
                        var verticalBoundary = tileBoundaryData.VerticalBoundaries[j];

                        if((x == verticalBoundary.X) && 
                        (rectangleTop >= verticalBoundary.YMin) && 
                        (rectangleBottom <= verticalBoundary.YMax))
                        {
                            verticalBoundaryCrosses++;
                            goto b;
                        }
                    }
                    b: {}
                }
            }
            else
            {
                verticalBoundaryCrosses = 0;

            }

            if(((verticalBoundaryCrosses != 0) || (horizontalBoundaryCrosses != 0)))
            {
                Console.WriteLine($"Rectangle ({rectangleLeft},{rectangleTop}),({rectangleRight},{rectangleBottom}) crosses uncolored tiles!");
                Console.WriteLine($"Vertical crosses: {verticalBoundaryCrosses}");
                Console.WriteLine($"Horizontal crosses: {horizontalBoundaryCrosses}");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"Rectangle ({rectangleLeft},{rectangleTop}),({rectangleRight},{rectangleBottom}) is good!");
                Console.WriteLine($"Vertical crosses: {verticalBoundaryCrosses}");
                Console.WriteLine($"Horizontal crosses: {horizontalBoundaryCrosses}");
                Console.WriteLine();
            }

            return ((verticalBoundaryCrosses != 0) || (horizontalBoundaryCrosses != 0));
        }

        /// <summary>
        /// Calculate the area of the rectangle defined by two points in opposite corners.
        /// </summary>
        /// <param name="point1"> The point that defines the first rectangle corner. </param>
        /// <param name="point2"> The point that defines the rectangle corner opposite to the first. </param>
        /// <returns> The area of the rectangle defined by the points. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong CalculateRectangleArea(
            Coordinate2D topCornerPoint, 
            Coordinate2D bottomCornerPoint)
        {
            return (ulong)Math.Abs(topCornerPoint.X - bottomCornerPoint.X + 1) * 
                   (ulong)Math.Abs(topCornerPoint.Y - bottomCornerPoint.Y + 1);
        }     

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            var (redTileCoordinates, redTileCount) = ParseRedTiles(puzzleInput);
            var tileBoundaryData = DetermineTileBoundaryLines(redTileCoordinates, redTileCount);

            ulong largestColoredTileArea = 0;
            Coordinate2D largestColoredTilePoint1 = redTileCoordinates[0];
            Coordinate2D largestColoredTilePoint2 = redTileCoordinates[0];

            for(int i = 0; i < redTileCount; i++)
            {
                var firstCornerPoint = redTileCoordinates[i];

                for(int j = (i + 1); j < redTileCount; j++)
                {
                    ulong rectangleArea = CalculateRectangleArea(firstCornerPoint, redTileCoordinates[j]);

                    if((rectangleArea > largestColoredTileArea) && 
                       (!RectangleCrossesUncoloredTiles(firstCornerPoint, redTileCoordinates[j], in tileBoundaryData)))
                    {
                        largestColoredTileArea = rectangleArea;
                        largestColoredTilePoint1 = firstCornerPoint;
                        largestColoredTilePoint2 = redTileCoordinates[j];
                    }
                }
            }

            return largestColoredTileArea;
        }
    }
}