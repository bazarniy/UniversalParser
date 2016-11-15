namespace ConsoleApplication1.XPath
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Base.Helpers;
    using HtmlAgilityPack;

    public sealed class XNodeManager
    {
        private readonly ConcurrentBag<XNode> _nodes = new ConcurrentBag<XNode>();

        private XNode GetNode(HtmlNode node)
        {
            return _nodes.GetOrAdd(x => x.Matches(node), () => new XNode(node));
        }

        public XPath GetXPath(HtmlNode node)
        {
            var result = new List<XNode>();

            var lnode = node;
            while (lnode != null && lnode.NodeType == HtmlNodeType.Element)
            {
                result.Insert(0, GetNode(lnode));
                lnode = lnode.ParentNode;
            }

            return new XPath(node.XPath, result.ToArray());
        }
    }
}
