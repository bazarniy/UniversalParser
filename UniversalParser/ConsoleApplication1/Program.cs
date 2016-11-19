using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApplication1
{
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            var x= new TestNode(0);
            x.Children.AddRange(new []{new TestNode(1), new TestNode(1) , new TestNode(1) });
            x.Children[0].Children.AddRange(new[] { new TestNode(2), new TestNode(2), new TestNode(2) });
            x.Children[1].Children.AddRange(new[] { new TestNode(2), new TestNode(2) });
            x.Children[1].Children[0].Children.AddRange(new[] { new TestNode(3), new TestNode(3) });

            var res = x.CountNodesByLevel();

            Init(x);            
        }

        static void CountNodesByLevel(TestNode node, Dictionary<int, int> dict)
        {
            if (!dict.ContainsKey(node._level)) dict[node._level] = 0;

            dict[node._level]++;

            foreach (var nodeChild in node.Children)
            {
                CountNodesByLevel(nodeChild, dict);
            }

        }

        private static void Init(TestNode node)
        {
            const double Normalization = 2;

            var levels = new Dictionary<int, int>();
            CountNodesByLevel(node, levels);

            var nodeWeightOnLevel = new Dictionary<int, double>();

            foreach (var levelPair in levels)
            {
                var levelWeight = Math.Pow(Normalization, -(levelPair.Key + 1));
                nodeWeightOnLevel.Add(levelPair.Key, levelWeight/levelPair.Value);
            }

            /*double levelWeight = 0;
            for (var i = 0; i < levels.Count - 1; i++)
            {
                var level = i + 1;
                levelWeight = Math.Pow(Normalization, -level);
                nodeWeightOnLevel.Add(levelWeight / levels[i]);
            }
            nodeWeightOnLevel.Add(levelWeight / levels.Values.Last());*/

            //SetWeight(nodeWeightOnLevel.ToArray());
        }

        private class TestNode
        {
            public List<TestNode> Children = new List<TestNode>();
            public int _level;

            public TestNode(int level)
            {
                _level = level;
            }

            public List<int> CountNodesByLevel()
            {
                var result = new List<int>();

                if (Children.Any()) result.Add(Children.Count);

                foreach (var child in Children)
                {
                    var childLevelCount = child.CountNodesByLevel();
                    for (var i = 0; i < childLevelCount.Count; i++)
                    {
                        if (result.Count < i + 2)
                        {
                            result.Add(0);
                        }
                        result[i + 1] += childLevelCount[i];
                    }
                }

                if (_level == 0)
                {
                    result.Insert(0, 1);
                }

                return result;
            }
        }
    }
}
