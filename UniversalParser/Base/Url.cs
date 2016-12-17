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
        private const string ProtocolDelimeter = "://";
        private const string DomainPattern = @"https?:\/\/([\w\d-]*\.)?[\w\d-]*\.[\w]*";
        private static readonly Regex _domainRegex = new Regex(DomainPattern, RegexOptions.Compiled);
        private static readonly string[] _nonPageSubstrings = { ".jpg", ".jpeg", ".gif", ".png", ".pdf", ".xls", ".xlsx", ".rtf", ".zip", ".rar", ".7z", ".gz", ".bz", ".docx", ".dwg" };
        private static readonly string[] _nonPageSubstringStarts = { "mailto:", "javascript:", "tel:", "skype:", "fax:", "modem:" };

        public string Domain { get; private set; }
        public string Path { get; internal set; }

        private IEnumerable<string> _params;

        public string Params => string.Join(ParamsDelimeterString, _params.OrderBy(s => s));

        public static Url Create(string url, string domain = "")
        {
            if (domain.IsEmpty()) url.ThrowIfEmpty(nameof(url));

            url = DecodeUrl(url);

            var result = new Url {Domain = ParseDomain(url) ?? ParseDomain(domain)};
            result.Domain.ThrowIfEmpty(nameof(Domain), "invalid domain");

            result.Path = GetPath(url, result.Domain);
            if (!result.Path.IsEmpty() && !result.Path.StartsWith(PathDelimeterString)) result.Path = PathDelimeterString + result.Path;

            if (!IsContent(result.Path)) return null;

            result._params = GetParams(url);
            return result;
        }

        private static string GetPath(string url, string domain)
        {
            var result = url.RemoveRight(PathAnchorDelimeterChar)
                .RemoveRight(PathParamsDelimeterChar)
                .RemoveFirst(domain);

            return ResolvePathParent(result);
        }

        private static IEnumerable<string> GetParams(string url)
        {
            return url.Contains(PathParamsDelimeterChar)
                ? url.RemoveRight(PathAnchorDelimeterChar)
                    .RemoveLeft(PathParamsDelimeterChar)
                    .Split(new[] {ParamsDelimeterChar}, StringSplitOptions.RemoveEmptyEntries)
                : Enumerable.Empty<string>();
        }

        public Url LinkTo(string url)
        {
            url = DecodeUrl(url);

            if (url.IsEmpty()) return this;
            if (!IsContent(url)) return null;
            if (ParseDomain(url) != null) return Create(url);
            if (url.Contains(ProtocolDelimeter)) return null;
            if (url.StartsWith(PathDelimeterString)) return Create(url, Domain);

            var result = new Url
            {
                Domain = Domain,
                Path = GetPath(url, Domain),
                _params = GetParams(url)
            };

            result.Path = PathConcat(Path, result.Path);

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

        private static bool IsContent(string url)
        {
            return !url.Contains(_nonPageSubstringStarts) 
                && !url.EndsWith(_nonPageSubstrings) 
                && !url.Contains(_nonPageSubstrings.Select(x => x + PathParamsDelimeterString));
        }

        private static string ParseDomain(string url)
        {
            var domain = _domainRegex.Match(url).Value;
            return domain.IsEmpty() ? null : domain;
        }

        private static string ResolvePathParent(string path)
        {
            if (path.Contains("/./") || path.EndsWith("/."))
            {
                if (path.EndsWith("/./")) path = path.Substring(0, path.Length - 1);
                while (path.EndsWith("/."))
                {
                    path = path.Substring(0, path.Length - 2);
                }
                if (path.Length != 0 && !path.Contains("/./")) path += "/";

                while (path.Contains("/./"))
                {
                    path = path.Replace("/./", "/");
                }
            }
            while (path.Contains("/../"))
            {
                var index = path.IndexOf("/../", StringComparison.Ordinal);
                var indexDelimeter = path.Substring(0, index).LastIndexOf("/", StringComparison.Ordinal);
                path = path.Substring(0, indexDelimeter < 0 ? index : indexDelimeter) + "/" + path.Substring(index + 4);
            }
            while (path.EndsWith("/.."))
            {
                path = path.Substring(0, path.Length - 3).RemoveLastSegment("/");
            }

            return path;
        }

        private static string PathConcat(string path1, string path2)
        {
            if (path2 == PathDelimeterString || path2.IsEmpty()) return path1;

            if (!path1.EndsWith("/")) path1 = path1.RemoveLastSegment("/") + "/";

            return path1 + path2;
        }




    }
}