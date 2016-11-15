namespace Base.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;

    public static class ConcurrentBagExtentions
    {
        public static TValue GetOrAdd<TValue>(this ConcurrentBag<TValue> list, Func<TValue, bool> predicate, Func<TValue> valueFactory)
        {
            var ls = list.FirstOrDefault(predicate);
            if (ls != null) return ls;

            ls = valueFactory();
            list.Add(ls);
            return ls;
        }
    }
}