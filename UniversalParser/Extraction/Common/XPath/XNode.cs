namespace Extraction.Common.XPath
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Base.Helpers;
    using HtmlAgilityPack;

    internal sealed class XNode
    {
        public XNode(string name, string id, IEnumerable<string> classes)
        {
            Name = name;
            Id = id;
            Class = classes.ToArray();
            Array.Sort(Class);
        }

        public XNode(HtmlNode node) : this(node.Name, node.Id, node.GetClasses())
        {
        }

        public string Name { get; }
        public string Id { get; }
        public string[] Class { get; }

        public string GetXpath(bool useId, bool useClass)
        {
            var id = useId ? GetXPathIdParam() : "";
            var cl = useClass ? GetXPathClassParam() : "";

            var attr = "";
            if (!id.IsEmpty() && !cl.IsEmpty())
            {
                attr = $"[{id} and {cl}]";
            }
            else if (!id.IsEmpty())
            {
                attr = $"[{id}]";
            }
            else if (!cl.IsEmpty())
            {
                attr = $"[{cl}]";
            }

            return Name + attr;
        }

        private string GetXPathIdParam()
        {
            return !string.IsNullOrEmpty(Id) ? $"@id='{Id}'" : string.Empty;
        }

        private string GetXPathClassParam()
        {
            return Class.Length > 0 ? string.Join(" and ", Class.Select(x => $"contains(@class, '{x}')")) : string.Empty;
        }

        public override string ToString()
        {
            return $"{Name}.{Id} [{string.Join(", ", Class)}]";
        }

        public bool Matches(HtmlNode node)
        {
            return node != null && Name == node.Name && Id == node.Id && Class.ScrambledEquals(node.GetClasses());
        }

        public override bool Equals(object obj)
        {
            var node = obj as XNode;
            return node != null && Name == node.Name && Id == node.Id && Class.SequenceEqual(node.Class);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Id.GetHashCode();
        }
    }
}