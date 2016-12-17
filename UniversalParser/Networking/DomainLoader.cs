namespace Networking
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Base;
    using Base.Helpers;
    using Base.Utilities;
    using WebClient;

    public class DomainLoader
    {
        private readonly IWebClientFactory _client;
        private readonly IDataWriter _writer;
        private readonly Url _domain;
        private readonly ConcurrentQueue<Url> _queue = new ConcurrentQueue<Url>();
        private readonly Dictionary<Url, Task<Exception>> _allTasks = new Dictionary<Url, Task<Exception>>();

        public DomainLoader(IWebClientFactory client, IDataWriter writer, string domain)
        {
            client.ThrowIfNull(nameof(client));
            writer.ThrowIfNull(nameof(writer));
            _domain = Url.Create(domain);

            _client = client;
            _writer = writer;
            _queue.Enqueue(_domain);
        }

        public Dictionary<string, Exception> GetResults()
        {
            return _allTasks.ToDictionary(x => x.Key.ToString(), x => x.Value.Result);
        }

        public async Task Download(int parralel = 5)
        {
            using (var semaphore = GetSemaphore(parralel))
            {
                if (semaphore == null) return;
                while (!_queue.IsEmpty)
                {
                    Url link;
                    while (_queue.TryDequeue(out link))
                    {
                        if (_allTasks.ContainsKey(link)) continue;
                        _allTasks.Add(link, GetLink(link, semaphore));
                    }
                    await Task.WhenAll(_allTasks.Values);
                }
            }
        }

        private Task<Exception> GetLink(Url link, SemaphoreSlim semaphore)
        {
            return Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    using (var client = _client.Create())
                    {
                        var result = await client.Download(link).ConfigureAwait(false);
                        _queue.AddRange(
                            result.Links
                                .Select(x => x.Fix())
                                .Where(x => x != null && x.Domain == _domain.Domain)
                        );
                        _writer.Write(result);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    return ex;
                }
                finally
                {
                    semaphore.Release();
                }
            });
        }

        private bool _isInit;
        private readonly object _latch = new object();
        private SemaphoreSlim GetSemaphore(int parralel)
        {
            if (_isInit) return null;
            lock (_latch)
            {
                if (_isInit) return null;
                _isInit = true;
                return new SemaphoreSlim(parralel);
            }
        }


    }
}
