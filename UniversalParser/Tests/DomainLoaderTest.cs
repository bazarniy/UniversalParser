using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    using Base;
    using Networking;
    using Networking.WebClient;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using NUnit.Framework;

    [TestFixture]
    public class DomainLoaderTest
    {
        private IWebClientFactory _factory;
        private IWebClient _client;
        private IDataWriter _writer;
        private const string TestDomain = "http://domain.com/";

        private DomainLoader GetNewDomainLoader => new DomainLoader(_factory, _writer, TestDomain);

        //TODO: докачка
        //TODO: эвенты для обновления или IObservable

        [SetUp]
        public void TestSetup()
        {
            _factory = Substitute.For<IWebClientFactory>();
            _client = Substitute.For<IWebClient>();
            _writer = Substitute.For<IDataWriter>();

            _factory.Create().Returns(_client);
        }

        [Test]
        public void Ctor()
        {
            Assert.Throws<ArgumentNullException>(() => new DomainLoader(null, _writer, TestDomain));
            Assert.Throws<ArgumentNullException>(() => new DomainLoader(_factory, null, TestDomain));
            Assert.Throws<ArgumentException>(() => new DomainLoader(_factory, _writer, ""));
            Assert.Throws<ArgumentException>(() => new DomainLoader(_factory, _writer, "domain.com"));
            Assert.DoesNotThrow(() => new DomainLoader(_factory, _writer, TestDomain));
        }

        [Test]
        public void StartDownload()
        {
            _client.Download(Arg.Any<Url>()).Returns(DelayReturns(new WebResult { Url = Url.Create(TestDomain), ErrorCode = 200, Data = "" }));

            var x = GetNewDomainLoader;
            Assert.DoesNotThrowAsync(() => x.Download());
            _client.Received(1).Download(Arg.Is(Url.Create(TestDomain)));
        }


        //TODO:rewrite
        /*[Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Download(int parralel)
        {
            var links = new List<Url> { new Url(TestDomain) };
            for (var i = 0; i < 100; i++)
            {
                links.Add(new Url(TestDomain + i));
            }

            _client.Download(Arg.Any<Url>()).Returns(DelayReturns(new DataInfo(TestDomain) { Links = links.ToArray() }));
            var x = GetNewDomainLoader;
            Parallel.For(0, 100, i => Assert.DoesNotThrowAsync(() => x.Download(parralel)));
            foreach (var link in links)
            {
                _client.Received(1).Download(Arg.Is(link));
            }
        }*/

        [Test]
        public void DownloadAnyException()
        {
            var links = new List<Url> { Url.Create(TestDomain) };
            for (var i = 0; i < 100; i++)
            {
                links.Add(Url.Create(TestDomain + i));
            }

            _client.Download(Arg.Any<Url>()).Throws(new ApplicationException("test"));
            var x = GetNewDomainLoader;
            Assert.DoesNotThrowAsync(() => x.Download(2));
            Assert.DoesNotThrow(() => GetNewDomainLoader.GetResults());
        }

        [Test]
        public void Download404()
        {
            _client.Download(Arg.Any<Url>()).Returns(new WebResult {Url = Url.Create(TestDomain), ErrorCode = 404, Data = ""});
            var x = GetNewDomainLoader;
            Assert.DoesNotThrowAsync(() => x.Download(1));
            Assert.IsNull(x.GetResults().First().Value);
        }

        [Test]
        public void DownloadEmpty()
        {
            _client.Download(Arg.Any<Url>()).Returns(new WebResult { Url = Url.Create(TestDomain), ErrorCode = 200, Data = "" });
            var x = GetNewDomainLoader;
            Assert.DoesNotThrowAsync(() => x.Download(1));
            Assert.NotNull(x.GetResults().First().Value);
        }

        [Test]
        public void DownloadGetErrorResults()
        {
            Assert.DoesNotThrow(() => GetNewDomainLoader.GetResults());
        }

        private static Task<T> DelayReturns<T>(T info)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(100);
                return info;
            });
        }
    }
}
