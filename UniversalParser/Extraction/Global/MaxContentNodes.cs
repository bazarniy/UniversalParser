namespace Extraction.Global
{
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
        private readonly IDataReader _storage;
        private readonly int _percentFilter;

        public MaxContentNodes(IDataReader storage, int percentFilter = 20)
        {
            _storage = storage;
            _percentFilter = percentFilter;
        }

        public IEnumerable<XPath> GetNodes()
        {
            EvaluateMetrics(_storage.GetInfos());
            var filtered = FilterDepthNodes(_metricBag.Keys);
            filtered = FilterMetrics(filtered, _storage.Count(), _percentFilter);

            return filtered;
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

        private static Dictionary<XPath, ContentLengthMetric> EvaluateDocumentMetric(DataInfo info)
        {
            var root = HtmlHelpers.GetBody(info.Data);
            FilterNodes(root);

            return root.GetAllTags()
                .ToDictionary(x => x, x => x.GetClearTextLength())
                .Where(x => x.Value > 0)
                .ToDictionary(x => new XPath(x.Key), x => new ContentLengthMetric(x, info.Url));
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

        private IEnumerable<XPath> FilterMetrics(IEnumerable<XPath> nodes, int documentCount, int percentFilter)
        {
            return _metricBag
                .Where(x => nodes.Contains(x.Key))
                .Where(x => x.Value.Num*100/documentCount >= percentFilter)
                .Where(x => x.Value.Min != x.Value.Max)
                .Where(x => x.Value.HasHTag && (x.Value.HasPTag || x.Value.HasTableTag))
                .Select(x => x.Key);
        }

        private static void FilterNodes(HtmlNode node)
        {
            if (node.HasClassValue(_badClasses))
            {
                node.Remove();
                return;
            }

            foreach (var childNode in node.ChildNodes)
            {
                if (childNode.HasClassValue(_badClasses))
                {
                    childNode.Remove();
                    continue;
                }
                FilterNodes(childNode);
            }
        }
    }
}