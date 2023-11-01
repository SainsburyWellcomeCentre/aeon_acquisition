using System.Text.RegularExpressions;

namespace Aeon.Environment
{
    static class StringExtensions
    {
        public static string AsNullIfEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value) ? value : null;
        }

        public static string ReplaceLineEndings(this string value)
        {
            return ReplaceLineEndings(value, System.Environment.NewLine);
        }

        public static string ReplaceLineEndings(this string value, string newLine)
        {
            return Regex.Replace(value, @"\r\n|\n\r|\n|\r", newLine);
        }
    }
}
