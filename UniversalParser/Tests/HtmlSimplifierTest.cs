using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void SimplifyHead()
        {
            var text =
                "<head><meta name=\"Keywords\" content=\"22\"/><meta name=\"Description\" content=\"11\" /><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" /><title>Кабельные маркеры МКН: теперь в промышленной упаковке Группа компаний IEK</title><link rel=\"shortcut icon\" href=\"/favicon.ico\" type=\"image/x-icon\"><link rel=\"alternate\"type=\"application/rss+xml\" title=\"Новости компании ИЭК\" href=\"http://www.iek.ru/press/1/rss.php\"><meta name=\"cmsmagazine\" content=\"4c547883b1fca0f4fb69c5ffab088159\" /><meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1251\"/><link href=\"/bitrix/cache/css/ru/innerpage/kernel_main/kernel_main.css?148179925545098\" type=\"text/css\"  rel=\"stylesheet\" /><script type=\"text/javascript\"src=\"/bitrix/cache/js/ru/innerpage/kernel_main/kernel_main.js?1481799255269754\"></script><script type=\"text/javascript\">$(function(){$(\"BODY\").addClass(\"hideSelects\");$(\"BODY\").removeClass(\"hideSelects\");});</script></head>";

            var result = _simplifier.Simplify(text);
            Assert.IsTrue(result.Contains("<title>"));
            Assert.IsTrue(result.Contains("name=\"title\""));
            Assert.IsTrue(result.Contains("name=\"Keywords\""));
            Assert.IsTrue(result.Contains("name=\"Description\""));
            Assert.IsFalse(result.Contains("script"));
            Assert.IsFalse(result.Contains("link"));
        }
    }
}
