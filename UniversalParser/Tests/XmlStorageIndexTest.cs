﻿using System;
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

        // TODO: thread safe Save method

        [Test]
        public void CtorArguments()
        {
            Assert.DoesNotThrow(() => new XmlStorageIndex(_driver));
            Assert.Catch<ArgumentNullException>(() => new XmlStorageIndex(null));
        }

        [Test]
        public void GetIndexNotExist()
        {
            _driver.Exists(Arg.Is(XmlStorageIndex.IndexName)).Returns(false);

            var x = new XmlStorageIndex(_driver);

            Assert.AreEqual(x.Count(), 0);
        }

        private static readonly StorageItem[][] _indexItems = {new StorageItem[] {}, new[] {new StorageItem {FileName = "ololo", Url = "azaza"}, new StorageItem()}};

        [Test]
        [TestCaseSource(nameof(_indexItems))]
        public void GetIndexExist(StorageItem[] items)
        {
            _driver.Exists(Arg.Is(XmlStorageIndex.IndexName)).Returns(false);

            var ind = new XmlStorageIndex(_driver);
            foreach (var storageItem in items)
            {
                ind.Add(storageItem);
            }
            ind.Save();

            _driver.Exists(Arg.Is(XmlStorageIndex.IndexName)).Returns(true);
            _driver.Read(Arg.Is(XmlStorageIndex.IndexName)).Returns(ux => File.OpenRead(XmlStorageIndex.IndexName));

            var x = new XmlStorageIndex(_driver);

            Assert.AreEqual(x.Count(), items.Length);
            Assert.DoesNotThrow(x.Save);
        }

        private static readonly Stream[] _corruptedStreams = {Stream.Null, new MemoryStream()};

        [Test]
        [TestCaseSource(nameof(_corruptedStreams))]
        public void GetIndexExistCorrupted(Stream stream)
        {
            _driver.Exists(Arg.Is(XmlStorageIndex.IndexName)).Returns(true);
            _driver.Read(Arg.Is(XmlStorageIndex.IndexName)).Returns(ux => stream);

            var x = new XmlStorageIndex(_driver);

            Assert.AreEqual(x.Count(), 0);
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

        [Test]
        public void Exists()
        {
            var filename = "testFilename";
            var ind = new XmlStorageIndex(_driver);
            Assert.Throws<ArgumentException>(() => ind.Exists(""));
            Assert.DoesNotThrow(() => ind.Exists(filename));
            Assert.IsFalse(ind.Exists(filename));
            ind.Add(new StorageItem {FileName = filename});
            Assert.IsTrue(ind.Exists(filename));
        }
    }
}
