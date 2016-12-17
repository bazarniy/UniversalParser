using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    using Base;
    using Base.Helpers;
    using NUnit.Framework;

    [TestFixture]
    public class UrlExtentionsTest
    {
        [Test]
        public void Ctor()
        {
            Assert.IsNull(((Url)null).Fix());
        }

        [Test]
        [TestCase("http://test.com//", "")]
        [TestCase("http://test.com//#2354", "")]
        [TestCase("//", "http://test.com")]
        [TestCase("http://test.com//?", "")]
        [TestCase("http://test.com//?sdf=2354", "")]
        [TestCase("//?sdf=sdf", "http://test.com")]
        public void Path1Double(string url, string domain)
        {
            var u = Url.Create(url, domain).Fix();
            Assert.AreEqual("/", u.Path);
        }

        [Test]
        [TestCase("http://test.com/sdf//", "http://test.com")]
        [TestCase("http://test.com/sdf//", "")]
        [TestCase("http://test.com/sdf//#2354", "")]
        [TestCase("/sdf//", "http://test.com")]
        [TestCase("http://test.com/sdf//?", "")]
        [TestCase("http://test.com/sdf//?sdf=2354", "")]
        [TestCase("/sdf//?sdf=sdf", "http://test.com")]
        public void Path2Double(string url, string domain)
        {
            var u = Url.Create(url, domain).Fix();
            Assert.AreEqual("/sdf/", u.Path);
        }

        [Test]
        [TestCase("http://test.com/asd/sdf//", "http://test.com")]
        [TestCase("http://test.com/asd/sdf//", "")]
        [TestCase("http://test.com/asd/sdf//#2354", "")]
        [TestCase("/asd/sdf//", "http://test.com")]
        [TestCase("http://test.com/asd/sdf//?", "")]
        [TestCase("http://test.com/asd/sdf//?sdf=2354", "")]
        [TestCase("/asd/sdf//?sdf=sdf", "http://test.com")]
        public void Path3Double(string url, string domain)
        {
            var u = Url.Create(url, domain).Fix();
            Assert.AreEqual("/asd/sdf/", u.Path);
        }

        [Test]
        public void PathMailto()
        {
            var u = Url.Create("http://test.com/mailtoololo@sdf.sd", "http://test.com").Fix();
            Assert.IsNull(u);
            u = Url.Create("mailtoololo@sdf.sd", "http://test.com").Fix();
            Assert.IsNull(u);
            u = Url.Create("/sdf/", "http://test.com");
            Assert.IsNull(u.LinkTo("mailtoololo@sdf.sd").Fix());
        }
    }
}
