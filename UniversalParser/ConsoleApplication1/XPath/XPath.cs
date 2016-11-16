namespace ConsoleApplication1.XPath
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HtmlAgilityPack;

    public sealed class XPath
    {
        internal readonly XNode[] Nodes;
        public readonly string OriginalXpath;

        internal XPath(string originalXpath, XNode[] nodes)
        {
            OriginalXpath = originalXpath;
            Nodes = nodes;
        }

        internal XPath(HtmlNode node)
        {
            var result = new List<XNode>();
            OriginalXpath = node.XPath;

            var lnode = node;
            while (lnode != null && lnode.NodeType == HtmlNodeType.Element)
            {
                result.Insert(0, new XNode(lnode));
                lnode = lnode.ParentNode;
            }

            Nodes = result.ToArray();
        }

        public IEnumerable<string> Classes
        {
            get { return Nodes.SelectMany(x => x.Class); }
        }

        public int NodeCount => Nodes.Length;

        public string GetXpath(bool useId, bool useClass)
        {
            var ret = new StringBuilder();
            foreach (var node in Nodes)
                ret.Append("/" + node.GetXpath(useId, useClass));
            return "/" + ret;
        }

        public override string ToString()
        {
            return GetXpath(false, false);
        }

        public override bool Equals(object obj)
        {
            var x = obj as XPath;
            return x != null && x.OriginalXpath == OriginalXpath && x.Nodes.SequenceEqual(Nodes);
        }

        public override int GetHashCode()
        {
            return this.OriginalXpath.GetHashCode(); // ^ b.GetHashCode();
        }
    }
}