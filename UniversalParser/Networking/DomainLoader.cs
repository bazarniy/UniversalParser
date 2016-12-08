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
        }

        public async Task Download()
        {
            var allTasks = new List<Task>();
            var parsedLinks = new List<string>();
            _queue.Enqueue(_domain);

            string link;
            using (var semaphore = new SemaphoreSlim(5))
            {
                while (_queue.TryDequeue(out link))
                {
                    await semaphore.WaitAsync();
                    if (parsedLinks.Contains(link)) continue;

                    allTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            using (var client = _client.Create())
                            {
                                var result = await client.Download(link, _domain).ConfigureAwait(false);
                                _queue.AddRange(result.Links);
                                parsedLinks.Add(link);
                            }
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));
                }
                await Task.WhenAll(allTasks);
            }

        }


    }
}
