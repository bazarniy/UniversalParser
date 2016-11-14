namespace Base.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class StringExtentions
    {
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
    }
}