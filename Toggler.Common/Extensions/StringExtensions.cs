using System;
using System.Text;

namespace Toggler.Common.Extensions
{
    /// <summary>
    /// Extension class for string type
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Breaks the long string.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="chunkLength">Length of the chunk.</param>
        /// <returns></returns>
        public static string BreakLongString(this string source, int chunkLength = 30)
        {
            var sb = new StringBuilder(source);
            for (var i = source.Length / chunkLength; i > 0; i--)
            {
                sb.Insert(i * chunkLength, "<br/>");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Substitutes the environment variables.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string SubstituteEnvironmentVariables(this string source)
        {
            return source.Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
        }
    }
}
