using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    using System.IO;
    using Base;
    using NSubstitute;
    using NUnit.Framework;
    using XmlStorage;

    [TestFixture]
    public class XmlStorageIndexTest
    {
        private IStorageDriver _driver;

        [SetUp]
        public void TestSetup()
        {
            _driver = Substitute.For<IStorageDriver>();
            _driver.Write(Arg.Is(XmlStorageIndex.IndexName)).Returns(ux => File.Create(XmlStorageIndex.IndexName));
        }

        [TearDown]
        public void TestTearDown()
        {
            var file = new FileInfo(XmlStorageIndex.IndexName);
            if (file.Exists) file.Delete();
        }

        private static void SetIndexExist(IStorageDriver driver)
        {
            driver.Exists(Arg.Is(XmlStorageIndex.IndexName)).Returns(true);
            driver.Read(Arg.Is(XmlStorageIndex.IndexName)).Returns(ux => File.OpenRead(XmlStorageIndex.IndexName));
        }

        private static void SetIndexNotExist(IStorageDriver driver)
        {
            driver.Exists(Arg.Is(XmlStorageIndex.IndexName)).Returns(false);
        }

        // TODO: thread safe Save method
        // index corruption

        [Test]
        public void CtorArguments()
        {
            Assert.DoesNotThrow(() => new XmlStorageIndex(_driver));
            Assert.Catch<ArgumentNullException>(() => new XmlStorageIndex(null));
        }

        [Test]
        public void GetIndexNotExist()
        {
            SetIndexNotExist(_driver);

            var x = new XmlStorageIndex(_driver);

            _driver.Received(1).Exists(Arg.Is(XmlStorageIndex.IndexName));
            Assert.AreEqual(x.Count(), 0);
        }

        [Test]
        public void GetIndexExistEmpty()
        {
            SetIndexNotExist(_driver);

            var ind = new XmlStorageIndex(_driver);
            ind.Save();

            SetIndexExist(_driver);
            _driver.ClearReceivedCalls();

            var x = new XmlStorageIndex(_driver);

            _driver.Received(1).Exists(Arg.Is(XmlStorageIndex.IndexName));
            Assert.AreEqual(x.Count(), 0);
            Assert.DoesNotThrow(x.Save);
        }

        [Test]
        public void GetIndexExistNotEmpty()
        {
            SetIndexNotExist(_driver);

            var ind = new XmlStorageIndex(_driver);
            ind.Add(new StorageItem {FileName = "ololo", Url = "azaza"});
            ind.Add(new StorageItem());
            ind.Save();

            SetIndexExist(_driver);
            _driver.ClearReceivedCalls();

            var x = new XmlStorageIndex(_driver);

            _driver.Received(1).Exists(Arg.Is(XmlStorageIndex.IndexName));
            Assert.AreEqual(x.Count(), 2);
            Assert.DoesNotThrow(x.Save);
        }

        [Test]
        public void SaveEmpty()
        {
            var ind = new XmlStorageIndex(_driver);

            var file = new FileInfo(XmlStorageIndex.IndexName);
            Assert.DoesNotThrow(ind.Save);
            Assert.IsTrue(file.Exists);
            Assert.Greater(file.Length, 0);
        }

        [Test]
        public void AddCount()
        {
            var ind = new XmlStorageIndex(_driver);
            Assert.AreEqual(ind.Count(), 0);

            Assert.Catch<ArgumentNullException>(() => ind.Add(null));
            ind.Add(new StorageItem());
            Assert.AreEqual(ind.Count(), 1);
        }
    }
}
