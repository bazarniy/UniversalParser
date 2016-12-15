namespace Base.Helpers
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public static class ConcurrentQueueExtentions
    {
        public static void AddRange<T>(this ConcurrentQueue<T> queue, IEnumerable<T> links)
        {
            if (links == null) return;
            foreach (var link in links)
                queue.Enqueue(link);
        }
    }
}