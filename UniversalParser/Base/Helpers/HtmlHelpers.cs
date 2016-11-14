namespace Base.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using HtmlAgilityPack;

    public static class HtmlHelpers
    {
        public static IEnumerable<string> GetLinks(string html, string url, string onlyDomain = "")
        {
            var linq = GetHrefValues(html)
                .Select(x => UrlHelpers.CanonicalizePageLink(x, url))
                .Where(link => !string.IsNullOrEmpty(link));
            if (!string.IsNullOrWhiteSpace(onlyDomain)) linq = linq.Where(x => x.StartsWith(onlyDomain));
            return linq.Distinct();
        }

        private static IEnumerable<string> GetHrefValues(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.DocumentNode
                .SelectNodes("//a[@href")
                .Select(link => link.GetAttributeValue("href", ""))
                .Distinct();
        }
    }
}