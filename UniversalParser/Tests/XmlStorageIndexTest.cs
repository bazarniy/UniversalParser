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

        [Test]
        public void CtorArguments()
        {
            Assert.DoesNotThrow(() => new XmlStorageIndex(_driver));
            Assert.Catch<ArgumentNullException>(() => new XmlStorageIndex(null));
        }

        [Test]
        public void GetIndexArguments()
        {
            Assert.DoesNotThrow(() => XmlStorageIndex.GetIndex(_driver));
            Assert.Catch<ArgumentNullException>(() => XmlStorageIndex.GetIndex(null));
        }

        [Test]
        public void GetIndexNotExist()
        {
            SetIndexNotExist(_driver);

            var x = XmlStorageIndex.GetIndex(_driver);

            _driver.Received(1).Exists(Arg.Is(XmlStorageIndex.IndexName));
            Assert.AreEqual(x.Items.Count, 0);
        }

        [Test]
        public void GetIndexExistEmpty()
        {
            SetIndexExist(_driver);

            var ind = new XmlStorageIndex(_driver);
            ind.Save();

            var x = XmlStorageIndex.GetIndex(_driver);

            _driver.Received(1).Exists(Arg.Is(XmlStorageIndex.IndexName));
            Assert.AreEqual(x.Items.Count, 0);
            Assert.DoesNotThrow(x.Save);
        }

        [Test]
        public void GetIndexExistNotEmpty()
        {
            SetIndexExist(_driver);

            var ind = new XmlStorageIndex(_driver);
            ind.Items.Add(new XmlStorageItem {FileName = "ololo", Url = "azaza"});
            ind.Items.Add(new XmlStorageItem());
            ind.Save();

            var x = XmlStorageIndex.GetIndex(_driver);

            _driver.Received(1).Exists(Arg.Is(XmlStorageIndex.IndexName));
            Assert.AreEqual(x.Items.Count, 2);
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
    }
}
