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

        private static readonly string[] _nonPageSubstringStarts = {"mailto", "javascript"};

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

        private static bool IsContentLink(string url)
        {
            return !url.StartsWith(_nonPageSubstringStarts) && !url.EndsWith(_nonPageSubstrings) && !url.Contains(_nonPageSubstrings.Select(x => x + "?"));
        }

        private static string ClearAnchor(string uri)
        {
            var index = uri.IndexOf('#');
            return index > 0 ? uri.Substring(0, index) : uri;
        }

        private static bool IsAbsolute(string url)
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