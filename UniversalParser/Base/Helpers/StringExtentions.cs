namespace Base.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class StringExtentions
    {
        private static readonly string[] _charsForClearingText = { " ", "\t", "\n", "\r" };

        public static bool StartsWith(this string data, IEnumerable<string> starters)
        {
            return starters.Any(x => data.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool EndsWith(this string data, IEnumerable<string> starters)
        {
            return starters.Any(x => data.EndsWith(x, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool Contains(this string data, IEnumerable<string> starters)
        {
            var data1 = data.ToLowerInvariant();
            return starters.Any(x => data1.Contains(x.ToLowerInvariant()));
        }

        public static string ClearText(this string text)
        {
            return _charsForClearingText.Aggregate(
                text,
                (current, ch) => current.Replace(ch, string.Empty)
                );
        }

        public static string RemoveRight(this string data, char searchChar)
        {
            var index = data.IndexOf(searchChar);
            return index >= 0 ? data.Substring(0, index) : data;
        }

        public static string RemoveLeft(this string data, char searchChar)
        {
            var index = data.IndexOf(searchChar);
            return index >= 0 ? data.Substring(index + 1) : data;
        }

        public static string RemoveFirst(this string data, string text)
        {
            var index = data.ToUpperInvariant().IndexOf(text.ToUpperInvariant(), StringComparison.Ordinal);
            return index >= 0 ? data.Substring(index + text.Length) : data;
        }

        public static string RemoveLastSegment(this string path, string delimeter)
        {
            var indexDelimeter = path.LastIndexOf(delimeter, StringComparison.Ordinal);
            return indexDelimeter >= 0 ? path.Substring(0, indexDelimeter) : path;
        }

        public static bool IsEmpty(this string url)
        {
            return string.IsNullOrWhiteSpace(url);
        }
    }
}