using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
    using System.Collections.Concurrent;
    using System.Threading;
    using Base.Helpers;
    using Base.Utilities;
    using WebClient;

    public class DomainLoader
    {
        private readonly IWebClientFactory _client;
        private readonly string _domain;
        private readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();

        public DomainLoader(IWebClientFactory client, string domain)
        {
            client.ThrowIfNull(nameof(client));
            domain.ThrowIfEmpty(nameof(domain));

            if(!UrlHelpers.IsValidDomain(domain)) throw new ArgumentException($"Invalid domain name {domain}", nameof(domain));

            _client = client;
            _domain = domain;
            _queue.Enqueue(_domain);
        }

        public async Task Download(int parralel = 5)
        {
            var allTasks = new Dictionary<string, Task>();

            using (var semaphore = new SemaphoreSlim(parralel))
            {
                while (!_queue.IsEmpty)
                {
                    string link;
                    while (_queue.TryDequeue(out link))
                    {
                        if (allTasks.ContainsKey(link)) continue;

                        allTasks.Add(link, GetLink(link, semaphore));
                    }
                    await Task.WhenAll(allTasks.Values);
                }
            }

        }

        private Task GetLink(string link, SemaphoreSlim semaphore)
        {
            return Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    using (var client = _client.Create())
                    {
                        var result = await client.Download(link, _domain).ConfigureAwait(false);
                        _queue.AddRange(result.Links);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });
        }


    }
}
