namespace Base.Helpers
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class UrlHelpers
    {
        private const string DomainPattern = @"https?:\/\/([\w\d]*\.)?[\w\d]*\.[\w]*";

        private const string ProtocolString = "://";

        private static readonly string[] _nonPageSubstrings = {".jpg", ".jpeg", ".gif", ".png", ".pdf", ".xls", ".xlsx", ".rtf", ".zip", ".rar", ".7z", ".gz", ".bz"};

        private static readonly string[] _nonPageSubstringStarts = {"mailto:", "javascript:", "tel:"};

        private static readonly Regex _domainRegex = new Regex(DomainPattern, RegexOptions.Compiled);


        public static string CanonicalizePageLink(string url, string currentUrl)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            var link = ClearAnchor(url);
            if (!IsContentLink(link)) return null;

            // http://domain.com/blabla
            if (HasProtocol(link)) return link;

            // /blabla
            var fullDomain = GetDomain(currentUrl);
            if (IsAbsolute(link)) return fullDomain + link;

            // domain.com/blabla
            var domain = RemoveProtocol(fullDomain);
            if (link.StartsWith(domain)) return GetProtocol(fullDomain) + ProtocolString + link;

            // blabla
            return currentUrl + (currentUrl.EndsWith("/") ? "" : "/") + link;
        }

        public static bool IsValidDomain(string url)
        {
            var domain = GetDomain(url);
            if (string.IsNullOrWhiteSpace(domain)) return false;
            if (domain != url) return false;
            return true;
        }

        public static string GetDomain(string url)
        {
            return _domainRegex.Match(url).Value;
        }

        public static string RemoveDomain(string url, string domain)
        {
            var index = url.ToUpperInvariant().IndexOf(domain.ToUpperInvariant(), StringComparison.Ordinal);
            return index >= 0 ? url.Substring(index + domain.Length) : url;
        }

        public static string ClearParams(string url)
        {
            var index = url.IndexOf("?", StringComparison.Ordinal);
            return index >= 0 ? url.Substring(0, index) : url;
        }

        public static bool IsContentLink(string url)
        {
            return !url.StartsWith(_nonPageSubstringStarts) && !url.EndsWith(_nonPageSubstrings) && !url.Contains(_nonPageSubstrings.Select(x => x + "?"));
        }

        public static string ClearAnchor(string uri)
        {
            var index = uri.IndexOf('#');
            return index >= 0 ? uri.Substring(0, index) : uri;
        }

        public static string GetParams(string url)
        {
            var index = url.IndexOf("?", StringComparison.Ordinal);
            if (index < 0) return "";

            url = UrlHelpers.ClearAnchor(url);

            var param = url.Substring(index + 1);
            index = param.IndexOf("?", StringComparison.Ordinal);
            return index < 0 ? param : param.Substring(0, index);
        }

        public static bool IsAbsolute(string url)
        {
            return url.StartsWith("/");
        }

        private static bool HasProtocol(string url)
        {
            return url.Contains(ProtocolString);
        }

        private static string RemoveProtocol(string url)
        {
            return !HasProtocol(url) ? url : url.Split(new[] {ProtocolString}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        }

        private static string GetProtocol(string url)
        {
            return !HasProtocol(url) ? "" : url.Split(new[] {ProtocolString}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }
    }
}