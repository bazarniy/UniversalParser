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
    }

    public class Clusterization
    {
        public XPath.XPath GetContentNode(IDataReader project, int percentFilter = 20)
        {
            var gl = GetGlobalContentNodes(project, percentFilter);
            var y = gl
                .Where(pair => pair.Value.HasPTag && pair.Value.HasHTag && pair.Value.HasTableTag)
                .Select(x => x.Key)
                .ToList();
            var max = y.Max(x => x.NodeCount);
            return y.First(x => x.NodeCount == max);
        }

        private Dictionary<XPath.XPath, MinMaxPair> GetGlobalContentNodes(IDataReader storage, int percentFilter = 20)
        {
            var ret = new ConcurrentDictionary<XPath.XPath, MinMaxPair>();

            Parallel.ForEach(storage.GetFileNames(), fileName =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(storage.GetFile(fileName).Data);

                foreach (var xpathInfo in GetMaxContentNodes(html, fileName))
                {
                    ret.AddOrUpdate(xpathInfo.Key, xpathInfo.Value, (key, oldValue) => oldValue.Merge(xpathInfo.Value));
                }
            });

            var sc = storage.GetFileNames().Count();
            return ret
                .Where(pair => pair.Value.Num * 100 / sc >= percentFilter) // процент содержания
                .Where(pair => pair.Value.Min != pair.Value.Max) //min=max
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private Dictionary<XPath.XPath, MinMaxPair> GetMaxContentNodes(HtmlDocument html, string fileName)
        {
            var rootnode = html.GetBody();

            var ret = new Dictionary<XPath.XPath, MinMaxPair>();
            if (rootnode == null)
            {
                return ret;
            }

            var manager = new XNodeManager();
            var filteredChildren = rootnode
                .GetAllTags()
                .Where(FilterChildren)
                .GetMax(x => x.GetClearTextLength());

            foreach (var pair in filteredChildren)
            {
                ret.Add(
                    manager.GetXPath(pair.Key),
                    new MinMaxPair
                    {
                        Length = pair.Value,
                        Min = pair.Value,
                        Max = pair.Value,
                        MaxFile = fileName,
                        InnerH = pair.Key.ConaintsDescendants(HtmlHelpers.HTags).ToArray(),
                        HasPTag = pair.Key.ConaintsDescendants("p"),
                        HasTableTag = pair.Key.ConaintsDescendants("table")
                    }
                    );
            }

            return ret;
        }

        private static bool FilterChildren(HtmlNode node)
        {
            return node.NodeType == HtmlNodeType.Element && !node.HasClassValue(new[] { "header", "footer"});
        }


        private class MinMaxPair
        {
            public int Length = 0;
            public int Min = -1;
            public int Max = -1;
            public int Num = 0; //сколько раз встретил
            public string MaxFile = "";
            public bool[] InnerH = { false, false, false, false, false, false };
            public bool HasHTag => InnerH.Any(x => x);
            public bool HasPTag = false;
            public bool HasTableTag = false;

            public MinMaxPair Merge(MinMaxPair newValue)
            {
                if (newValue.Length < Min)
                {
                    Min = newValue.Length;
                }
                else if (newValue.Length > Max)
                {
                    Max = newValue.Length;
                    MaxFile = newValue.MaxFile;
                }
                Num++;
                InnerH = newValue.InnerH;
                HasPTag |= newValue.HasPTag;
                HasTableTag |= newValue.HasTableTag;
                return this;
            }

            public override string ToString()
            {
                return string.Format("[{0}, {1}, {2}, {3}, {4}, {5}]", Min, Max, Num, HasHTag, HasPTag, HasTableTag);
            }
        }
    }
}
