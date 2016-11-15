namespace Base.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HtmlAgilityPack;

    public static class HtmlHelpers
    {
        public static string[] HTags = { "h1", "h2", "h3", "h4", "h5", "h6" };
        public const string BodyTag = "body";
        public const string ClassAttribute = "class";

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

        public static bool ConaintsDescendants(this HtmlNode node, string tag)
        {
            return node.Descendants(tag).Any();
        }

        public static IEnumerable<bool> ConaintsDescendants(this HtmlNode node, IEnumerable<string> tags)
        {
            return tags.Select(node.ConaintsDescendants);
        }

        public static int GetClearTextLength(this HtmlNode node)
        {
            return node.InnerText.ClearText().Length;
        }

        public static IEnumerable<string> GetClasses(this HtmlNode node)
        {
            var classValue = node.GetAttributeValue(ClassAttribute, null);
            return string.IsNullOrEmpty(classValue) ? Enumerable.Empty<string>() : classValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static bool HasAttribute(this HtmlNode node, string attributeName)
        {
            return node.HasAttributes && node.Attributes.Contains(attributeName);
        }

        public static bool HasAttributeValue(this HtmlNode node, string attributeName, string value)
        {
            return node.HasAttribute(attributeName) && node.Attributes[attributeName].Value == value;
        }

        public static bool HasAttributeValue(this HtmlNode node, string attributeName, IEnumerable<string> value)
        {
            return node.HasAttribute(attributeName) && value.Any(x => x == node.Attributes[attributeName].Value);
        }

        public static bool HasClassValue(this HtmlNode node, string value)
        {
            return node.HasAttribute(ClassAttribute) && node.Attributes[ClassAttribute].Value.Contains(value);
        }

        public static bool HasClassValue(this HtmlNode node, IEnumerable<string> value)
        {
            return node.HasAttribute(ClassAttribute) && value.Any(x => node.Attributes[ClassAttribute].Value.Contains(x));
        }

        public static HtmlNode GetBody(this HtmlDocument html)
        {
            return html.DocumentNode.Descendants(BodyTag).FirstOrDefault();
        }

        public static IEnumerable<HtmlNode> GetAllTags(this HtmlNode node)
        {
            return node.Descendants().Where(x => x.NodeType == HtmlNodeType.Element);
        }
    }
}