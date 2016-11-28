using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
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
            /*_path = "testDir";
            _indexPath = XmlStorage.IndexName;*/
            _driver = Substitute.For<IStorageDriver>();
            /*_info = new DataInfo("test data");
            _fakeIndex = new XmlStorageIndex();
            _fakeIndex.Items.AddRange(new[] { new XmlStorageItem { FileName = "file1.bin", Url = "url1" }, new XmlStorageItem { FileName = "file2.bin", Url = "url2" } });
            _driverFiles = new[] { _indexPath, "file1.bin", "file2.bin", "file3.bin" };*/
        }

        [Test]
        public void CtorArguments()
        {
            Assert.DoesNotThrow(() => new XmlStorageIndex(_driver));
            Assert.Catch<ArgumentNullException>(() => new XmlStorageIndex(null));
        }

        [Test]
        public void GetIndexNotExist()
        {
            Assert.DoesNotThrow(() => XmlStorageIndex.GetIndex(_driver));
            Assert.Catch<ArgumentNullException>(() => XmlStorageIndex.GetIndex(null));

            _driver.Exists(Arg.Is(XmlStorageIndex.IndexName)).Returns(false);
            _driver.ClearReceivedCalls();

            var x = XmlStorageIndex.GetIndex(_driver);

            _driver.Received(1).Exists(Arg.Is(XmlStorageIndex.IndexName));
            Assert.AreEqual(x.Items.Count, 0);
        }

        [Test]
        public void GetIndexExist()
        {
            _driver.Exists(Arg.Is(XmlStorageIndex.IndexName)).Returns(true);

            var x = XmlStorageIndex.GetIndex(_driver);

            _driver.Received(1).Exists(Arg.Is(XmlStorageIndex.IndexName));
            Assert.AreEqual(x.Items.Count, 0);
        }


    }
}
