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

        public static IEnumerable<Url> GetAllLinks(string html, Url url)
        {
            if (html.IsEmpty()) return Enumerable.Empty<Url>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.DocumentNode
                .SelectNodesSafe("//a[@href]")
                .Select(link => link.GetAttributeValue("href", ""))
                .Distinct()
                .Select(url.LinkTo)
                .Distinct();
        }

        public static IEnumerable<Url> GetAllLinks(string html, string url)
        {
            return GetAllLinks(html, Url.Create(url));
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
            if (node == null) return Enumerable.Empty<string>();
            var classValue = node.GetAttributeValue(ClassAttribute, null);
            return string.IsNullOrEmpty(classValue) ? Enumerable.Empty<string>() : classValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static bool HasAttribute(this HtmlNode node, string attributeName)
        {
            return node != null && node.HasAttributes && node.Attributes.Contains(attributeName);
        }

        public static bool HasAttributeValue(this HtmlNode node, string attributeName, string value)
        {
            return node != null && node.HasAttribute(attributeName) && node.Attributes[attributeName].Value == value;
        }

        public static bool HasAttributeValueCaseInsensitive(this HtmlNode node, string attributeName, string value)
        {
            return node != null && node.HasAttribute(attributeName) && node.Attributes[attributeName].Value.ToUpperInvariant() == value.ToUpperInvariant();
        }

        public static bool HasAttributeValue(this HtmlNode node, string attributeName, IEnumerable<string> value)
        {
            return node != null && node.HasAttribute(attributeName) && value.Any(x => x == node.Attributes[attributeName].Value);
        }
        public static bool HasAttributeValueCaseInsensitive(this HtmlNode node, string attributeName, IEnumerable<string> value)
        {
            return node != null && node.HasAttribute(attributeName) && value.Any(x => x.ToUpperInvariant() == node.Attributes[attributeName].Value.ToUpperInvariant());
        }

        public static bool HasClassValue(this HtmlNode node, string value)
        {
            return node != null && node.HasAttribute(ClassAttribute) && node.Attributes[ClassAttribute].Value.Contains(value);
        }

        public static bool HasClassValue(this HtmlNode node, IEnumerable<string> value)
        {
            return node != null && node.HasAttribute(ClassAttribute) && value.Any(x => node.Attributes[ClassAttribute].Value.Contains(x));
        }

        public static HtmlNode GetBody(this HtmlDocument html)
        {
            return html.DocumentNode.Descendants(BodyTag).FirstOrDefault();
        }

        public static HtmlNode GetBody(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            return htmlDoc.DocumentNode.Descendants(BodyTag).FirstOrDefault();
        }

        public static IEnumerable<HtmlNode> GetAllTags(this HtmlNode node)
        {
            return node.Descendants().Where(IsElementNodeType);
        }

        public static bool IsElementNodeType(this HtmlNode node)
        {
            return node.NodeType == HtmlNodeType.Element;
        }

        public static IEnumerable<HtmlNode> GetAllTags(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var body = htmlDoc.GetBody();

            return body == null ? Enumerable.Empty<HtmlNode>() : body.GetAllTags();
        }

        public static IEnumerable<HtmlNode> GetMaxDepthNodes(HtmlNode rootNode, int depth)
        {
            var last = rootNode.GetAllTags().Where(x => x.HasChildNodes == false);
            for (var i = 0; i < depth; i++)
            {
                last = last.Select(x => x.ParentNode).Where(IsElementNodeType).Distinct();
            }
            return last;
        }

        public static double CompareAttributeValues(IEnumerable<string> attr1, IEnumerable<string> attr2)
        {
            var a1 = attr1.ToArray();
            var a2 = attr2.ToArray();

            var union = a1.Union(a2).Distinct().Count();

            return union != 0 ? a1.Intersect(a2).Count() / union : 0; //пересечение/объединение
        }

        public static int GetColspan(this HtmlNode node)
        {
            return node.GetAttributeValue("colspan", 1);
        }

        public static int GetRowspan(this HtmlNode node)
        {
            return node.GetAttributeValue("rowspan", 1);
        }

        public static IEnumerable<HtmlNode> SelectNodesSafe(this HtmlNode node, string xpath)
        {
            return node.SelectNodes(xpath) ?? Enumerable.Empty<HtmlNode>();
        }
    }
}