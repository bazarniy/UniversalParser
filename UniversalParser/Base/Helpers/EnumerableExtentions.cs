namespace Base.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (var s in list1)
                if (cnt.ContainsKey(s))
                    cnt[s]++;
                else
                    cnt.Add(s, 1);
            foreach (var s in list2)
                if (cnt.ContainsKey(s))
                    cnt[s]--;
                else
                    return false;
            return cnt.Values.All(c => c == 0);
        }

        public static IEnumerable<KeyValuePair<T, int>> GetMax<T>(this IEnumerable<T> collection, Func<T, int> valueGenerator)
        {
            var result = collection.ToDictionary(x => x, valueGenerator);
            var max = result.Values.Max();
            return result.Where(x => x.Value == max);
        }
    }
}