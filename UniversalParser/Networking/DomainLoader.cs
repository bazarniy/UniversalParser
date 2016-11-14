namespace Networking
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Base.Helpers;

    public class DomainLoader
    {
        private readonly string _domain;
        private readonly int _maxThread;
        private readonly IEnumerable<IDataWriter> _writers;
        private readonly ConcurrentQueue<string> _urlsToGet = new ConcurrentQueue<string>();
        private readonly List<string> _parsedUrls = new List<string>();
        private readonly object _latch = new object();
        private int _activeThreads;

        public DomainLoader(string url, IEnumerable<IDataWriter> writers, int maxThread = 10)
        {
            _writers = writers;
            _maxThread = maxThread;
            _domain = UrlHelpers.GetDomain(url);
            _urlsToGet.Enqueue(url);
        }

        public DomainLoader(string url, IEnumerable<string> parsedUrls, IEnumerable<IDataWriter> writers, int maxThread = 10)
        {
            _writers = writers;
            _maxThread = maxThread;
            _domain = UrlHelpers.GetDomain(url);
            _urlsToGet.Enqueue(url);
            _parsedUrls.AddRange(parsedUrls);
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
                using (var wc = new WebClient())
                {
                    string url;
                    while (GetLink(out url))
                    {
                        var info = wc.Download(url);
                        if (info.Code == 200)
                        {
                            info.Links = HtmlHelpers.GetLinks(info.Data, info.Url, _domain).ToArray();
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