
using System.Runtime.InteropServices;
using System.Text;

namespace Aoc25.Common
{
    /// <summary>
    /// A custom console implementation that prints to standard out using native interop.
    /// </summary>
    public static class FastConsole
    {
        /// <summary>
        /// The file descriptor ID for the standard output stream.
        /// </summary>
        private const int StdoutDescriptorId = 1;

        /// <summary>
        /// Imported console write function from libc.
        /// </summary>
        /// <remarks>
        /// See documentation at: https://man7.org/linux/man-pages/man2/write.2.html
        /// </remarks>
        [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
        private static extern int write(int file_descriptor, string buffer, uint count);
        
        /// <summary>
        /// Writes the string representation of the passed object, followed by a newline character, to standard out.
        /// </summary>
        /// <param name="text"> The object whose string representation to write to standard out. </param>
        public static void WriteLine(object obj)
        {
            Write(obj.ToString() + Environment.NewLine);
        }

        /// <summary>
        /// Writes the string representation of the passed object, followed by a newline character, to standard out.
        /// </summary>
        /// <param name="text"> The object whose string representation to write to standard out. </param>
        public static void WriteLine<T>(T obj) where T : struct
        {
            Write(obj.ToString() + Environment.NewLine);
        }

        /// <summary>
        /// Writes the passed string, followed by a newline character, to standard out.
        /// </summary>
        /// <param name="text"> The string to write to standard out. </param>
        public static void WriteLine(string text)
        {
            Write(text + Environment.NewLine);
        }

        /// <summary>
        /// Writes the string representation of the passed object to standard out.
        /// </summary>
        /// <param name="text"> The object whose string representation to write to standard out. </param>
        public static void Write(object obj)
        {
            Write(obj.ToString()!);
        }

        /// <summary>
        /// Writes the string representation of the passed object string to standard out.
        /// </summary>
        /// <param name="text"> The string to write to standard out. </param>
        public static void Write<T>(T obj) where T : struct
        {
            Write(obj.ToString()!);
        }
    
        /// <summary>
        /// Writes the passed string to standard out.
        /// </summary>
        /// <param name="text"> The string to write to standard out. </param>
        public static void Write(string text)
        {
            // NB! 
            // Note that this write function is all lower case.
            // This is the 'write' function from libc.
            write(StdoutDescriptorId, text, (uint)text.Length);
        }
    }
}