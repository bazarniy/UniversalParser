using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    using System.Runtime.InteropServices;
    using Base;
    using NUnit.Framework;

    [TestFixture]
    public class UrlTest
    {
        //TODO: validate noncontent links in constructor
        //TODO: null url

        [Test]
        public void Ctor()
        {
            Assert.Throws<ArgumentException>(() => new Url(""));
            Assert.Throws<ArgumentException>(() => new Url("/", "invaliddomain"));
            Assert.DoesNotThrow(() => new Url("http://test.com", "http://test2.com"));
            Assert.AreEqual("http://test.com/", new Url("http://test.com", "http://test2.com").ToString());
            Assert.Throws<ArgumentException>(() => new Url("http://testcom"));
            Assert.DoesNotThrow(() => new Url("/sdf", "http://test2.com"));
            Assert.Throws<ArgumentException>(() => new Url("/sdf"));
        }

        [Test]
        [TestCase("http://test.com", "http://test.com")]
        [TestCase("http://test.com", "")]
        [TestCase("http://test.com/", "")]
        [TestCase("http://test.com/sdf", "")]
        [TestCase("http://test.com/sdf/", "")]
        [TestCase("http://test.com/sdf/sdf?", "")]
        [TestCase("http://test.com/sdf/sdf?sdf=123", "")]
        [TestCase("http://test.com/sdf/sdf?sdf=123#", "")]
        [TestCase("http://test.com/sdf/sdf?sdf=123#2354", "")]
        [TestCase("http://test.com/sdf/sdf#2354", "")]
        [TestCase("http://test.com/sdf#2354", "")]
        [TestCase("http://test.com/#2354", "")]
        [TestCase("http://test.com#2354", "")]
        [TestCase("http://test.com/?sdf=2354", "")]
        [TestCase("http://test.com?sdf=2354", "")]
        [TestCase("http://test.com/?", "")]
        [TestCase("http://test.com?", "")]
        [TestCase("", "http://test.com")]
        [TestCase("/", "http://test.com")]
        [TestCase("/sdf", "http://test.com")]
        [TestCase("/sdf/", "http://test.com")]
        [TestCase("/sdf/sdf?", "http://test.com")]
        [TestCase("/sdf/sdf?sdf=123", "http://test.com")]
        [TestCase("?", "http://test.com")]
        [TestCase("?sdf=123", "http://test.com")]
        [TestCase("/?sdf=123", "http://test.com")]
        [TestCase("/sdf/sdf?sdf=123#", "http://test.com")]
        [TestCase("/sdf/sdf?sdf=123#2354", "http://test.com")]
        [TestCase("/sdf/sdf#2354", "http://test.com")]
        [TestCase("/sdf#2354", "http://test.com")]
        [TestCase("/#2354", "http://test.com")]
        [TestCase("#2354", "http://test.com")]
        [TestCase("sdf", "http://test.com")]
        [TestCase("sdf/", "http://test.com")]
        [TestCase("sdf/sdf?", "http://test.com")]
        [TestCase("sdf/sdf?sdf=123", "http://test.com")]
        [TestCase("sdf/sdf?sdf=123#", "http://test.com")]
        [TestCase("sdf/sdf?sdf=123#2354", "http://test.com")]
        [TestCase("sdf/sdf#2354", "http://test.com")]
        [TestCase("sdf#2354", "http://test.com")]
        public void Domain(string url, string domain)
        {
            var u = new Url(url, domain);
            Assert.AreEqual("http://test.com", u.Domain);
        }

        [Test]
        [TestCase("http://test.com", "http://test.com")]
        [TestCase("http://test.com", "")]
        [TestCase("http://test.com/", "")]
        [TestCase("http://test.com//", "")]
        [TestCase("http://test.com/#2354", "")]
        [TestCase("http://test.com//#2354", "")]
        [TestCase("http://test.com#2354", "")]
        [TestCase("", "http://test.com")]
        [TestCase("/", "http://test.com")]
        [TestCase("//", "http://test.com")]
        [TestCase("http://test.com/?", "")]
        [TestCase("http://test.com//?", "")]
        [TestCase("http://test.com/?sdf=2354", "")]
        [TestCase("http://test.com//?sdf=2354", "")]
        [TestCase("http://test.com?sdf=2354", "")]
        [TestCase("?", "http://test.com")]
        [TestCase("?sdf=sd", "http://test.com")]
        [TestCase("/?sdf=sdf", "http://test.com")]
        [TestCase("//?sdf=sdf", "http://test.com")]
        public void Path1(string url, string domain)
        {
            var u = new Url(url, domain);
            Assert.AreEqual("/", u.Path);
        }

        [Test]
        [TestCase("http://test.com/sdf", "http://test.com")]
        [TestCase("http://test.com/sdf/", "http://test.com")]
        [TestCase("http://test.com/sdf//", "http://test.com")]
        [TestCase("http://test.com/sdf", "")]
        [TestCase("http://test.com/sdf/", "")]
        [TestCase("http://test.com/sdf//", "")]
        [TestCase("http://test.com/sdf/#2354", "")]
        [TestCase("http://test.com/sdf//#2354", "")]
        [TestCase("http://test.com/sdf#2354", "")]
        [TestCase("/sdf", "http://test.com")]
        [TestCase("/sdf/", "http://test.com")]
        [TestCase("/sdf//", "http://test.com")]
        [TestCase("http://test.com/sdf/?", "")]
        [TestCase("http://test.com/sdf//?", "")]
        [TestCase("http://test.com/sdf/?sdf=2354", "")]
        [TestCase("http://test.com/sdf//?sdf=2354", "")]
        [TestCase("http://test.com/sdf?sdf=2354", "")]
        [TestCase("/sdf?", "http://test.com")]
        [TestCase("/sdf?sdf=sd", "http://test.com")]
        [TestCase("/sdf/?sdf=sdf", "http://test.com")]
        [TestCase("/sdf//?sdf=sdf", "http://test.com")]
        public void Path2(string url, string domain)
        {
            var u = new Url(url, domain);
            Assert.AreEqual("/sdf", u.Path);
        }

        [Test]
        [TestCase("http://test.com/asd/sdf", "http://test.com")]
        [TestCase("http://test.com/asd/sdf/", "http://test.com")]
        [TestCase("http://test.com/asd/sdf//", "http://test.com")]
        [TestCase("http://test.com/asd/sdf", "")]
        [TestCase("http://test.com/asd/sdf/", "")]
        [TestCase("http://test.com/asd/sdf//", "")]
        [TestCase("http://test.com/asd/sdf/#2354", "")]
        [TestCase("http://test.com/asd/sdf//#2354", "")]
        [TestCase("http://test.com/asd/sdf#2354", "")]
        [TestCase("/asd/sdf", "http://test.com")]
        [TestCase("/asd/sdf/", "http://test.com")]
        [TestCase("/asd/sdf//", "http://test.com")]
        [TestCase("http://test.com/asd/sdf/?", "")]
        [TestCase("http://test.com/asd/sdf//?", "")]
        [TestCase("http://test.com/asd/sdf/?sdf=2354", "")]
        [TestCase("http://test.com/asd/sdf//?sdf=2354", "")]
        [TestCase("http://test.com/asd/sdf?sdf=2354", "")]
        [TestCase("/asd/sdf?", "http://test.com")]
        [TestCase("/asd/sdf?sdf=sd", "http://test.com")]
        [TestCase("/asd/sdf/?sdf=sdf", "http://test.com")]
        [TestCase("/asd/sdf//?sdf=sdf", "http://test.com")]
        public void Path3(string url, string domain)
        {
            var u = new Url(url, domain);
            Assert.AreEqual("/asd/sdf", u.Path);
        }

        [Test]
        [TestCase("http://test.com", "http://test.com")]
        [TestCase("http://test.com", "")]
        [TestCase("http://test.com/", "")]
        [TestCase("http://test.com/sdf", "")]
        [TestCase("http://test.com/sdf/", "")]
        [TestCase("http://test.com/sdf/sdf?", "")]
        [TestCase("http://test.com/sdf/sdf#2354", "")]
        [TestCase("http://test.com/sdf#2354", "")]
        [TestCase("http://test.com/#2354", "")]
        [TestCase("http://test.com#2354", "")]
        [TestCase("http://test.com/?", "")]
        [TestCase("http://test.com?", "")]
        [TestCase("", "http://test.com")]
        [TestCase("/", "http://test.com")]
        [TestCase("/sdf", "http://test.com")]
        [TestCase("/sdf/", "http://test.com")]
        [TestCase("/sdf/sdf?", "http://test.com")]
        [TestCase("?", "http://test.com")]
        [TestCase("/sdf/sdf#2354", "http://test.com")]
        [TestCase("/sdf#2354", "http://test.com")]
        [TestCase("/#2354", "http://test.com")]
        [TestCase("#2354", "http://test.com")]
        [TestCase("sdf", "http://test.com")]
        [TestCase("sdf/", "http://test.com")]
        [TestCase("sdf/sdf?", "http://test.com")]
        [TestCase("sdf/sdf#2354", "http://test.com")]
        [TestCase("sdf#2354", "http://test.com")]
        public void Params1(string url, string domain)
        {
            var u = new Url(url, domain);
            Assert.AreEqual("", u.Params);
        }

        [Test]
        [TestCase("http://test.com/sdf/sdf?sdf=123", "")]
        [TestCase("http://test.com/sdf/sdf?sdf=123#", "")]
        [TestCase("http://test.com/sdf/sdf?sdf=123#2354", "")]
        [TestCase("http://test.com/?sdf=123", "")]
        [TestCase("http://test.com?sdf=123", "")]
        [TestCase("/sdf/sdf?sdf=123", "http://test.com")]
        [TestCase("?sdf=123", "http://test.com")]
        [TestCase("/?sdf=123", "http://test.com")]
        [TestCase("/sdf/sdf?sdf=123#", "http://test.com")]
        [TestCase("/sdf/sdf?sdf=123#2354", "http://test.com")]
        [TestCase("sdf/sdf?sdf=123", "http://test.com")]
        [TestCase("sdf/sdf?sdf=123#", "http://test.com")]
        [TestCase("sdf/sdf?sdf=123#2354", "http://test.com")]
        [TestCase("sdf/sdf?sdf=123?x=2#", "http://test.com")]
        public void Params2(string url, string domain)
        {
            var u = new Url(url, domain);
            Assert.AreEqual("sdf=123", u.Params);
        }

        [Test]
        [TestCase("http://test.com/sdf/sdf?sdf=123&x=abs", "")]
        [TestCase("http://test.com/sdf/sdf?sdf=123&x=abs#", "")]
        [TestCase("http://test.com/sdf/sdf?sdf=123&x=abs#2354", "")]
        [TestCase("http://test.com/?sdf=123&x=abs", "")]
        [TestCase("http://test.com?sdf=123&x=abs", "")]
        [TestCase("/sdf/sdf?sdf=123&x=abs", "http://test.com")]
        [TestCase("?sdf=123&x=abs", "http://test.com")]
        [TestCase("/?sdf=123&x=abs", "http://test.com")]
        [TestCase("/sdf/sdf?sdf=123&x=abs#", "http://test.com")]
        [TestCase("/sdf/sdf?sdf=123&x=abs#2354", "http://test.com")]
        [TestCase("sdf/sdf?sdf=123&x=abs", "http://test.com")]
        [TestCase("sdf/sdf?sdf=123&x=abs#", "http://test.com")]
        [TestCase("sdf/sdf?sdf=123&x=abs#2354", "http://test.com")]
        public void Params3(string url, string domain)
        {
            var u = new Url(url, domain);
            Assert.AreEqual("sdf=123&x=abs", u.Params);
        }

        private static Url BaseUrl => new Url("http://test.com/sdf/sdf?sdf=123&x=abs#2354");

        [Test]
        [TestCase("", "http://test.com/sdf/sdf?sdf=123&x=abs")]
        [TestCase("http://test.com", "http://test.com/")]
        [TestCase("http://ololo.com/fg?sdf=123#sdf", "http://ololo.com/fg?sdf=123")]
        [TestCase("/", "http://test.com/")]
        [TestCase("/sdf", "http://test.com/sdf")]
        [TestCase("/sdf/", "http://test.com/sdf")]
        [TestCase("/sdf/sdf", "http://test.com/sdf/sdf")]
        [TestCase("/sdf/sdf/", "http://test.com/sdf/sdf")]
        [TestCase("/sdf/sdf?sdf=123", "http://test.com/sdf/sdf?sdf=123")]
        [TestCase("/sdf/sdf/?sdf=123", "http://test.com/sdf/sdf?sdf=123")]
        [TestCase("/sdf/sdf?sdf=123&sfg=3", "http://test.com/sdf/sdf?sdf=123&sfg=3")]
        [TestCase("/sdf/sdf/?sdf=123&sfg=3", "http://test.com/sdf/sdf?sdf=123&sfg=3")]
        [TestCase("/sdf/sdf?sdf=123&sfg=3#sdf", "http://test.com/sdf/sdf?sdf=123&sfg=3")]
        [TestCase("/sdf/sdf/?sdf=123&sfg=3#sdf", "http://test.com/sdf/sdf?sdf=123&sfg=3")]
        [TestCase("/?sdf=123&sfg=3#sdf", "http://test.com/?sdf=123&sfg=3")]
        [TestCase("/?sdf=123?&sfg=3#sdf", "http://test.com/?sdf=123")]
        [TestCase("sdf", "http://test.com/sdf/sdf/sdf")]
        [TestCase("sdf/", "http://test.com/sdf/sdf/sdf")]
        [TestCase("sdf/sdf", "http://test.com/sdf/sdf/sdf/sdf")]
        [TestCase("sdf/sdf/", "http://test.com/sdf/sdf/sdf/sdf")]
        [TestCase("sdf/sdf?sdf=123", "http://test.com/sdf/sdf/sdf/sdf?sdf=123")]
        [TestCase("sdf/sdf/?sdf=123", "http://test.com/sdf/sdf/sdf/sdf?sdf=123")]
        [TestCase("sdf/sdf?sdf=123&sfg=3", "http://test.com/sdf/sdf/sdf/sdf?sdf=123&sfg=3")]
        [TestCase("sdf/sdf/?sdf=123&sfg=3", "http://test.com/sdf/sdf/sdf/sdf?sdf=123&sfg=3")]
        [TestCase("sdf/sdf?sdf=123&sfg=3#sdf", "http://test.com/sdf/sdf/sdf/sdf?sdf=123&sfg=3")]
        [TestCase("sdf/sdf/?sdf=123&sfg=3#sdf", "http://test.com/sdf/sdf/sdf/sdf?sdf=123&sfg=3")]
        [TestCase("?sdf=123&afg=3#sdf", "http://test.com/sdf/sdf?afg=3&sdf=123")]
        [TestCase("?sdf=123?&sfg=3#sdf", "http://test.com/sdf/sdf?sdf=123")]
        [TestCase("mailto:dsfg@dfg.rw", "http://test.com/sdf/sdf?sdf=123&x=abs")]
        [TestCase("javascript:xfg4444", "http://test.com/sdf/sdf?sdf=123&x=abs")]
        [TestCase("/http://test2.com", "http://test2.com/")]
        public void LinkTo(string url, string result)
        {
            Assert.AreEqual(result, BaseUrl.LinkTo(url).ToString());
        }


    }
}
