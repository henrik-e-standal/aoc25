
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day8A
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
        public sealed class JunctionBoxCircuit : IComparable<JunctionBoxCircuit>
        {
            /// <summary>
            /// The number of junction boxes that make up this circuit.
            /// </summary>
            public int Count { get; set; }

            /// <summary>
            /// Determine the relative order of two junction box distances.
            /// </summary>
            /// <param name="other"> The other junction box distance. </param>
            /// <returns> A signed value that represents the relative ordering of the distances. </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int CompareTo(JunctionBoxCircuit? other)
            {
                if(other == null) {
                    return 1;
                }

                // Circuits do not have that many junction boxes each, 
                // so this calculation should never over- or underflow.
                return (Count - other.Count);
            }
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
        /// Finds the <see cref="JunctionBoxConnectionCount"/> junction boxes closest together. 
        /// </summary>
        /// <param name="junctionBoxPositions"> A list of junction box positions. </param>
        /// <param name="junctionBoxCount"> The number of junction boxes. </param>
        /// <returns> 
        /// A min-heap containing the distances of the <see cref="JunctionBoxConnectionCount"/>
        /// that are closest together.
        ///  </returns>
        private static MinHeap<JunctionBoxDistance> FindClosestJunctionBoxes(
            JunctionBoxPosition[] junctionBoxPositions, 
            int junctionBoxCount)
        { 
            // A bit counter-intuitively; use a max-heap to keep track of the N smallest
            // junction box distances found so far. Using a max-heap, we can efficiently 
            // look up the largest distance out of the N smallest distances that we store
            // in the heap. See loop below for clarification.
            var maxHeap = new MaxHeap<JunctionBoxDistance>(JunctionBoxConnectionCount);

            for(int i = 0; i < junctionBoxCount; i++)
            {
                for(int j = (i + 1); j < junctionBoxCount; j++)
                {
                    long junctionDistanceSquared = CalculateJunctionBoxDistanceSquared(
                        junctionBoxPositions[i], 
                        junctionBoxPositions[j]);

                    if (!maxHeap.IsFull()) 
                    {
                        maxHeap.Push(new JunctionBoxDistance 
                        {
                            DistanceSquared = junctionDistanceSquared, 
                            JunctionBox1Index = i, 
                            JunctionBox2Index = j 
                        });
                    }
                    else if(junctionDistanceSquared < maxHeap.Peek().DistanceSquared)
                    {
                        maxHeap.RemoveTopAndPush(new JunctionBoxDistance 
                        {
                            DistanceSquared = junctionDistanceSquared, 
                            JunctionBox1Index = i, 
                            JunctionBox2Index = j 
                        });
                    }
                }
            }

            return maxHeap.ConvertToMinHeap(); 
        }

        /// <summary>
        /// Calculates the product of the sizes of the three largest junction box circuits.
        /// </summary>
        /// <param name="closestJunctionBoxes">
        /// A heap containing the <see cref="JunctionBoxConnectionCount"/> junction box pairs that 
        /// closest together in distance, ordered by distance.
        /// </param>
        /// <param name="junctionBoxCount"> The number of junction boxes in the puzzle input. </param>
        /// <returns> The product of the sizes of the three largest junction box circuits. </returns>
        private static ulong CalculateProductOfThreeLargestCircuitSizes(
            MinHeap<JunctionBoxDistance> closestJunctionBoxes, 
            int junctionBoxCount)
        {
            // A indexing structure that combines a lookup table and many linked lists. 
            // Which circuit each junction box is part of can be checked by index lookup.
            // The index of every junction box that is also part of the same circuit can be 
            // found by following the "Next" index until it ends up back at the initial index.
            var junctionBoxCircuitLookup = new (JunctionBoxCircuit? Circuit, int Next)[junctionBoxCount];
  
            while(!closestJunctionBoxes.IsEmpty())
            {
                var (junctionBox1Index, junctionBox2Index) = closestJunctionBoxes.Pop();

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
            }

            // The following part could be done a lot more elegantly using LINQ, but this is a lot faster.

            // Sort the circuit indexing structure (will use the JunctionBoxCircuit IComparable implementation).
            // The result will be all junction box circuits sorted by size, smallest to largest, with duplicates
            // (empty circuits, i.e. circuits that are null, are considered the smallest during sorting).
            Array.Sort(junctionBoxCircuitLookup);

            ulong circuitJunctionSizeProduct = 1;
            var lastProcessedCircuit = default(JunctionBoxCircuit);
            int processedCircuitCount = 0;

            // Iterate the sorted indexing structure backwards to find the largest circuits. 
            for(int i = (junctionBoxCircuitLookup.Length - 1); i >= 0; i--)
            {
                if(junctionBoxCircuitLookup[i].Circuit != lastProcessedCircuit)
                {
                    lastProcessedCircuit = junctionBoxCircuitLookup[i].Circuit;
                    circuitJunctionSizeProduct *= (ulong)lastProcessedCircuit!.Count; 
            
                    if(++processedCircuitCount >= 3) {
                        break;
                    }
                }
            }

            return circuitJunctionSizeProduct;
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
            
            return CalculateProductOfThreeLargestCircuitSizes(junctionBoxDistances, junctionBoxCount);
        }
    }
}