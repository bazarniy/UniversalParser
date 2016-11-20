using System;
using System.Collections.Generic;
using System.Linq;
using Base.Helpers;
using HtmlAgilityPack;

namespace Extraction.Local
{
    public static class SimilarNodeSearcher
    {
        private class Pair
        {
            public double Key = 0;
            public readonly List<GraphNode> Value = new List<GraphNode>();
        }

        /// <summary>
        /// Searches similar nodes in HtmlNode
        /// </summary>
        /// <param name="rootNode">documentRoot</param>
        /// <param name="level">depth of comaping nodes (глубина с конца)</param>
        /// <param name="minSimilarity">min similarity coefficient value</param>
        /// <returns></returns>
        public static Dictionary<List<HtmlNode>, double> SearchSimilarNodes(HtmlNode rootNode, int level, double minSimilarity = 0.5)
        {
            //TODO: погуглить на что можно заменить (надо keyValuePair с изменяемым ключем)
            var groups = new List<Pair>();
            var last = HtmlHelpers.GetMaxDepthNodes(rootNode, level).Select(GenerateTree).ToList();

            foreach (var node in last)
            {
                var found = false;

                //compare with first node from group
                foreach (var group in groups)
                {
                    var c = node.CompareTrees(group.Value[0]);
                    if (c >= minSimilarity)
                    {
                        group.Value.Add(node);
                        group.Key += c;
                        found = true;
                    }
                }

                if (!found)
                {
                    var newPair = new Pair();
                    newPair.Value.Add(node);
                    groups.Add(newPair);
                }
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
