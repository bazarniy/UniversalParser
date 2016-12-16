﻿using System;
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
        public void PathMailto()
        {
            var u = new Url("http://test.com/mailtoololo@sdf.sd", "http://test.com");
            Assert.AreEqual("/", u.Path);
            u = new Url("mailtoololo@sdf.sd", "http://test.com");
            Assert.AreEqual("", u.Path);
            u = new Url("/sdf/", "http://test.com");
            Assert.AreEqual("http://test.com/sdf/", u.LinkTo("mailto:dsfg@dfg.rw").ToString());
            Assert.AreEqual("http://test.com/sdf/", u.LinkTo("javascript:xfg4444").ToString());
            Assert.AreEqual("http://test.com/sdf/", u.LinkTo("mailtoololo@sdf.sd").ToString());
        }

        [Test]
        [TestCase("/../", "/")]
        [TestCase("/../register.php", "/register.php")]
        [TestCase("/../../", "/")]
        [TestCase("/../../register.php", "/register.php")]
        [TestCase("/test/../", "/")]
        [TestCase("/test/../register.php", "/register.php")]
        [TestCase("/test/test/../", "/test/")]
        [TestCase("/test/test/..", "/test")]
        [TestCase("/test/test/../register.php", "/test/register.php")]
        [TestCase("/./.", "")]
        [TestCase("/././", "")]
        [TestCase("/sdf/./.", "/sdf/")]
        [TestCase("/sdf/././", "/sdf/")]
        [TestCase("/asd/sdf/./.", "/asd/sdf/")]
        [TestCase("/asd/sdf/././", "/asd/sdf/")]
        [TestCase("/sdf/././asd", "/sdf/asd")]
        [TestCase("/sdf/././asd/", "/sdf/asd/")]
        [TestCase("/sdf/./.asd", "/sdf/.asd")]
        [TestCase("/sdf/./.asd/", "/sdf/.asd/")]
        public void PathRelative(string url, string result)
        {
            var u = new Url(url, "http://test.com");
            Assert.AreEqual(result, u.Path);
            u = new Url("/abb/dfg", "http://test.com");
            Assert.AreEqual(result, u.LinkTo(url).Path);
        }

        [Test]
        [TestCase("http://test.com/", "")]
        [TestCase("http://test.com//", "")]
        [TestCase("http://test.com/#2354", "")]
        [TestCase("http://test.com//#2354", "")]
        [TestCase("/", "http://test.com")]
        [TestCase("//", "http://test.com")]
        [TestCase("http://test.com/?", "")]
        [TestCase("http://test.com//?", "")]
        [TestCase("http://test.com/?sdf=2354", "")]
        [TestCase("http://test.com//?sdf=2354", "")]
        [TestCase("/?sdf=sdf", "http://test.com")]
        [TestCase("//?sdf=sdf", "http://test.com")]
        [TestCase("http://test.com/./.", "http://test.com")]
        [TestCase("http://test.com/mailtoololo@sdf.sd", "http://test.com")]
        public void Path1(string url, string domain)
        {
            var u = new Url(url, domain);
            Assert.AreEqual("/", u.Path);
        }

        [Test]
        [TestCase("http://test.com", "http://test.com")]
        [TestCase("http://test.com", "")]
        [TestCase("http://test.com#2354", "")]
        [TestCase("", "http://test.com")]
        [TestCase("http://test.com?sdf=2354", "")]
        [TestCase("?", "http://test.com")]
        [TestCase("?sdf=sd", "http://test.com")]
        public void Path11(string url, string domain)
        {
            var u = new Url(url, domain);
            Assert.AreEqual("", u.Path);
        }

        [Test]
        [TestCase("http://test.com/sdf", "http://test.com")]
        [TestCase("http://test.com/sdf", "")]
        [TestCase("http://test.com/sdf#2354", "")]
        [TestCase("/sdf", "http://test.com")]
        [TestCase("http://test.com/sdf?sdf=2354", "")]
        [TestCase("/sdf?", "http://test.com")]
        [TestCase("/sdf?sdf=sd", "http://test.com")]
        public void Path2(string url, string domain)
        {
            var u = new Url(url, domain);
            Assert.AreEqual("/sdf", u.Path);
        }

        [Test]
        [TestCase("http://test.com/sdf/", "http://test.com")]
        [TestCase("http://test.com/sdf//", "http://test.com")]
        [TestCase("http://test.com/sdf/", "")]
        [TestCase("http://test.com/sdf//", "")]
        [TestCase("http://test.com/sdf/#2354", "")]
        [TestCase("http://test.com/sdf//#2354", "")]
        [TestCase("/sdf/", "http://test.com")]
        [TestCase("/sdf//", "http://test.com")]
        [TestCase("http://test.com/sdf/?", "")]
        [TestCase("http://test.com/sdf//?", "")]
        [TestCase("http://test.com/sdf/?sdf=2354", "")]
        [TestCase("http://test.com/sdf//?sdf=2354", "")]
        [TestCase("/sdf/?sdf=sdf", "http://test.com")]
        [TestCase("/sdf//?sdf=sdf", "http://test.com")]
        public void Path21(string url, string domain)
        {
            var u = new Url(url, domain);
            Assert.AreEqual("/sdf/", u.Path);
        }

        [Test]
        [TestCase("http://test.com/asd/sdf", "http://test.com")]
        [TestCase("http://test.com/asd/sdf", "")]
        [TestCase("http://test.com/asd/sdf#2354", "")]
        [TestCase("/asd/sdf", "http://test.com")]
        [TestCase("http://test.com/asd/sdf?sdf=2354", "")]
        [TestCase("/asd/sdf?", "http://test.com")]
        [TestCase("/asd/sdf?sdf=sd", "http://test.com")]
        public void Path3(string url, string domain)
        {
            var u = new Url(url, domain);
            Assert.AreEqual("/asd/sdf", u.Path);
        }

        [Test]
        [TestCase("http://test.com/asd/sdf/", "http://test.com")]
        [TestCase("http://test.com/asd/sdf//", "http://test.com")]
        [TestCase("http://test.com/asd/sdf/", "")]
        [TestCase("http://test.com/asd/sdf//", "")]
        [TestCase("http://test.com/asd/sdf/#2354", "")]
        [TestCase("http://test.com/asd/sdf//#2354", "")]
        [TestCase("/asd/sdf/", "http://test.com")]
        [TestCase("/asd/sdf//", "http://test.com")]
        [TestCase("http://test.com/asd/sdf/?", "")]
        [TestCase("http://test.com/asd/sdf//?", "")]
        [TestCase("http://test.com/asd/sdf/?sdf=2354", "")]
        [TestCase("http://test.com/asd/sdf//?sdf=2354", "")]
        [TestCase("/asd/sdf/?sdf=sdf", "http://test.com")]
        [TestCase("/asd/sdf//?sdf=sdf", "http://test.com")]
        public void Path31(string url, string domain)
        {
            var u = new Url(url, domain);
            Assert.AreEqual("/asd/sdf/", u.Path);
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
        [TestCase("/sdf/", "http://test.com/sdf/")]
        [TestCase("/sdf/sdf", "http://test.com/sdf/sdf")]
        [TestCase("/sdf/sdf/", "http://test.com/sdf/sdf/")]
        [TestCase("/sdf/sdf?sdf=123", "http://test.com/sdf/sdf?sdf=123")]
        [TestCase("/sdf/sdf/?sdf=123", "http://test.com/sdf/sdf/?sdf=123")]
        [TestCase("/sdf/sdf?sdf=123&sfg=3", "http://test.com/sdf/sdf?sdf=123&sfg=3")]
        [TestCase("/sdf/sdf/?sdf=123&sfg=3", "http://test.com/sdf/sdf/?sdf=123&sfg=3")]
        [TestCase("/sdf/sdf?sdf=123&sfg=3#sdf", "http://test.com/sdf/sdf?sdf=123&sfg=3")]
        [TestCase("/sdf/sdf/?sdf=123&sfg=3#sdf", "http://test.com/sdf/sdf/?sdf=123&sfg=3")]
        [TestCase("/?sdf=123&sfg=3#sdf", "http://test.com/?sdf=123&sfg=3")]
        [TestCase("/?sdf=123?&sfg=3#sdf", "http://test.com/?sdf=123")]
        [TestCase("sdf", "http://test.com/sdf/sdf/sdf")]
        [TestCase("sdf/", "http://test.com/sdf/sdf/sdf/")]
        [TestCase("sdf/sdf", "http://test.com/sdf/sdf/sdf/sdf")]
        [TestCase("sdf/sdf/", "http://test.com/sdf/sdf/sdf/sdf/")]
        [TestCase("sdf/sdf?sdf=123", "http://test.com/sdf/sdf/sdf/sdf?sdf=123")]
        [TestCase("sdf/sdf/?sdf=123", "http://test.com/sdf/sdf/sdf/sdf/?sdf=123")]
        [TestCase("sdf/sdf?sdf=123&sfg=3", "http://test.com/sdf/sdf/sdf/sdf?sdf=123&sfg=3")]
        [TestCase("sdf/sdf/?sdf=123&sfg=3", "http://test.com/sdf/sdf/sdf/sdf/?sdf=123&sfg=3")]
        [TestCase("sdf/sdf?sdf=123&sfg=3#sdf", "http://test.com/sdf/sdf/sdf/sdf?sdf=123&sfg=3")]
        [TestCase("sdf/sdf/?sdf=123&sfg=3#sdf", "http://test.com/sdf/sdf/sdf/sdf/?sdf=123&sfg=3")]
        [TestCase("?sdf=123&afg=3#sdf", "http://test.com/sdf/sdf?afg=3&sdf=123")]
        [TestCase("?sdf=123?&sfg=3#sdf", "http://test.com/sdf/sdf?sdf=123")]
        [TestCase("/http://test2.com", "http://test2.com/")]
        [TestCase("http://test2com", "http://test.com/sdf/sdf?sdf=123&x=abs")]
        public void LinkTo(string url, string result)
        {
            Assert.AreEqual(result, BaseUrl.LinkTo(url).ToString());
        }

        [Test]
        public void LinkToPage()
        {
            var url=new Url("https://www.iek.ru/products/catalog/index.php");
            Assert.AreEqual("https://www.iek.ru/products/catalog/detail.php?ID=1", url.LinkTo("detail.php?ID=1").ToString());
            url = new Url("https://www.iek.ru/products/catalog/index.php/");
            Assert.AreEqual("https://www.iek.ru/products/catalog/index.php/detail.php?ID=1", url.LinkTo("detail.php?ID=1").ToString());
        }

        [Test]
        [TestCase("http://test.com?sdf=1&amp;qgen=2", "http://test.com/?qgen=2&sdf=1")]
        public void UrlDecode(string url, string result)
        {
            Assert.AreEqual(result, new Url(url).ToString());
            Assert.AreEqual(result, BaseUrl.LinkTo(url).ToString());
            
        }
    }
}
