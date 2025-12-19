
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
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
        /// Represents a vertical line segment in 2-dimensional space.
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
        /// Represents a horizontal line segment in 2-dimensional space.
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

        /// <summary>
        /// Stores information about the tile boundaries in the puzzle input. 
        /// </summary>
        private struct TileBoundaryInfo
        {
            /// <summary>
            /// Stores all vertical tile boundaries, sorted in ascending order by X-coordinate.
            /// </summary>
            public VerticalLineSegment2D[] VerticalBoundaries; 

            /// <summary>
            /// Stores the index of the next relevant vertical tile boundary for each tile X-coordinate.
            /// </summary>
            public int[] NextVerticalBoundaryLookup;
 
            /// <summary>
            /// Stores all horizontal tile boundaries, sorted in ascending order by Y-coordinate.
            /// </summary>
            public HorizontalLineSegment2D[] HorizontalBoundaries;
            
            /// <summary>
            /// Stores the index of the next relevant horizontal tile boundary for each tile Y-coordinate.
            /// </summary>
            public int[] NextHorizontalBoundaryLookup;
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
                uint xCoordinate = 0;
                uint yCoordinate = 0; 

                while(TryGetCharacterNumericValue(puzzleInput[i++], out uint number)) {
                    xCoordinate = (xCoordinate * 10) + number;
                } 
                while(TryGetCharacterNumericValue(puzzleInput[i++], out uint number)) {
                    yCoordinate = (yCoordinate * 10) + number;
                }

                redTileCoordinates[redTileOffset++] = new Coordinate2D
                {
                    X = (int)xCoordinate,
                    Y = (int)yCoordinate,
                };
            }

            return (redTileCoordinates, redTileOffset);
        }

        /// <summary>
        /// Expand the polygon formed by the red tile coordinates in the puzzle input by 1 unit. 
        /// </summary>
        /// <param name="redTileCoordinates"> A list of all the red tiles in the puzzle input. </param>
        /// <param name="redTileCount"> The number of red tiles in the puzzle input. </param>
        /// <returns> The polygon formed by the red tile coordinates in the puzzle input by 1 unit. </returns>
        private static Coordinate2D[] ExpandRedTilePolygon(Coordinate2D[] redTileCoordinates, int redTileCount)
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

                int orientation = -Math.Sign(
                    ((currentTile.Y - previousTile.Y) * (nextTile.X - currentTile.X)) -
                    ((currentTile.X - previousTile.X) * (nextTile.Y - currentTile.Y)));

                int directionX;
                int directionY;

                if(currentTile.X == previousTile.X) 
                {
                    directionY = (currentTile.Y < previousTile.Y) ? -1 : 1;
                    directionX = (currentTile.X < nextTile.X) ? -1 : 1;
                }
                else 
                {
                    directionX = (currentTile.X < previousTile.X) ? -1 : 1;
                    directionY = (currentTile.Y < nextTile.Y) ? -1 : 1;
                }

                tileBoundaryCoordinates[i - 1] = new Coordinate2D
                {
                    X = (currentTile.X + (directionX * orientation)),
                    Y = (currentTile.Y + (directionY * orientation)),
                };
            }

            return tileBoundaryCoordinates;
        }

        /// <summary>
        /// Finds all tile boundaries in the puzzle input.
        /// </summary>
        /// <param name="redTileCoordinates"> A list of all the red tiles in the puzzle input. </param>
        /// <param name="redTileCount"> The number of red tiles in the puzzle input. </param>
        /// <returns> Information about the tile boundaries in the puzzle input. </returns>
        private static TileBoundaryInfo FindAllTileBoundaries(
            Coordinate2D[] redTileCoordinates, 
            int redTileCount)
        {
            // Arrays that keep track of tile boundary segments on both axes.
            var verticalBoundaries = new VerticalLineSegment2D[MaxSupportedRedTiles];
            var horizontalBoundaries = new HorizontalLineSegment2D[MaxSupportedRedTiles];

            int verticalBoundaryCount = 0;
            int horizontalBoundaryCount = 0;

            // Add the first red tile coordinate to the end to also handle the boundary between the last -> first wrap.
            redTileCoordinates[redTileCount] = redTileCoordinates[0];

            // Index all tile boundary segments.
            for(int i = 1; i <= redTileCount; i++)
            {
                var previousCoordinate = redTileCoordinates[i - 1];
                var currentCoordinate = redTileCoordinates[i];

                // NB! This code assumes that there are no adjacent red tiles that overlaps on both x- and y- axis!
                if(previousCoordinate.X == currentCoordinate.X)
                {
                    verticalBoundaries[verticalBoundaryCount++] = new VerticalLineSegment2D
                    {
                        X = currentCoordinate.X,
                        YMin = Math.Min(previousCoordinate.Y, currentCoordinate.Y),
                        YMax = Math.Max(previousCoordinate.Y, currentCoordinate.Y)
                    };
                }
                else // if(previousCoordinate.Y == currentCoordinate.Y)
                {
                    horizontalBoundaries[horizontalBoundaryCount++] = new HorizontalLineSegment2D
                    {
                        Y = currentCoordinate.Y,
                        XMin = Math.Min(previousCoordinate.X, currentCoordinate.X),
                        XMax = Math.Max(previousCoordinate.X, currentCoordinate.X)
                    }; 
                }
            }

            // Sort the vertical tile boundary segments according to their X-coordinates 
            // and the horizontal tile boundary segments according to their Y-coordinates.
            Array.Sort(verticalBoundaries, 0, verticalBoundaryCount);
            Array.Sort(horizontalBoundaries, 0, horizontalBoundaryCount);

            // The code below creates a lookup table that stores the index of the next relevant 
            // vertical tile boundary segment for each X-coordinate. 
            var nextVerticalBoundaryLookup = new int[verticalBoundaries[verticalBoundaryCount - 1].X]; 
            int currentVerticalBoundaryIndex = 0;
            var currentVerticalBoundary = verticalBoundaries[0];

            for(int i = 0; i < nextVerticalBoundaryLookup.Length; i++)
            {
                while((currentVerticalBoundaryIndex < verticalBoundaryCount) && (i >= currentVerticalBoundary.X)) 
                {
                    currentVerticalBoundaryIndex++;
                    currentVerticalBoundary = verticalBoundaries[currentVerticalBoundaryIndex];
                }

                nextVerticalBoundaryLookup[i] = currentVerticalBoundaryIndex;
            }
 
            // The code below creates a lookup table that stores the index of the next relevant 
            // horizontal tile boundary segment for each Y-coordinate. 
            var nextHorizontalBoundaryLookup = new int[horizontalBoundaries[horizontalBoundaryCount - 1].Y];
            int currentHorizontalBoundaryIndex = 0;
            var currentHorizontalBoundary = horizontalBoundaries[0];

            for(int i = 0; i < nextHorizontalBoundaryLookup.Length; i++)
            {
                while((currentHorizontalBoundaryIndex < horizontalBoundaryCount) && (i >= currentHorizontalBoundary.Y)) 
                {
                    currentHorizontalBoundaryIndex++;
                    currentHorizontalBoundary = horizontalBoundaries[currentHorizontalBoundaryIndex];
                }

                nextHorizontalBoundaryLookup[i] = currentHorizontalBoundaryIndex;
            }

            return new TileBoundaryInfo
            {
                VerticalBoundaries = verticalBoundaries,
                HorizontalBoundaries = horizontalBoundaries,
                NextVerticalBoundaryLookup = nextVerticalBoundaryLookup,
                NextHorizontalBoundaryLookup = nextHorizontalBoundaryLookup
            };
        }

        /// <summary>
        /// Determines whether a rectangle defined by two opposite corners points intersects an uncolored tile.
        /// </summary>
        /// <param name="point1"> The point that defines the first rectangle corner. </param>
        /// <param name="point2"> The point that defines the rectangle corner opposite to the first. </param>
        /// <param name="tileBoundaryData"> Information about the tile boundaries in the puzzle input. </param>
        /// <returns> True if the rectangle intersects an uncolored tile. </returns>
        private static bool RectangleIntersectsUncoloredTiles(
            Coordinate2D point1, 
            Coordinate2D point2,
            TileBoundaryInfo tileBoundaryData)
        {
            int rectangleTop = Math.Min(point1.Y, point2.Y);
            int rectangleBottom = Math.Max(point1.Y, point2.Y);
            int rectangleLeft = Math.Min(point1.X, point2.X);
            int rectangleRight = Math.Max(point1.X, point2.X);

            // Check whether rectangle crosses a horizontal boundary.
            if(rectangleTop != rectangleBottom) 
            {
                var horizontalBoundaries = tileBoundaryData.HorizontalBoundaries;
                int firstBoundaryIndex = tileBoundaryData.NextHorizontalBoundaryLookup[rectangleTop];

                for(int i = firstBoundaryIndex; i < horizontalBoundaries.Length; i++)
                { 
                    var horizontalBoundary = horizontalBoundaries[i];
                    
                    if(horizontalBoundary.Y >= rectangleBottom) {
                        break;
                    }
                    if((rectangleLeft >= horizontalBoundary.XMin && rectangleLeft <= horizontalBoundary.XMax) ||
                       (rectangleRight >= horizontalBoundary.XMin && rectangleRight <= horizontalBoundary.XMax)) {
                        return true;
                    }
                }
            }

            // Check whether rectangle crosses a vertical boundary.
            if(rectangleLeft != rectangleRight) 
            { 
                var verticalBoundaries = tileBoundaryData.VerticalBoundaries;
                int firstBoundaryIndex = tileBoundaryData.NextVerticalBoundaryLookup[rectangleLeft];

                for(int j = firstBoundaryIndex; j < verticalBoundaries.Length; j++)
                { 
                    var verticalBoundary = verticalBoundaries[j];
 
                    if(verticalBoundary.X >= rectangleRight) {
                        break;
                    }
                    if((rectangleTop >= verticalBoundary.YMin && rectangleTop <= verticalBoundary.YMax) || 
                       (rectangleBottom >= verticalBoundary.YMin && rectangleBottom <= verticalBoundary.YMax)) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Calculate the area of the rectangle defined by two points in opposite corners.
        /// </summary>
        /// <param name="point1"> The point that defines the first rectangle corner. </param>
        /// <param name="point2"> The point that defines the rectangle corner opposite to the first. </param>
        /// <returns> The area of the rectangle defined by the points. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long CalculateRectangleArea(
            Coordinate2D point1, 
            Coordinate2D point2)
        {
            return (long)(Math.Abs(point1.X - point2.X) + 1) * 
                   (long)(Math.Abs(point1.Y - point2.Y) + 1);
        }     

        /// <summary>
        /// Finds the area of the largest rectangle that can be formed using two red tile coordinates
        /// that doesn't intersect an uncolored tile.
        /// </summary>
        /// <param name="redTileCoordinates"> A list of red tile coordinates. </param>
        /// <param name="redTileCount"> The number of red tiles in the puzzle input. </param>
        /// <param name="tileBoundaryData"> Information about the tile boundaries in the puzzle input. </param>
        /// <returns> 
        /// The area of the largest rectangle that can be formed using two red tile coordinates
        /// that doesn't intersect an uncolored tile.
        /// </returns>
        private static ulong FindLargestRectangleArea(
            Coordinate2D[] redTileCoordinates, 
            int redTileCount, 
            TileBoundaryInfo tileBoundaryData)
        {
            long largestColoredTileArea = 0;

            for(int i = 0; i < redTileCount; i++)
            {
                var firstCornerPoint = redTileCoordinates[i];

                for(int j = (i + 1); j < redTileCount; j++)
                {
                    var secondCornerPoint = redTileCoordinates[j];

                    long rectangleArea = CalculateRectangleArea(firstCornerPoint, secondCornerPoint);

                    if (rectangleArea > largestColoredTileArea)
                    {
                        if(!RectangleIntersectsUncoloredTiles(firstCornerPoint, secondCornerPoint, tileBoundaryData))
                        {
                            largestColoredTileArea = rectangleArea;
                        }       
                    }
                }
            }

            return (ulong)largestColoredTileArea;
        }

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            var (redTileCoordinates, redTileCount) = ParseRedTiles(puzzleInput);

            // The current implementation only works for puzzle input where the largest rectangle 
            // does not fall entirely outside of polygon formed by the red tiles. For such a puzzle 
            // input, the red tile polygon should be expanded by 1 unit, and each rectangle should 
            // be checked for intersection against this expanded polygon.
            // var expandedRedTileCoordinates = ExpandRedTilePolygon(redTileCoordinates, redTileCount);
            // var tileBoundaryInfo = FindAllTileBoundaries(expandedRedTileCoordinates, redTileCount);

            var tileBoundaryInfo = FindAllTileBoundaries(redTileCoordinates, redTileCount);

            return FindLargestRectangleArea(redTileCoordinates, redTileCount, tileBoundaryInfo);
        }
    }
}