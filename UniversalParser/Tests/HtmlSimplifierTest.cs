namespace Tests
{
    using Base.Utilities;
    using NUnit.Framework;

    [TestFixture]
    public class HtmlSimplifierTest
    {
        private HtmlSimplifier _simplifier=> new HtmlSimplifier();

        [Test]
        public void EmptyParam()
        {
            Assert.DoesNotThrow(() => _simplifier.Simplify(null));
            Assert.DoesNotThrow(() => _simplifier.Simplify(""));
        }

        [Test]
        public void RemoveComments()
        {
            const string text = "<html><td><!-- Yandex.Metrika counter --><!-- /Yandex.Metrika counter --></td></html>";

            var result = _simplifier.Simplify(text);
            Assert.IsTrue(result.Contains("td"));
            Assert.IsFalse(result.Contains("<!--"));
        }

        [Test]
        public void RemoveStyle()
        {
            const string text = "<html><td><style>abc</style><style src=\"1.css\" /></td></html>";

            var result = _simplifier.Simplify(text);
            Assert.IsTrue(result.Contains("td"));
            Assert.IsFalse(result.Contains("<style"));
        }

        [Test]
        public void RemoveScript()
        {
            const string text = "<html><td><script>abc</script><script type=\"text/javascript\" src=\"1.js\" /></td></html>";

            var result = _simplifier.Simplify(text);
            Assert.IsTrue(result.Contains("td"));
            Assert.IsFalse(result.Contains("<script"));
        }

        [Test]
        public void RemoveNoscript()
        {
            const string text = "<html><td><noscript>abc</script></td></html>";

            var result = _simplifier.Simplify(text);
            Assert.IsTrue(result.Contains("td"));
            Assert.IsFalse(result.Contains("<noscript"));
        }

        [Test]
        public void RemoveLink()
        {
            const string text = "<html><td><link href=\"123\" /></td></html>";

            var result = _simplifier.Simplify(text);
            Assert.IsTrue(result.Contains("td"));
            Assert.IsFalse(result.Contains("<link"));
        }

        [Test]
        public void RemoveMetaInHead()
        {
            const string text = "<html><head><meta content=\"text/html; charset=windows-1251\"><meta name=\"cmsmagazine\" content=\"4c547883b1fca0f4fb69c5ffab088159\"><meta name =\"Keywords\" content=\"22\"/><meta name=\"Description\" content=\"11\" /><meta name=\"title\" content=\"33\"/><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" /></head></html>";

            var result = _simplifier.Simplify(text);
            Assert.IsTrue(result.Contains("<meta name=\"title\""));
            Assert.IsTrue(result.Contains("<meta name=\"Keywords\""));
            Assert.IsTrue(result.Contains("<meta name=\"Description\""));
            Assert.IsFalse(result.Contains("<meta http"));
            Assert.IsFalse(result.Contains("cmsmagazine"));
            Assert.IsFalse(result.Contains("text/html"));
        }

        [Test]
        public void RemoveAttributes()
        {
            const string text = "<html><form action=\"/search/\" method=\"get\" name=\"searchForm\" id=\"searchForm\"></form><meta name=\"Keywords\" content=\"22\" value=\"\"/><td colspan=2 rowspan=2 class=\"123\" id=\"test1\" style=\"background-color: #2b4244;\"><a target=\"_blank\" href=\"\" title=\"\"><img style=\"background-color: #2b4244;\" src=\"\" alt=\"\"></a></td><input type=\"text\" name=\"q\" value=\"поиск\" placeholder=\"поиск\" class=\"search\" style=\"height: 20px; width: 175px; box-sizing: border-box;\" onFocus=\"this.value=\'\';\"></html>";

            var result = _simplifier.Simplify(text);
            foreach (var goodAttribute in HtmlSimplifier.GoodAttributes)
            {
                Assert.IsTrue(result.Contains($"{goodAttribute}="));
            }

            Assert.IsFalse(result.Contains("style="));
            Assert.IsFalse(result.Contains("placeholder="));
            Assert.IsFalse(result.Contains("onFocus="));
        }

        [Test]
        public void RemoveEmptyRows()
        {
            const string text = "<head>\r\n\r\n" +
                                "<meta name =\"Keywords\" content=\"22\"/>\r\n\n" +
                                "<meta name=\"Description\" content=\"11\" />\n\n" +
                                "<meta name=\"title\" content=\"33\"/>" +
                                "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />\n\n\n\r\n\n\n\n\n\n" +
                                "</head>";

            var result = _simplifier.Simplify(text);
            Assert.IsFalse(result.Contains("\r\n\r\n"));
            Assert.IsFalse(result.Contains("\r\n\n"));
            Assert.IsFalse(result.Contains("\n\n"));
        }
    }
}
