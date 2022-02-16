namespace Aeon.Acquisition
{
    static class StringExtensions
    {
        public static string AsNullIfEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value) ? value : null;
        }
    }
}
