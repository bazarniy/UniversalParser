using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    using System.Collections.Concurrent;
    using System.IO;
    using Base;
    using Base.Helpers;
    using Base.Utilities;
    using HtmlAgilityPack;
    using XPath;

    public interface IDataReader
    {
        IEnumerable<string> GetFileNames();
        DataInfo GetFile(string file);

        IEnumerable<DataInfo> GetInfos();
    }

    public class Clusterization
    {
        public XPath.XPath GetContentNode(IDataReader storage, int percentFilter = 20)
        {
            var sc = storage.GetFileNames().Count();
            return GetAllNodesMaxText(storage.GetInfos())
                .Where(x => x.Num*100/sc >= percentFilter) // процент содержания
                .Where(x => x.Min != x.Max) //min=max
                .Where(x => x.HasPTag && x.HasHTag && x.HasTableTag)
                .Where(x => x.XPath.Classes.All(IsGoodClass))
                .Select(x => x.XPath)
                .GetMax(x => x.NodeCount)
                .Select(x => x.Key)
                .FirstOrDefault();
        }

        private static IEnumerable<ContentLengthMetric> GetAllNodesMaxText(IEnumerable<DataInfo> infos)
        {
            var ret = new ConcurrentBag<ContentLengthMetric>();

            Parallel.ForEach(infos, info =>
            {
                var metrics = HtmlHelpers.GetAllTags(info.Data)
                    .GetMax(x => x.GetClearTextLength())
                    .Select(x => new ContentLengthMetric(x, info.Url));

                foreach (var xpathInfo in metrics)
                {
                    var result = ret.FirstOrDefault(x => x.XPath.Equals(xpathInfo.XPath));
                    if (result != null)
                    {
                        result.Merge(xpathInfo);
                    }
                    else
                    {
                        ret.Add(xpathInfo);
                    }
                }
            });

            return ret;
        }

        private static bool IsGoodClass(string htmlclass)
        {
            return htmlclass != "header" && htmlclass != "footer";
        }


        private class ContentLengthMetric
        {
            public readonly XPath.XPath XPath;
            private readonly Dictionary<string, int> _files = new Dictionary<string, int>();
            private readonly int ContentLength;
            public int Min;
            public int Max;
            public int Num => _files.Count;

            public bool HasHTag { get; private set; }
            public bool HasPTag { get; private set; }
            public bool HasTableTag { get; private set; }

            public ContentLengthMetric(KeyValuePair<HtmlNode, int> metric, string fileName)
            {
                var node = metric.Key;
                var length = metric.Value;

                XPath = new XPath.XPath(node);

                _files.Add(fileName, length);

                ContentLength = length;
                Min = length;
                Max = length;
                HasHTag = node.ConaintsDescendants(HtmlHelpers.HTags).Any(x => x);
                HasPTag = node.ConaintsDescendants("p");
                HasTableTag = node.ConaintsDescendants("table");
            }

            public void Merge(ContentLengthMetric newValue)
            {
                if (newValue.ContentLength < Min)
                {
                    Min = newValue.ContentLength;
                }
                else if (newValue.ContentLength > Max)
                {
                    Max = newValue.ContentLength;
                }

                HasHTag |= newValue.HasHTag;
                HasPTag |= newValue.HasPTag;
                HasTableTag |= newValue.HasTableTag;

                foreach (var file in newValue._files)
                {
                    _files[file.Key] = file.Value;
                }
            }

            public override string ToString()
            {
                return $"[{Min}, {Max}, {Num}, {HasHTag}, {HasPTag}, {HasTableTag}]";
            }
        }
    }
}
