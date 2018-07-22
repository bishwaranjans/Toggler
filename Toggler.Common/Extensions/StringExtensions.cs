using System;
using System.Text;

namespace Toggler.Common.Extensions
{
    public static class StringExtensions
    {
        public static string BreakLongString(this string source, int chunkLength = 30)
        {
            var sb = new StringBuilder(source);
            for (var i = source.Length / chunkLength; i > 0; i--)
            {
                sb.Insert(i * chunkLength, "<br/>");
            }

            return sb.ToString();
        }

        public static string SubstituteEnvironmentVariables(this string source)
        {
            return source.Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
        }
    }
}
