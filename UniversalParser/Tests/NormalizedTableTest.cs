using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    using Extraction.Common;
    using HtmlAgilityPack;
    using NUnit.Framework;

    [TestFixture]
    public class NormalizedTableTest
    {
        //TODO: height weight control if needed
        //TODO: move out logic from constructor

        [Test]
        public void Ctor()
        {
            Assert.Throws<ArgumentNullException>(() => new NormalizedTable(null));
            var html=new HtmlDocument();
            html.LoadHtml("<p></p>");
            Assert.Throws<ArgumentException>(() => new NormalizedTable(html.DocumentNode.FirstChild));
            html.LoadHtml("<table><tr><td></td></tr></table>");
            Assert.DoesNotThrow(() => new NormalizedTable(html.DocumentNode.FirstChild));
        }

        [Test]
        public void RowspanColspan()
        {
            var html = new HtmlDocument();
            html.LoadHtml("<table><thead><tr><td colspan=\"2\" rowspan=\"2\">1</td><td>2</td></tr><tr><td>3</td><td>4</td></tr></thead><tr><td>3</td><td>4</td></tr><tbody><tr><td>3</td><td>4</td></tr></tbody></table>");
            Assert.DoesNotThrow(() => new NormalizedTable(html.DocumentNode.FirstChild));
        }

        [Test]
        public void Negative()
        {
            var html = new HtmlDocument();
            html.LoadHtml("<table></table>");
            Assert.DoesNotThrow(() => new NormalizedTable(html.DocumentNode.FirstChild));
            html.LoadHtml("<table><tr></tr></table>");
            Assert.DoesNotThrow(() => new NormalizedTable(html.DocumentNode.FirstChild));
        }

        [Test]
        public void Transpose()
        {
            var html = new HtmlDocument();
            html.LoadHtml("<table><tr><td></td></tr></table>");
            var table = new NormalizedTable(html.DocumentNode.FirstChild);
            Assert.DoesNotThrow(() => table.Transpose());
        }
    }
}
