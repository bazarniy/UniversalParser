namespace Base.Helpers
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public static class ConcurrentQueueExtentions
    {
        public static void AddRange(this ConcurrentQueue<string> queue, IEnumerable<string> links)
        {
            foreach (var link in links)
                queue.Enqueue(link);
        }
    }
}