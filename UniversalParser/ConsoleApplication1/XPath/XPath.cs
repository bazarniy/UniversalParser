namespace ConsoleApplication1.XPath
{
    using System.Text;

    public sealed class XPath
    {
        internal readonly XNode[] Nodes;
        public readonly string OriginalXpath;

        internal XPath(string originalXpath, XNode[] nodes)
        {
            OriginalXpath = originalXpath;
            Nodes = nodes;
        }

        public string GetXpath(bool useId, bool useClass)
        {
            var ret = new StringBuilder();
            foreach (var node in Nodes)
                ret.Append("/" + node.GetXpath(useId, useClass));
            return "/" + ret;
        }

        public override string ToString()
        {
            return GetXpath(false, false);
        }

        /*public override bool Equals(object obj)
        {
            var x = obj as XPath;
            return x != null && x.OriginalXpath == OriginalXpath && x.Xpath.SequenceEqual(Xpath);
        }

        public override int GetHashCode()
        {
            return this.OriginalXpath.GetHashCode(); // ^ b.GetHashCode();
        }*/
    }
}