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
            var last = HtmlHelpers.GetMaxDepthNodes(rootNode, level).Select(GraphNode.GenerateTree).ToList();

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
    }
}
