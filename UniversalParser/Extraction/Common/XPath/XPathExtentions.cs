namespace Extraction.Common.XPath
{
    using System.Collections.Generic;
    using System.Linq;

    public static class XPathExtentions
    {
        public static IEnumerable<XPath> GetChildrenFrom(this XPath rootNode, IEnumerable<XPath> collection)
        {
            return collection.Where(rootNode.HasChild);
        }
    }
}