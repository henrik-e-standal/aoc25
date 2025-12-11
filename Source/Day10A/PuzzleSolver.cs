
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day10A
{
    /// <summary>
    /// Contains the puzzle solving code.
    /// </summary>
    public static class PuzzleSolver
    {
        /// <summary>
        /// The maximum number of machines the solver can solve for.
        /// </summary>
        private const int MaxSupportedMachines = 256;

        /// <summary>
        /// The maximum number of machine buttons the solver can solve for.
        /// </summary>
        private const int MaxSupportedMachineButtonCount = 16;

        /// <summary>
        /// Represents a machine in the puzzle input.
        /// </summary>
        [DebuggerDisplay("{PrintIndicatorStateBitmask(TargetIndicatorState, IndicatorCount)}")]
        private struct Machine
        {
            /// <summary>
            /// A bitmask that represent the desired indicator light state for the machine.
            /// </summary>
            public uint TargetIndicatorState;

            /// <summary>
            /// The number of light indicators on the machine.
            /// </summary>
            public int IndicatorCount;

            /// <summary>
            /// Stores bitmasks that represent which light indicators each button toggle for the machine.
            /// </summary>
            public uint[] ButtonIndicatorToggleStates;

            /// <summary>
            /// The number of buttons on the machine.
            /// </summary>
            public int ButtonCount;
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
        /// Parse all the machines in the puzzle input. 
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input whose machines to parse. </param>
        /// <returns> 
        /// A list of information about all the machines parsed from the puzzle input.
        /// </returns>
        private static FastList<Machine> ParseMachines(string puzzleInput)
        {
            var parsedMachines = new FastList<Machine>(MaxSupportedMachines);

            for(int i = 0; i < puzzleInput.Length;)
            {
                uint targetIndicatorState = 0;
                int indicatorCount = 0;
                var buttonIndicatorToggleStates = new uint[MaxSupportedMachineButtonCount];;
                int buttonCount = 0;

                // Parse the target light indicator state for the current machine.
                while(puzzleInput[++i] != ']')
                {     
                    if(puzzleInput[i] == '#') {
                        targetIndicatorState |= (1U << indicatorCount);
                    } 

                    indicatorCount++;
                }
                i++;

                // Parse which indicators each button on the current machine toggles.
                while(puzzleInput[++i] == '(')
                {
                    while(TryGetCharacterNumericValue(puzzleInput[++i], out uint numericValue)) {
                        buttonIndicatorToggleStates[buttonCount] |= (1U << (int)numericValue);
                        i++;
                    }
                    buttonCount++;
                }
            
                // Skip parsing the joltage requirements for this puzzle.
                while(puzzleInput[i++] != '\n');

                parsedMachines.Add(new Machine
                {
                    TargetIndicatorState = targetIndicatorState,
                    IndicatorCount = indicatorCount,
                    ButtonIndicatorToggleStates = buttonIndicatorToggleStates!,
                    ButtonCount = buttonCount,
                });
            }

            return parsedMachines;
        }

        /// <summary>
        /// Determines the least possible number of button presses to achieve at the desired indicator
        /// state for a machine.
        /// </summary>
        /// <param name="machine"> The machine to find the optimal number of button presses for. </param>
        /// <param name="statePressCountIndex">
        /// Preallocated lookup table that the algorithm will use to store the number of button presses  
        /// required to arrive at each machine indicator state. Each indicator state maps to an index
        /// in the lookup table, and the value stored is a button press amount.
        /// </param>
        /// <param name="unprocessedIndicatorStates"></param>
        /// Preallocated queue that the algorithm will use to store machine indicator states it hasn't
        /// yet processed fully.
        /// <returns> 
        /// The optimal number of button presses to achieve the desired indicator state for the machine. 
        /// </returns>
        private static int FindOptimalMachineButtonPressCount(
            Machine machine, 
            int[] indicatorStatePressCountLookup,
            FastQueue<uint> unprocessedIndicatorStateQueue)
        { 
            // Clear any left-over state in the preallocated storage structures passed to the method.
            Array.Clear(indicatorStatePressCountLookup);
            unprocessedIndicatorStateQueue.Clear(); 
          
            unprocessedIndicatorStateQueue.EnqueueBack(0);

            // Performs a breadth-first search to determine the number of button presses 
            // required to arrive at every possible machine indicator state.
            while(unprocessedIndicatorStateQueue.TryDequeueFront(out uint indicatorState))
            {
                // The number of button presses required to arrive at the current state + 1.
                int nextPressCount = (indicatorStatePressCountLookup[indicatorState] + 1);

                for(int i = 0; i < machine.ButtonCount; i++)
                {
                    uint newIndicatorState = (machine.ButtonIndicatorToggleStates[i] ^ indicatorState);

                    // If the current indicator state has a button press count other than zero,
                    // we have arrived at this state earlier. In this case, we do not need to 
                    // process this indicator state further.
                    if(indicatorStatePressCountLookup[newIndicatorState] == 0)
                    {
                        indicatorStatePressCountLookup[newIndicatorState] = nextPressCount;
                        unprocessedIndicatorStateQueue.EnqueueBack(newIndicatorState);
                    }
                }
            }

            return indicatorStatePressCountLookup[machine.TargetIndicatorState];
        }

        /// <summary>
        /// Calculates the sum of the optimal number of button presses for each machine.
        /// state for a machine.
        /// </summary>
        /// <param name="machines"> The machines to process. </param>
        /// <returns> The sum of the optimal number of button presses for each machine. </returns>
        private static ulong SumOptimalMachineButtonPressCount(FastList<Machine> machines)
        {
            int buttonPressSum = 0;

            // Allocate memory for storage structures used in the algorithm that finds the 
            // optimal number of button presses for a machine. Kinda clunky to allocate them
            // here, but it is a lot faster to allocate them once and keep reusing them.
            var indicatorStatePressCountLookup = new int[1 << machines.Max(x => x.IndicatorCount)];
            var unprocessedIndicatorStateQueue = new FastQueue<uint>(1024);

            for(int i = 0; i < machines.Count; i++)
            {
                // Extra check just in case the puzzle input has a machine that wants all
                // indicators to be off - in which case no button presses are required.
                // The algorithm for calculating optimal number of button presses for 
                // a machine breaks in this specific scenario.
                if(machines[i].TargetIndicatorState != 0)
                {
                    buttonPressSum += FindOptimalMachineButtonPressCount(
                        machines[i], 
                        indicatorStatePressCountLookup,
                        unprocessedIndicatorStateQueue);
                }
            }

            return (ulong)buttonPressSum;
        }

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            var machines = ParseMachines(puzzleInput);

            return SumOptimalMachineButtonPressCount(machines);
        }
    }
}