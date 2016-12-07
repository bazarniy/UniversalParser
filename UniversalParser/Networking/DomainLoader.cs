namespace Networking
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Base;
    using Base.Helpers;
    using WebClient;

    public class DomainLoader
    {
        private readonly string _domain;
        private readonly int _maxThread;
        private readonly IEnumerable<IDataWriter> _writers;
        private readonly ConcurrentQueue<string> _urlsToGet = new ConcurrentQueue<string>();
        private readonly List<string> _parsedUrls = new List<string>();
        private readonly object _latch = new object();
        private readonly IWebClientFactory _clientFactory;
        private int _activeThreads;

        public DomainLoader(string url, IEnumerable<IDataWriter> writers, IWebClientFactory clientFactory, int maxThread = 10)
            :this(url, Enumerable.Empty<string>(), writers, clientFactory, maxThread)
        {

        }

        public DomainLoader(string url, IEnumerable<string> parsedUrls, IEnumerable<IDataWriter> writers, IWebClientFactory clientFactory, int maxThread = 10)
        {
            _writers = writers;
            _maxThread = maxThread;
            _domain = UrlHelpers.GetDomain(url);
            _urlsToGet.Enqueue(url);
            _parsedUrls.AddRange(parsedUrls);
            _clientFactory = clientFactory;
        }

        public void Start()
        {
            if (_activeThreads < _maxThread)
            {
                Task.Run((Action) DownloadThread);
            }
        }

        private void DownloadThread()
        {
            _activeThreads++;
            try
            {
                using (var wc = _clientFactory.Create())
                {
                    string url;
                    while (GetLink(out url))
                    {
                        var info = wc.Download(url, _domain);
                        if (info.Links.Any())
                        {
                            AddLinks(info.Links);
                            Start();
                        }

                        foreach (var downloadWriter in _writers)
                        {
                            downloadWriter.Write(info);
                        }
                    }
                }
            }
            finally
            {
                _activeThreads--;
            }
        }

        private bool GetLink(out string link)
        {
            link = null;
            while (!_urlsToGet.IsEmpty)
            {
                if (!_urlsToGet.TryPeek(out link)) continue;
                lock (_latch)
                {
                    if (_parsedUrls.Contains(link)) continue;

                    _parsedUrls.Add(link);
                    return true;
                }
            }
            return false;
        }

        private void AddLinks(IEnumerable<string> links)
        {
            lock (_latch)
            {
                _urlsToGet.AddRange(links.Where(newlink => !_parsedUrls.Contains(newlink)));
            }
        }
    }
}