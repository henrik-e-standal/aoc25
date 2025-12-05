
using System.Diagnostics;

namespace Aoc25.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class DiagnosticsExtensions
    {
        /// <summary>
        /// Prints the contents of the passed array to console.
        /// </summary>
        public static string Stringify<T>(this T[] array)
        {
            return string.Join(", ", array);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Stringify<T>(this T[] array, int count)
        {
            return string.Join(", ", array.AsSpan(0, count).ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Stringify<T>(this T[] array, int offset, int count)
        {
            return string.Join(", ", array.AsSpan(offset, count).ToArray());
        }
    }
}