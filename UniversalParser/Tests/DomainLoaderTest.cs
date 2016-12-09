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
    using NUnit.Framework;

    [TestFixture]
    public class DomainLoaderTest
    {
        private const string TestDomain = "http://domain.com";
        [Test]
        public void Ctor()
        {
            Assert.Throws<ArgumentNullException>(() => new DomainLoader(null, TestDomain));
            Assert.Throws<ArgumentException>(() => new DomainLoader(Substitute.For<IWebClientFactory>(), ""));
            Assert.Throws<ArgumentException>(() => new DomainLoader(Substitute.For<IWebClientFactory>(), "domain.com"));
            Assert.DoesNotThrow(() => new DomainLoader(Substitute.For<IWebClientFactory>(), TestDomain));
        }

        [Test]
        public void StartDownload()
        {
            var factory = Substitute.For<IWebClientFactory>();
            var client = Substitute.For<IWebClient>();
            client.Download(Arg.Any<string>(), Arg.Is(TestDomain)).Returns(DelayReturns(new DataInfo(TestDomain) { Links = new string[0] }));
            
            factory.Create().Returns(client);

            var x = new DomainLoader(factory, TestDomain);
            Assert.DoesNotThrowAsync(() => x.Download());
            client.Received(1).Download(Arg.Is(TestDomain), Arg.Is(TestDomain));
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void Download(int parralel)
        {
            var links = new[] { "link1", "link2", TestDomain };

            var factory = Substitute.For<IWebClientFactory>();
            var client = Substitute.For<IWebClient>();
            client.Download(Arg.Any<string>(), Arg.Any<string>()).Returns(DelayReturns(new DataInfo("") { Links = new string[0] }));
            client.Download(Arg.Is(TestDomain), Arg.Is(TestDomain)).Returns(DelayReturns(new DataInfo(TestDomain) {Links = links }));
            client.Download(Arg.Is(links[0]), Arg.Is(TestDomain)).Returns(DelayReturns(new DataInfo(links[0]) { Links = links }));
            factory.Create().Returns(client);
            var x = new DomainLoader(factory, TestDomain);
            Assert.DoesNotThrowAsync(() => x.Download(parralel));
            foreach (var link in links)
            {
                client.Received(1).Download(Arg.Is(link), Arg.Is(TestDomain));
            }
        }

        private static Task<DataInfo> DelayReturns(DataInfo info)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(500);
                return info;
            });
        }
    }
}
