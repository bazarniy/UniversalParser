using System;
using System.Collections.Generic;
using System.Linq;
using Base.Helpers;
using HtmlAgilityPack;

namespace Extraction.Local
{
    public class ReversePageNodes
    {
        private class SimilarityMetric
        {
            public double Key = 0;
            public readonly List<GraphNode> Value = new List<GraphNode>();
        }

        public Dictionary<List<HtmlNode>, double> SearchSimilarNodes(HtmlNode rootNode, int level, double minSimilarity = 0.5)
        {
            var groups = new List<SimilarityMetric>();
            var last = HtmlHelpers.GetMaxDepthNodes(rootNode, level).Select(GenerateTree);

            foreach (var node in last)
            {
                var found = false;

                foreach (var group in groups)
                {
                    var c = node.CompareTrees(group.Value[0]);
                    if (c < minSimilarity) continue;

                    group.Value.Add(node);
                    group.Key += c;
                    found = true;
                }

                if (found) continue;

                var newPair = new SimilarityMetric();
                newPair.Value.Add(node);
                groups.Add(newPair);
            }

            return groups
                .Where(p => p.Value.Count > 1)
                .ToDictionary(
                    pair => pair.Value.Select(x => x.OrigNode).ToList(),
                    pair => pair.Key / pair.Value.Count
                );
        }

        private const double Normalization = 2;

        public static GraphNode GenerateTree(HtmlNode htmlNode, int level = 0)
        {
            var node = new GraphNode(htmlNode, level);
            foreach (var innerHtmlNode in htmlNode.ChildNodes.Where(HtmlHelpers.IsElementNodeType))
            {
                node.Children.Add(GenerateTree(innerHtmlNode, level));
            }
            if (level == 0)
            {
                SetTreeWeight(node);
            }
            return node;
        }

        private static void SetTreeWeight(GraphNode node)
        {
            var levels = new Dictionary<int, int>();
            CountNodesByLevel(node, levels);

            var nodeWeightOnLevel = new List<double>();

            double levelWeight = 0;
            for (var i = 0; i < levels.Count - 1; i++)
            {
                levelWeight = Math.Pow(Normalization, -(i + 1));
                nodeWeightOnLevel.Add(levelWeight / levels[i]);
            }
            nodeWeightOnLevel.Add(levelWeight / levels[levels.Keys.Max()]);

            SetWeight(node, nodeWeightOnLevel.ToArray());
        }

        private static void CountNodesByLevel(GraphNode node, IDictionary<int, int> dict)
        {
            if (!dict.ContainsKey(node.Level)) dict[node.Level] = 0;

            dict[node.Level]++;
            foreach (var nodeChild in node.Children)
            {
                CountNodesByLevel(nodeChild, dict);
            }
        }

        private static void SetWeight(GraphNode node, IList<double> nodeWeightByLevel)
        {
            node.Weight = nodeWeightByLevel[node.Level];
            foreach (var child in node.Children)
            {
                SetWeight(child, nodeWeightByLevel);
            }
        }
    }
}
