using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Utilities
{
    using Helpers;
    using HtmlAgilityPack;

    public class HtmlSimplifier
    {
        public static readonly string[] GoodAttributes = 
        {
            "id", "class", "src", "href", "alt", "title", "name", "content", "value", "colspan", "rowspan", "target", "type", "action", "method"
        };

        public string Simplify(string html)
        {
            if (html.IsEmpty()) return "";

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            foreach (var scriptNode in doc.DocumentNode.SelectNodesSafe("//script|//noscript|//link|//comment()|//style").ToArray())
            {
                scriptNode.Remove();
            }

            foreach (var scriptNode in doc.DocumentNode.SelectNodesSafe("head//meta").ToArray())
            {
                if (!scriptNode.HasAttributeValueCaseInsensitive("name", new[] {"title", "keywords", "description"}))
                {
                    scriptNode.Remove();
                }
            }

            foreach (var scriptNode in doc.DocumentNode.Descendants())
            {
                foreach (var attribute in scriptNode.Attributes.ToArray())
                {
                    if (!GoodAttributes.Contains(attribute.Name))
                    {
                        attribute.Remove();
                    }
                }
            }

            return doc.DocumentNode.OuterHtml;

        }
    }
}
