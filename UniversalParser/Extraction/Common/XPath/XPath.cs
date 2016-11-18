namespace Extraction.Common.XPath
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HtmlAgilityPack;

    public sealed class XPath
    {
        internal readonly XNode[] Nodes;
        private readonly string _xpathSceleton;

        internal XPath(HtmlNode node)
        {
            var result = new List<XNode>();
            
            var lnode = node;
            while (lnode != null && lnode.NodeType == HtmlNodeType.Element)
            {
                result.Insert(0, new XNode(lnode));
                lnode = lnode.ParentNode;
            }

            Nodes = result.ToArray();

            _xpathSceleton = GetXpath(false, false);
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

        public bool HasChild(XPath node)
        {
            
        }

        public override string ToString()
        {
            return _xpathSceleton;
        }

        public override bool Equals(object obj)
        {
            var x = obj as XPath;
            return x != null && x._xpathSceleton == _xpathSceleton && x.Nodes.SequenceEqual(Nodes);
        }

        public override int GetHashCode()
        {
            return _xpathSceleton.GetHashCode(); // ^ b.GetHashCode();
        }
    }
}