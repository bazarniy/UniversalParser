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
        private const string TestDomain = "http://domain.com";

        [SetUp]
        public void TestSetup()
        {
            _factory = Substitute.For<IWebClientFactory>();
            _client = Substitute.For<IWebClient>();

            _factory.Create().Returns(_client);
        }

        [Test]
        public void Ctor()
        {
            Assert.Throws<ArgumentNullException>(() => new DomainLoader(null, TestDomain));
            Assert.Throws<ArgumentException>(() => new DomainLoader(_factory, ""));
            Assert.Throws<ArgumentException>(() => new DomainLoader(_factory, "domain.com"));
            Assert.DoesNotThrow(() => new DomainLoader(_factory, TestDomain));
        }

        [Test]
        public void StartDownload()
        {
            _client.Download(Arg.Any<string>(), Arg.Is(TestDomain)).Returns(DelayReturns(new DataInfo(TestDomain) { Links = new string[0] }));

            var x = new DomainLoader(_factory, TestDomain);
            Assert.DoesNotThrowAsync(() => x.Download());
            _client.Received(1).Download(Arg.Is(TestDomain), Arg.Is(TestDomain));
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Download(int parralel)
        {
            var links = new List<string> { TestDomain };
            for (var i = 0; i < 100; i++)
            {
                links.Add($"link{i}");
            }

            _client.Download(Arg.Any<string>(), Arg.Any<string>()).Returns(DelayReturns(new DataInfo(TestDomain) { Links = links.ToArray() }));
            var x = new DomainLoader(_factory, TestDomain);
            Parallel.For(0, 100, i => Assert.DoesNotThrowAsync(() => x.Download(parralel)));
            foreach (var link in links)
            {
                _client.Received(1).Download(Arg.Is(link), Arg.Is(TestDomain));
            }
        }

        [Test]
        public void Download2()
        {
            var links = new List<string> { TestDomain };
            for (var i = 0; i < 100; i++)
            {
                links.Add($"link{i}");
            }

            _client.Download(Arg.Any<string>(), Arg.Any<string>()).Throws(new ApplicationException("test"));
            var x = new DomainLoader(_factory, TestDomain);
            Assert.DoesNotThrowAsync(() => x.Download(2));
        }

        private static Task<DataInfo> DelayReturns(DataInfo info)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(100);
                return info;
            });
        }
    }
}
