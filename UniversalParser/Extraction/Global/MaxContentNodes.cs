namespace Extraction.Global
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Base;
    using Base.Helpers;
    using Common.XPath;
    using HtmlAgilityPack;

    public class MaxContentNodes
    {
        private static readonly string[] _badClasses = {"header", "footer"};

        private readonly ConcurrentDictionary<XPath, ContentLengthMetric> _metricBag = new ConcurrentDictionary<XPath, ContentLengthMetric>();
        private readonly IEnumerable<DataInfo> _infos;
        private readonly int _percentFilter;

        public MaxContentNodes(IEnumerable<DataInfo> infos, int percentFilter = 20)
        {
            _infos = infos;
            _percentFilter = percentFilter;
        }

        public IEnumerable<KeyValuePair<XPath, ContentLengthMetric>> GetNodes()
        {
            EvaluateMetrics(_infos);
            var filtered = FilterDepthNodes(_metricBag.Keys);

            return FilterMetrics(filtered, _infos.Count(), _percentFilter).ToArray();
        }

        private void EvaluateMetrics(IEnumerable<DataInfo> infos)
        {
            Parallel.ForEach(infos, info =>
            {
                foreach (var metricPair in EvaluateDocumentMetric(info))
                {
                    _metricBag.AddOrUpdate(metricPair.Key, metricPair.Value, (key, value) => value.Merge(metricPair.Value));
                }
            });
        }

        private static IEnumerable<KeyValuePair<XPath, ContentLengthMetric>> EvaluateDocumentMetric(DataInfo info)
        {
            var root = HtmlHelpers.GetBody(info.Data);

            if (root == null) return Enumerable.Empty<KeyValuePair<XPath, ContentLengthMetric>>();

            FilterNodes(root);

            return root.GetAllTags()
                .ToDictionary(x => x, x => x.GetClearTextLength())
                .Where(x => x.Value > 0)
                .Select(x => new KeyValuePair<XPath, ContentLengthMetric>(
                    new XPath(x.Key),
                    new ContentLengthMetric(x, info.Url)));

            //.ToDictionary(x => new XPath(x.Key), x => new ContentLengthMetric(x, info.Url));
        }

        private IEnumerable<XPath> FilterDepthNodes(IEnumerable<XPath> nodeCollection)
        {
            var nodes = nodeCollection.ToArray();
            var nodesToRemove = new List<XPath>();

            var min = nodes.Min(x => x.NodeCount);
            var max = nodes.Max(x => x.NodeCount);

            for (var i = min; i <= max; i++)
                foreach (var rootNode in nodes.Where(x => (x.NodeCount == i) && !nodesToRemove.Contains(x)).ToArray())
                {
                    var rootMetric = _metricBag[rootNode];
                    var childrenMetric = rootNode.GetChildrenFrom(nodes).Select(x => _metricBag[x]);

                    if (childrenMetric.Any(rootMetric.IsEquivalent))
                        nodesToRemove.Add(rootNode);
                }

            return nodes.Except(nodesToRemove);
        }

        private IEnumerable<KeyValuePair<XPath, ContentLengthMetric>> FilterMetrics(IEnumerable<XPath> nodes, int documentCount, int percentFilter)
        {
            return _metricBag
                .Where(x => nodes.Contains(x.Key))
                .Where(x => x.Value.Num*100/documentCount >= percentFilter)
                .Where(x => x.Value.Min != x.Value.Max)
                .Where(x => x.Value.HasHTag && (x.Value.HasPTag || x.Value.HasTableTag));
        }

        private static int GetPercentage(double val)
        {
            return Convert.ToInt32(Math.Round(val));
        }

        private static void FilterNodes(HtmlNode node)
        {
            if (node == null) return;

            if (node.HasClassValue(_badClasses))
            {
                node.Remove();
                return;
            }

            foreach (var childNode in node.ChildNodes.ToArray())
            {
                if (childNode.HasClassValue(_badClasses))
                {
                    node.RemoveChild(childNode, false);
                    //childNode.Remove();
                    continue;
                }
                FilterNodes(childNode);
            }
        }
    }
}