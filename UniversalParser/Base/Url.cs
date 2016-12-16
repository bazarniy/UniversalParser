namespace Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using Helpers;
    using Utilities;

    public sealed class Url
    {
        private const char PathDelimeterChar = '/';
        private const string PathDelimeterString = "/";
        private const char PathParamsDelimeterChar = '?';
        private const string PathParamsDelimeterString = "?";
        private const char PathAnchorDelimeterChar = '#';
        private const char ParamsDelimeterChar = '&';
        private const string ParamsDelimeterString = "&";
        private const string DomainPattern = @"https?:\/\/([\w\d-]*\.)?[\w\d-]*\.[\w]*";
        private static readonly Regex _domainRegex = new Regex(DomainPattern, RegexOptions.Compiled);
        private static readonly string[] _nonPageSubstrings = { ".jpg", ".jpeg", ".gif", ".png", ".pdf", ".xls", ".xlsx", ".rtf", ".zip", ".rar", ".7z", ".gz", ".bz" };
        private static readonly string[] _nonPageSubstringStarts = { "mailto:", "javascript:", "tel:" };

        public string Domain { get; private set; }
        public string Path { get; private set; }

        private IEnumerable<string> _params;

        public string Params
        {
            get { return string.Join(ParamsDelimeterString, _params.OrderBy(s => s)); }
        }

        public Url(string url, string domain = "")
        {
            if (domain.IsEmpty()) url.ThrowIfEmpty(nameof(url));

            url = DecodeUrl(url);

            Domain = ParseDomain(url) ?? ParseDomain(domain);
            Domain.ThrowIfEmpty(nameof(Domain), "invalid domain");

            Path = GetPath(url, Domain);
            _params = GetParams(url);
        }

        private Url()
        {
        }

        private static string GetPath(string url, string domain)
        {
            var result = url.RemoveRight(PathAnchorDelimeterChar)
                .RemoveRight(PathParamsDelimeterChar)
                .RemoveFirst(domain);

            if (result.EndsWith(PathDelimeterString)) result = result.TrimEnd(PathDelimeterChar);
            return !result.IsEmpty() ? result : PathDelimeterString;
        }

        private static IEnumerable<string> GetParams(string url)
        {
            if (!url.Contains(PathParamsDelimeterChar)) return Enumerable.Empty<string>();

            return url.RemoveRight(PathAnchorDelimeterChar)
                .RemoveLeft(PathParamsDelimeterChar)
                .RemoveRight(PathParamsDelimeterChar) //?dsfgd=12/?sdf=df => dsfgd=12
                .Split(new[] { ParamsDelimeterChar }, StringSplitOptions.RemoveEmptyEntries);
        }

        public Url LinkTo(string url)
        {
            url = DecodeUrl(url);

            if (url.IsEmpty() || !IsContent(url)) return this;
            if (ParseDomain(url) != null) return new Url(url);
            if (IsAbsolute(url)) return new Url(url, Domain);

            var result = new Url
            {
                Domain = Domain,
                Path = GetPath(url, Domain),
                _params = GetParams(url)
            };

            result.Path = Path + (result.Path != PathDelimeterString ? PathDelimeterString + result.Path : "");

            return result;
        }

        public override string ToString()
        {
            return $"{Domain}{Path}{(!Params.IsEmpty() ? PathParamsDelimeterString + Params : "")}";
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

        private static string DecodeUrl(string url)
        {
            return HttpUtility.HtmlDecode(Uri.UnescapeDataString(url));
        }

        private static bool IsAbsolute(string url)
        {
            return url.StartsWith(PathDelimeterString);
        }

        private static bool IsContent(string url)
        {
            return !url.StartsWith(_nonPageSubstringStarts) && !url.EndsWith(_nonPageSubstrings) && !url.Contains(_nonPageSubstrings.Select(x => x + PathParamsDelimeterString));
        }

        private static string ParseDomain(string url)
        {
            var domain = _domainRegex.Match(url).Value;
            return domain.IsEmpty() ? null : domain;
        }
    }
}