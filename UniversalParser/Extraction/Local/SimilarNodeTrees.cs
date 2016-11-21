namespace Extraction.Local
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Base.Helpers;
    using Common;
    using HtmlAgilityPack;

    public class SimilarNodeTrees
    {
        private const double Normalization = 2;

        public static Dictionary<List<HtmlNode>, double> GetSimilarTrees(HtmlNode rootNode, int level, double minSimilarity = 0.5)
        {
            var groups = new List<SimilarityCollection>();
            var lastNodeTrees = HtmlHelpers.GetMaxDepthNodes(rootNode, level)
                .Select(GenerateTree);

            foreach (var nodeTree in lastNodeTrees)
            {
                var found = false;

                foreach (var group in groups)
                {
                    var c = nodeTree.CompareTrees(group.Value[0]);

                    if (c < minSimilarity) continue;

                    group.Value.Add(nodeTree);
                    group.SumSimilarity += c;
                    found = true;
                }

                if (found) continue;

                var newPair = new SimilarityCollection();
                newPair.Value.Add(nodeTree);
                groups.Add(newPair);
            }

            return groups
                .Where(p => p.Value.Count > 1)
                .ToDictionary(
                    pair => pair.Value.Select(x => x.OrigNode).ToList(),
                    pair => pair.Average
                );
        }

        internal static GraphNode GenerateTree(HtmlNode htmlNode, int level = 0)
        {
            var node = new GraphNode(htmlNode, level);
            foreach (var innerHtmlNode in htmlNode.ChildNodes.Where(HtmlHelpers.IsElementNodeType))
                node.Children.Add(GenerateTree(innerHtmlNode, level));

            if (level == 0) SetTreeWeight(node);
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
                nodeWeightOnLevel.Add(levelWeight/levels[i]);
            }
            nodeWeightOnLevel.Add(levelWeight/levels[levels.Keys.Max()]);

            SetWeight(node, nodeWeightOnLevel.ToArray());
        }

        private static void CountNodesByLevel(GraphNode node, IDictionary<int, int> dict)
        {
            if (!dict.ContainsKey(node.Level)) dict[node.Level] = 0;

            dict[node.Level]++;
            foreach (var nodeChild in node.Children)
                CountNodesByLevel(nodeChild, dict);
        }

        private static void SetWeight(GraphNode node, IList<double> nodeWeightByLevel)
        {
            node.Weight = nodeWeightByLevel[node.Level];
            foreach (var child in node.Children)
                SetWeight(child, nodeWeightByLevel);
        }

        private class SimilarityCollection
        {
            public readonly List<GraphNode> Value = new List<GraphNode>();
            public double SumSimilarity;

            public double Average => Value.Count != 0 ? SumSimilarity/Value.Count : 0;
        }
    }
}