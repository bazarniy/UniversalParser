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
    }
}