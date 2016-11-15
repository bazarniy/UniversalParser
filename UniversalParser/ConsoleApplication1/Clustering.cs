using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    using System.Collections.Concurrent;
    using Base.Helpers;
    using Base.Utilities;
    using HtmlAgilityPack;
    using XPath;


    /*public class Clusterization
    {
        public XPath.XPath GetContentNode(Catalog project, int percentFilter = 20)
        {
            var gl = this.GetGlobalContentNodes(project, percentFilter);
            var y = gl
                .Where(pair => pair.Value.InnerP && pair.Value.InnerH.Any(x => x) && pair.Value.InnerTable)
                .Select(x => x.Key)
                .ToList();
            var max = y.Max(x => x.Nodes.Count);
            return y.First(x => x.Nodes.Count == max);
        }

        private Dictionary<XPath.XPath, MinMaxPair> GetGlobalContentNodes(Catalog project, int percentFilter = 20)
        {
            var ret = new ConcurrentDictionary<XPath.XPath, MinMaxPair>();

            Parallel.ForEach(project.Files, fileInfo =>
            {
                var html = new HtmlDocument();
                using (var fileWrapper = project.GetFile(fileInfo))
                {
                    html.Load(fileWrapper.GetStream(false));
                }

                var xpathInfos = this.GetMaxContentNodes(html.DocumentNode.Descendants(HtmlHelpers.BodyTag).FirstOrDefault(), fileInfo.FileName);
                foreach (var xpathInfo in xpathInfos)
                {
                    ret.AddOrUpdate(xpathInfo.Key, xpathInfo.Value, (key, oldValue) => oldValue.Merge(xpathInfo.Value));
                }
            });

            var sc = project.Files.Count();
            return ret
                .Where(pair => pair.Value.Num * 100 / sc >= percentFilter) // процент содержания
                .Where(pair => pair.Value.Min != pair.Value.Max) //min=max
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private Dictionary<XPath.XPath, MinMaxPair> GetMaxContentNodes(HtmlNode rootnode, string fileName)
        {
            var ret = new Dictionary<XPath.XPath, MinMaxPair>();
            if (rootnode == null)
            {
                return ret;
            }

            Func<HtmlNode, bool> filterChild = x =>
                x.NodeType == HtmlNodeType.Element &&
                !(x.ParentNode.Attributes.Contains(HtmlHelpers.ClassAttribute) &&
                (x.ParentNode.Attributes[HtmlHelpers.ClassAttribute].Value == "header" || x.ParentNode.Attributes[HtmlHelpers.ClassAttribute].Value == "footer")
                    );

            var stack = new Stack<HtmlNode>();
            stack.Push(rootnode);

            var manager = new XNodeManager();

            while (stack.Any())
            {
                var node = stack.Pop();
                var children = node.ChildNodes.Where(x => filterChild(x)).ToArray();
                foreach (var pair in this.GetMaxLengthNodes(children))
                {
                    stack.Push(pair.Key);
                    ret.Add(
                        manager.GetXPath(pair.Key),
                        new MinMaxPair
                        {
                            Length = pair.Value,
                            Min = pair.Value,
                            Max = pair.Value,
                            MaxFile = fileName,
                            InnerH = pair.Key.ConaintsDescendants(HtmlHelpers.HTags).ToArray(),
                            InnerP = pair.Key.ConaintsDescendants("p"),
                            InnerTable = pair.Key.ConaintsDescendants("table")
                        }
                        );
                }
            }
            return ret;
        }

        private IEnumerable<KeyValuePair<HtmlNode, int>> GetMaxLengthNodes(HtmlNode[] children)
        {
            if (children == null || children.Length == 0)
            {
                return Enumerable.Empty<KeyValuePair<HtmlNode, int>>();
            }

            var cache = children.ToDictionary(node => node, node => node.GetClearTextLength());
            var max = cache.Values.Max();
            return cache.Where(x => x.Value == max);
        }



        private class MinMaxPair
        {
            public int Length = 0;
            public int Min = -1;
            public int Max = -1;
            public int Num = 0; //сколько раз встретил
            public string MaxFile = "";
            public bool[] InnerH = { false, false, false, false, false, false };
            public bool InnerP = false;
            public bool InnerTable = false;

            public MinMaxPair Merge(MinMaxPair newValue)
            {
                var result = CloneUtility.Clone(this);

                if (newValue.Length < result.Min)
                {
                    result.Min = newValue.Length;
                }
                else if (newValue.Length > result.Max)
                {
                    result.Max = newValue.Length;
                    result.MaxFile = newValue.MaxFile;
                }
                result.Num++;
                result.InnerH = newValue.InnerH;
                result.InnerP |= newValue.InnerP;
                result.InnerTable |= newValue.InnerTable;
                return result;
            }

            public override string ToString()
            {
                return string.Format("[{0}, {1}, {2}, {3}, {4}, {5}]", Min, Max, Num, this.InnerH.Any(x => x), InnerP, InnerTable);
            }
        }
    }*/
}
