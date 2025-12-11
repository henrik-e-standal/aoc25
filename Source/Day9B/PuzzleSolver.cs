
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

            public bool Intersects(HorizontalLineSegment2D other)
            {
                return (X >= other.XMin && X <= other.XMax) && 
                       (other.Y >= YMin && other.Y <= YMax);
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

            public bool Intersects(VerticalLineSegment2D other)
            {
                return other.Intersects(this);
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

        /// <summary>
        /// Find a list of points that define a polygon that of uncolored tiles.
        /// </summary>
        /// <remarks> 
        /// These points form a polygon which is the polygon defined by the red tiles, inflated by 
        /// a unit of 1 in all directions. The polygon formed by the original red tiles encompass 
        /// all colored (e.g. valid tiles). The inflated polygon returned by this method defines 
        /// boundary lines that no valid rectangle solution can intersect.
        /// </remarks>
        /// <param name="redTileCoordinates"> A list of all the red tiles in the puzzle input. </param>
        /// <param name="redTileCount"> The number of red tiles in the puzzle input. </param>
        /// <returns></returns>
        private static Coordinate2D[] DetermineTileBoundaryPoints(Coordinate2D[] redTileCoordinates, int redTileCount)
        {
            var tileBoundaryCoordinates = new Coordinate2D[redTileCount + 2];

            // Makes checking each adjacent point triplet easier in the loop below.
            redTileCoordinates[redTileCount] = redTileCoordinates[0];
            redTileCoordinates[redTileCount + 1] = redTileCoordinates[1];

            for(int i = 1; i <= redTileCount + 1; i++)
            {
                var previousTile = redTileCoordinates[i - 1];
                var currentTile = redTileCoordinates[i];  
                var nextTile = redTileCoordinates[i + 1];

                int shiftX = 0;
                int shiftY = 0;

                int orientation = -Math.Sign(
                    ((currentTile.Y - previousTile.Y) * (nextTile.X - currentTile.X)) -
                    ((currentTile.X - previousTile.X) * (nextTile.Y - currentTile.Y)));

                if(currentTile.X == previousTile.X) 
                {
                    shiftY = (currentTile.Y < previousTile.Y) ? -1 : 1;
                    shiftX = (currentTile.X < nextTile.X) ? -1 : 1;
                }
                else 
                {
                    shiftX = (currentTile.X < previousTile.X) ? -1 : 1;
                    shiftY = (currentTile.Y < nextTile.Y) ? -1 : 1;
                }

                tileBoundaryCoordinates[i - 1] = new Coordinate2D
                {
                    X = (currentTile.X + (shiftX * orientation)),
                    Y = (currentTile.Y + (shiftY * orientation)),
                };
            }

            return tileBoundaryCoordinates;
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

            /*if(rectangleTop != rectangleBottom) 
            {
                for(int i = 0; i < tileBoundaryData.HorizontalBoundaryCount; i++)
                { 
                    var horizontalBoundary = tileBoundaryData.HorizontalBoundaries[i];
                    
                    if(horizontalBoundary.Y < rectangleTop)
                        continue;
                    if(horizontalBoundary.Y > rectangleBottom) {
                        break;
                    }
                    if((rectangleLeft > horizontalBoundary.XMin && rectangleLeft < horizontalBoundary.XMax) ||
                       (rectangleRight > horizontalBoundary.XMin && rectangleRight < horizontalBoundary.XMax)) 
                    {
                        horizontalBoundaryCrosses++;
                        break;
                    }
                }
            }
            else
            {
                horizontalBoundaryCrosses = 0;
            }

            if(rectangleLeft != rectangleRight) 
            {
                for(int j = 0; j < tileBoundaryData.VerticalBoundaryCount; j++)
                { 
                    var verticalBoundary = tileBoundaryData.VerticalBoundaries[j];

                    if(verticalBoundary.X < rectangleLeft)
                        continue;
                    if(verticalBoundary.X > rectangleRight) {
                        break;
                    }
                    if((rectangleTop > verticalBoundary.YMin && rectangleTop < verticalBoundary.YMax) || 
                       (rectangleBottom > verticalBoundary.YMin && rectangleBottom < verticalBoundary.YMax)) 
                    {
                        verticalBoundaryCrosses++;
                        break;
                    }
                }
            }
            else
            {
                verticalBoundaryCrosses = 0;
            }*/

            var leftLineSegment = new VerticalLineSegment2D { X = rectangleLeft, YMin = rectangleTop, YMax = rectangleBottom};
            var rightLineSegment = new VerticalLineSegment2D { X = rectangleRight, YMin = rectangleTop, YMax = rectangleBottom};
            var topLineSegment = new HorizontalLineSegment2D { Y = rectangleTop, XMin = rectangleLeft, XMax = rectangleRight};
            var bottomLineSegment = new HorizontalLineSegment2D { Y = rectangleBottom, XMin = rectangleLeft, XMax = rectangleRight};

            for(int j = 0; j < tileBoundaryData.VerticalBoundaryCount; j++)
            { 
                var verticalBoundary = tileBoundaryData.VerticalBoundaries[j];

                if(verticalBoundary.Intersects(topLineSegment) || verticalBoundary.Intersects(bottomLineSegment))
                {
                    verticalBoundaryCrosses++;
                    break;
                }
            } 
           
            for(int i = 0; i < tileBoundaryData.HorizontalBoundaryCount; i++)
            { 
                var horizontalBoundary = tileBoundaryData.HorizontalBoundaries[i];
                
                 if(horizontalBoundary.Intersects(leftLineSegment) || horizontalBoundary.Intersects(rightLineSegment))
                {
                    horizontalBoundaryCrosses++;
                    break;
                }
            }
                
            if((verticalBoundaryCrosses > 0) || (horizontalBoundaryCrosses > 0))
            {
                Console.WriteLine($"Rectangle ({point1.X},{point1.Y}),({point2.X},{point2.Y}) crosses uncolored tiles!");
                Console.WriteLine($"Vertical crosses: {verticalBoundaryCrosses}");
                Console.WriteLine($"Horizontal crosses: {horizontalBoundaryCrosses}");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"Rectangle ({point1.X},{point1.Y}),({point2.X},{point2.Y}) is good!");
                Console.WriteLine($"Vertical crosses: {verticalBoundaryCrosses}");
                Console.WriteLine($"Horizontal crosses: {horizontalBoundaryCrosses}");
                Console.WriteLine();
            }

            return (verticalBoundaryCrosses > 0) || (horizontalBoundaryCrosses > 0);
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

        private static void DrawGrid(Coordinate2D[] redTiles, int count, TileBoundaryData tileBoundaryData)
        {
            int columnCount = redTiles.Max(x => x.X) + 3;
            int rowCount = redTiles.Max(x => x.Y) + 2;
 
            var charGrid = new Grid<char>(rowCount, columnCount);

            for(int i = 0; i < rowCount; i++)
            {
                for(int j = 0; j < columnCount; j++)
                {
                    if(redTiles.Take(count).Any(x => x.X == j && x.Y == i))
                    {
                        charGrid[i, j] = '#';
                    }
                    else
                    {
                        charGrid[i, j] = '.';
                    }
                }
            }

            for(int i = 0; i < tileBoundaryData.VerticalBoundaryCount; i++)
            {
                var verticalBoundary = tileBoundaryData.VerticalBoundaries[i];

                for(int y= verticalBoundary.YMin; y <= verticalBoundary.YMax; y++)
                {
                    charGrid[y, verticalBoundary.X] = '|';
                }
            }

            for(int i = 0; i < tileBoundaryData.HorizontalBoundaryCount; i++)
            {
                var horizontalBoundary = tileBoundaryData.HorizontalBoundaries[i];

                for(int x = horizontalBoundary.XMin; x <= horizontalBoundary.XMax; x++)
                {
                    if(charGrid[horizontalBoundary.Y, x] == '|')
                    {
                        charGrid[horizontalBoundary.Y, x] = '\\';
                    }
                    else
                    {
                        charGrid[horizontalBoundary.Y, x] = '-';
                    }
                }
            }

            for(int i = 0; i < rowCount; i++)
            {
                for(int j = 0; j < columnCount; j++)
                {
                    Console.Write(charGrid[i, j]);
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            var (redTileCoordinates, redTileCount) = ParseRedTiles(puzzleInput);
            var coloredTileBoundaryCoordinates = DetermineTileBoundaryPoints(redTileCoordinates, redTileCount);
            var tileBoundaryData = DetermineTileBoundaryLines(coloredTileBoundaryCoordinates, redTileCount);
  
            DrawGrid(redTileCoordinates, redTileCount, tileBoundaryData);

            ulong largestColoredTileArea = 0;
            Coordinate2D largestColoredTilePoint1 = redTileCoordinates[0];
            Coordinate2D largestColoredTilePoint2 = redTileCoordinates[0];

            for(int i = 0; i < redTileCount; i++)
            {
                var firstCornerPoint = redTileCoordinates[i];

                for(int j = (i + 1); j < redTileCount; j++)
                {
                    ulong rectangleArea = CalculateRectangleArea(firstCornerPoint, redTileCoordinates[j]);

                    if((!RectangleCrossesUncoloredTiles(firstCornerPoint, redTileCoordinates[j], in tileBoundaryData)))
                    {
                        if (rectangleArea > largestColoredTileArea)
                        {
                            largestColoredTileArea = rectangleArea;
                            largestColoredTilePoint1 = firstCornerPoint;
                            largestColoredTilePoint2 = redTileCoordinates[j];
                        }       
                    }
                }
            }

            Console.WriteLine($"Largest rectangle ({largestColoredTilePoint1.X},{largestColoredTilePoint1.Y}),({largestColoredTilePoint2.X},{largestColoredTilePoint2.Y})!");

            return largestColoredTileArea;
        }
    }
}