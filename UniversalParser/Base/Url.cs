namespace Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Helpers;
    using Utilities;

    [Serializable]
    public sealed class Url
    {
        public string Domain { get; private set; }
        public string Path { get; private set; }

        private IEnumerable<string> _params;

        public string Params
        {
            get { return string.Join("&", _params.OrderBy(s => s)); }
        }

        public Url(string url, string domain = "")
        {
            if (string.IsNullOrWhiteSpace(domain)) url.ThrowIfEmpty(nameof(url));

            Domain = GetDomain(url, domain);
            Path = GetPath(url, Domain);
            _params = GetParams(url, Domain);
        }

        private Url()
        {
        }

        private static string GetDomain(string url, string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                url.ThrowIfEmpty(nameof(url));
            }
            var urlDomain = UrlHelpers.GetDomain(url);
            if (!string.IsNullOrWhiteSpace(urlDomain)) domain = urlDomain;
            
            if (!UrlHelpers.IsValidDomain(domain)) throw new ArgumentException("invalid domain");

            return domain;
        }

        private static string GetPath(string url, string domain)
        {
            var result = UrlHelpers.ClearAnchor(url);
            result = UrlHelpers.ClearParams(result);
            result = UrlHelpers.RemoveDomain(result, domain);

            if (result.EndsWith("/")) result = result.TrimEnd('/');
            if (string.IsNullOrWhiteSpace(result)) result = "/";
            return result;
        }

        private static IEnumerable<string> GetParams(string url, string domain)
        {
            return UrlHelpers.GetParams(url).Split(new[] {'&'}, StringSplitOptions.RemoveEmptyEntries);
        }


        public Url LinkTo(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !UrlHelpers.IsContentLink(url)) return this;
            if (!string.IsNullOrWhiteSpace(UrlHelpers.GetDomain(url))) return new Url(url);
            if (UrlHelpers.IsAbsolute(url)) return new Url(url, Domain);

            var result = new Url
            {
                Domain = Domain,
                Path = GetPath(url, Domain),
                _params = GetParams(url, Domain)
            };

            result.Path = Path + (result.Path != "/" ? "/" + result.Path : "");

            return result;
        }

        public override string ToString()
        {
            return $"{Domain}{Path}" + (!string.IsNullOrWhiteSpace(Params) ? "?" + Params : "");
        }

        public override bool Equals(object obj)
        {
            var that = obj as Url;
            if (that == null) return false;
            return ToString() == that.ToString();
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }
    }
}