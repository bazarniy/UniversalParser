namespace Extraction.Global
{
    using System.Collections.Generic;
    using System.Linq;
    using Base.Helpers;
    using HtmlAgilityPack;

    internal class ContentLengthMetric
    {
        private readonly Dictionary<string, int> _files = new Dictionary<string, int>();

        public ContentLengthMetric(KeyValuePair<HtmlNode, int> metric, string fileName)
        {
            var node = metric.Key;

            _files.Add(fileName, metric.Value);

            HasHTag = node.ConaintsDescendants(HtmlHelpers.HTags).Any(x => x);
            HasPTag = node.ConaintsDescendants("p");
            HasTableTag = node.ConaintsDescendants("table");
        }

        public int Min => _files.Values.Min();
        public int Max => _files.Values.Max();
        public int Num => _files.Count;

        public bool HasHTag { get; private set; }
        public bool HasPTag { get; private set; }
        public bool HasTableTag { get; private set; }

        public ContentLengthMetric Merge(ContentLengthMetric newValue)
        {
            HasHTag |= newValue.HasHTag;
            HasPTag |= newValue.HasPTag;
            HasTableTag |= newValue.HasTableTag;

            foreach (var file in newValue._files)
                _files[file.Key] = file.Value;
            return this;
        }

        public bool IsEquivalent(ContentLengthMetric metric)
        {
            return Min == metric.Min && Max == metric.Max && HasHTag == metric.HasHTag && HasPTag == metric.HasPTag && HasTableTag == metric.HasTableTag;
        }

        public override string ToString()
        {
            return $"[{Min}, {Max}, {Num}, {HasHTag}, {HasPTag}, {HasTableTag}]";
        }
    }
}