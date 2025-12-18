
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day8B
{
    /// <summary>
    /// Contains the puzzle solving code.
    /// </summary>
    public static class PuzzleSolver
    {
        /// <summary>
        /// The maximum number of junction boxes this solver can keep track of.
        /// </summary>
        private const int MaxSupportedJunctionBoxes = 1024;

        /// <summary>
        /// The number of junction boxes that should be connected in the puzzle.
        /// </summary>
        private const int JunctionBoxConnectionCount = 1000;

        /// <summary>
        /// Represents the position of a junction box.
        /// </summary>
        [DebuggerDisplay("({X}, {Y}, {Z})")]
        private struct JunctionBoxPosition
        {
            public int X;
            public int Y;
            public int Z;
        }

        /// <summary>
        /// Represents the distance between two junction boxes.
        /// </summary>
        [DebuggerDisplay("({DistanceSquared}, {JunctionBox1Index}-{JunctionBox2Index})")]
        public struct JunctionBoxDistance : IComparable<JunctionBoxDistance>
        {
            /// <summary>
            /// The distance between the junction boxes for which this distance was calculated.
            /// </summary>
            public long DistanceSquared;

            /// <summary>
            /// The storage index of the first junction box for which this distance was calculated.
            /// </summary>
            public int JunctionBox1Index;

            /// <summary>
            /// The storage index of the second junction box for which this distance was calculated.
            /// </summary>
            public int JunctionBox2Index;

            /// <summary>
            /// Determine the relative order of two junction box distances.
            /// </summary>
            /// <param name="other"> The other junction box distance. </param>
            /// <returns> A signed value that represents the relative ordering of the distances. </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int CompareTo(JunctionBoxDistance other)
            {
                return DistanceSquared.CompareTo(other.DistanceSquared);
            }

            /// <summary>
            /// Break the junction box distance into a tuple containing the indices of the two 
            /// junction boxes for which this distance was calculated.
            /// </summary>
            /// <param name="junctionBox1Index"> The first junction box index. </param>
            /// <param name="junctionBox2Index"> The second junction box index. </param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out int junctionBox1Index, out int junctionBox2Index)
            {
                junctionBox1Index = JunctionBox1Index;
                junctionBox2Index = JunctionBox2Index;
            }
        }

        /// <summary>
        /// Represents a circuit box circuit.
        /// </summary>
        [DebuggerDisplay("(Connected: {Count})")]
        public sealed class JunctionBoxCircuit 
        {
            /// <summary>
            /// The number of junction boxes that make up this circuit.
            /// </summary>
            public int Count { get; set; }
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
        /// Parses the position of all the junction boxes in the puzzle input. 
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input whose junction box positions to parse. </param>
        /// <returns> 
        /// The position of all the junction boxes in the puzzle input. 
        /// </returns>
        private static (JunctionBoxPosition[] junctionBoxPositions, int junctionBoxCount) ParseJunctionBoxPositions(string puzzleInput)
        {
            var junctionBoxPositions = new JunctionBoxPosition[MaxSupportedJunctionBoxes];
            
            int junctionBoxOffset = 0;

            for(int i = 0; i < puzzleInput.Length;)
            {
                uint xCoordinateNumber = 0;
                uint yCoordinateNumber = 0; 
                uint zCoordinateNumber = 0;

                while(TryGetCharacterNumericValue(puzzleInput[i++], out uint number)) {
                    xCoordinateNumber = (xCoordinateNumber * 10) + number;
                } 
                while(TryGetCharacterNumericValue(puzzleInput[i++], out uint number)) {
                    yCoordinateNumber = (yCoordinateNumber * 10) + number;
                }
                while(TryGetCharacterNumericValue(puzzleInput[i++], out uint number)) {
                    zCoordinateNumber = (zCoordinateNumber * 10) + number;
                }

                junctionBoxPositions[junctionBoxOffset] = new JunctionBoxPosition
                {
                    X = (int)xCoordinateNumber,
                    Y = (int)yCoordinateNumber,
                    Z = (int)zCoordinateNumber
                };

                junctionBoxOffset++;
            }

            return (junctionBoxPositions, junctionBoxOffset);
        } 

        /// <summary>
        /// Calculates the squared distance between two junction based on their positions.
        /// </summary>
        /// <param name="position1"> The position of the first junction box. </param>
        /// <param name="position2"> The position of the second junction box. </param>
        /// <returns> The squared distance between the two junction boxes. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long CalculateJunctionBoxDistanceSquared(JunctionBoxPosition position1, JunctionBoxPosition position2)
        {
            long deltaX = (long)(position2.X - position1.X);
            long deltaY = (long)(position2.Y - position1.Y);
            long deltaZ = (long)(position2.Z - position1.Z);

            return ((deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ)); 
        }


        /// <summary>
        /// Finds the distance between every junction box and returns them ordered by distance.
        /// </summary>
        /// <param name="junctionBoxPositions"> A list of junction box positions. </param>
        /// <param name="junctionBoxCount"> The number of junction boxes. </param>
        /// <returns> 
        /// A heap containing every junction box pair and the distances between them, ordered by distance.
        /// </returns>
        private static MinHeap<JunctionBoxDistance> FindClosestJunctionBoxes(
            JunctionBoxPosition[] junctionBoxPositions, 
            int junctionBoxCount)
        { 
            var junctionBoxDistances = new JunctionBoxDistance[(junctionBoxCount * (junctionBoxCount / 2))];
            int junctionBoxPairCount = 0;

            for(int i = 0; i < junctionBoxCount; i++)
            {
                for(int j = (i + 1); j < junctionBoxCount; j++)
                {
                    junctionBoxDistances[junctionBoxPairCount++] = new JunctionBoxDistance 
                    {
                        DistanceSquared = CalculateJunctionBoxDistanceSquared(junctionBoxPositions[i], junctionBoxPositions[j]),
                        JunctionBox1Index = i, 
                        JunctionBox2Index = j 
                    };
                }
            }

            return new MinHeap<JunctionBoxDistance>(ref junctionBoxDistances, junctionBoxPairCount);
        }

        /// <summary>
        /// Finds the index of the two junction boxes that completes a circuit of all junction boxes in the input.
        /// </summary>
        /// <param name="junctionBoxDistances">
        /// A heap containing every junction box pair and the distances between them, ordered by distance.
        /// </param>
        /// <param name="junctionBoxCount"> The number of junction boxes in the puzzle input. </param>
        /// <returns> 
        /// The index of the two junction boxes that completes a circuit of all junction boxes in the input. 
        /// </returns>
        private static (int junctionBox1Index, int juctionBox2Index) FindJunctionBoxesThatCompleteCircuit(
            MinHeap<JunctionBoxDistance> junctionBoxDistances, 
            int junctionBoxCount)
        {
            // A indexing structure that combines a lookup table and many linked lists. 
            // Which circuit each junction box is part of can be checked by index lookup.
            // The index of every junction box that is also part of the same circuit can be 
            // found by following the "Next" index until it ends up back at the initial index.
            var junctionBoxCircuitLookup = new (JunctionBoxCircuit? Circuit, int Next)[junctionBoxCount];
            
            var largestJunctionBoxCircuitSize = 0;
            int junctionBox1Index = 0; 
            int junctionBox2Index = 0;

            while(largestJunctionBoxCircuitSize != junctionBoxCount)
            {
                // Assumption: puzzle is always solvable, so pop can never fail.
                (junctionBox1Index, junctionBox2Index) = junctionBoxDistances.Pop();

                ref var junctionBox1Node = ref junctionBoxCircuitLookup[junctionBox1Index];
                ref var junctionBox2Node = ref junctionBoxCircuitLookup[junctionBox2Index];

                var junctionBox1Circuit = junctionBox1Node.Circuit;
                var junctionBox2Circuit = junctionBox2Node.Circuit;

                if(junctionBox1Circuit == null && junctionBox2Circuit == null) 
                {
                    var newJunctionBoxCircuit = new JunctionBoxCircuit { Count = 2 };
                   
                    junctionBox1Node.Circuit = newJunctionBoxCircuit;
                    junctionBox1Node.Next = junctionBox2Index;

                    junctionBox2Node.Circuit = newJunctionBoxCircuit;
                    junctionBox2Node.Next = junctionBox1Index;
                }
                else if(junctionBox1Circuit == null)
                {
                    junctionBox1Node.Circuit = junctionBox2Node.Circuit;
                    junctionBox1Node.Next = junctionBox2Node.Next;
                    junctionBox2Node.Next = junctionBox1Index;
                    
                    junctionBox2Node.Circuit!.Count++;
                }               
                else if(junctionBox2Circuit == null)
                {
                    junctionBox2Node.Circuit = junctionBox1Node.Circuit;
                    junctionBox2Node.Next = junctionBox1Node.Next;
                    junctionBox1Node.Next = junctionBox2Index;

                    junctionBox1Node.Circuit!.Count++;
                }
                else if(junctionBox1Circuit != junctionBox2Circuit)
                {
                    junctionBox1Node.Circuit!.Count += junctionBox2Node.Circuit!.Count;
                    junctionBox2Node.Circuit = junctionBox1Node.Circuit;

                    ref var currentNode = ref junctionBox2Node;
                    while(currentNode.Next != junctionBox2Index)
                    {
                        currentNode = ref junctionBoxCircuitLookup[currentNode.Next];
                        currentNode.Circuit = junctionBox1Node.Circuit;
                    } 

                    currentNode.Next = junctionBox1Node.Next;
                    junctionBox1Node.Next = junctionBox2Index;
                }

                largestJunctionBoxCircuitSize = Math.Max(
                    largestJunctionBoxCircuitSize,
                    junctionBox1Node.Circuit!.Count);
            }

            return (junctionBox1Index, junctionBox2Index);
        }

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            var (junctionBoxPositions, junctionBoxCount) = ParseJunctionBoxPositions(puzzleInput);
           
            var junctionBoxDistances = FindClosestJunctionBoxes(junctionBoxPositions, junctionBoxCount);
            
            var (junctionBox1Index, junctionBox2Index) = FindJunctionBoxesThatCompleteCircuit(
                junctionBoxDistances, 
                junctionBoxCount);

            return ((ulong)junctionBoxPositions[junctionBox1Index].X * (ulong)junctionBoxPositions[junctionBox2Index].X);
        }
    }
}