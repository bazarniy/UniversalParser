namespace Extraction.Local
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Base.Helpers;
    using HtmlAgilityPack;

    internal class GraphNode
    {
        public readonly int Level;

        public List<GraphNode> Children = new List<GraphNode>();
        public HtmlNode OrigNode;

        public GraphNode(HtmlNode node, int level)
        {
            OrigNode = node;
            Level = level;
        }

        private string[] _attributes;
        public string[] Attribtes => _attributes ?? (_attributes = OrigNode.GetClasses().ToArray());

        public string Name => OrigNode.Name;

        public double Weight { get; set; }

        public double CompareTrees(GraphNode node)
        {
            var res = Comapre(node);
            if (Math.Abs(res) < double.Epsilon) return 0;

            var foundNodes = new List<GraphNode>();
            foreach (var child1 in Children)
            {
                var maxNode = FindSimilarNode(child1, node.Children.Where(x => !foundNodes.Contains(x)));
                if (maxNode == null) continue;

                res += child1.CompareTrees(maxNode);
                foundNodes.Add(maxNode);
            }

            return res;
        }

        private static GraphNode FindSimilarNode(GraphNode mainNode, IEnumerable<GraphNode> nodeCollection)
        {
            double max = 0;
            GraphNode maxNode = null;

            foreach (var node in nodeCollection)
            {
                var compareResult = mainNode.Comapre(node);

                if (compareResult < max) continue;

                max = compareResult;
                maxNode = node;
            }

            return (maxNode != null) && (max > 0) ? maxNode : null;
        }

        private double Comapre(GraphNode node)
        {
            return Name == node.Name
                ? HtmlHelpers.CompareAttributeValues(Attribtes, node.Attribtes)*Weight
                : 0;
        }

        public override string ToString()
        {
            return $"{Name}[{string.Join(", ", Attribtes)}]d{Level}c{Children.Count}w" + Weight.ToString("0.00");
        }
    }
}