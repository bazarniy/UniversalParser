using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Helpers
{
    public static class DictionaryExtentions
    {
        public static KeyValuePair<TKey, TValue> GetBy<TKey, TValue>(this IDictionary<TKey, TValue> dict,
            Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>, bool> comparePredicate)
        {
            return dict.Aggregate((l, r) => comparePredicate(l, r) == true ? l : r);
        }
    }
}
