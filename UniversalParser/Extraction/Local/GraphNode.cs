using System;
using System.Collections.Generic;
using System.Linq;
using Base.Helpers;
using HtmlAgilityPack;

namespace Extraction.Local
{
    

    internal class GraphNode
    {
        private string[] _attributes;

        public string[] Attribtes => _attributes ?? (_attributes = OrigNode.GetClasses().ToArray());

        public List<GraphNode> Children = new List<GraphNode>();
        public HtmlNode OrigNode;

        public string Name => OrigNode.Name;

        public int Depth { get; }

        private double _weight;
        private readonly int _level; //с нуля

        public GraphNode(HtmlNode node)
            : this(node, 0)
        {
        }

        private GraphNode(HtmlNode node, int level)
        {
            OrigNode = node;
            _level = level;
            foreach (var innerNode in node.ChildNodes.Where(HtmlHelpers.IsElementNodeType))
            {
                Children.Add(new GraphNode(innerNode, level + 1));
            }
            if (Children.Any())
            {
                Depth = Children.Max(x => x.Depth);
            }
            Depth++;
            if (_level == 0)
            {
                Init();
            }
        }

        private void Init()
        {
            const double Normalization = 2;

            var levels = new Dictionary<int, int>();
            CountNodesByLevel(this, levels);

            var nodeWeightOnLevel = new List<double>();

            double levelWeight = 0;
            for (var i = 0; i < levels.Count - 1; i++)
            {
                levelWeight = Math.Pow(Normalization, -(i + 1));
                nodeWeightOnLevel.Add(levelWeight / levels[i]);
            }
            nodeWeightOnLevel.Add(levelWeight / levels[levels.Keys.Max()]);

            SetWeight(nodeWeightOnLevel.ToArray());
        }

        private static void CountNodesByLevel(GraphNode node, IDictionary<int, int> dict)
        {
            if (!dict.ContainsKey(node._level)) dict[node._level] = 0;

            dict[node._level]++;
            foreach (var nodeChild in node.Children)
            {
                CountNodesByLevel(nodeChild, dict);
            }
        }

        internal void SetWeight(double[] nodeWeightByLevel)
        {
            _weight = nodeWeightByLevel[_level];
            foreach (var child in Children)
            {
                child.SetWeight(nodeWeightByLevel);
            }
        }

        public double CompareTrees(GraphNode node)
        {
            var res = Comapre(node);
            if (res.Equals(0)) return 0;

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

            return maxNode != null && max > 0 ? maxNode : null;
        }

        private double Comapre(GraphNode node)
        {
            return Name == node.Name
                ? HtmlHelpers.CompareAttributeValues(Attribtes, node.Attribtes) * _weight
                : 0;
        }

        public override string ToString()
        {
            return $"{Name}[{string.Join(", ", Attribtes)}]d{Depth}c{Children.Count}w"+_weight.ToString("0.00");
        }
    }
}
