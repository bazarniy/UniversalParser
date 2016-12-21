namespace Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using Base;
    using NSubstitute;
    using NUnit.Framework;
    using XmlStorage;

    [TestFixture]
    public class XmlStorageTest
    {
        private IStorageDriver _driver;
        private IStorageIndex _index;
        private DataInfo _info;
        private const string TestFileName = "randomFileName";

        [SetUp]
        public void TestSetup()
        {
            _driver = Substitute.For<IStorageDriver>();
            _index = Substitute.For<IStorageIndex>();
            _info = new DataInfo("test data");
        }

        [TearDown]
        public void TestTearDown()
        {
            var file = new FileInfo(TestFileName);
            if (file.Exists) file.Delete();
        }

        [Test]
        public void StorageCtorArguments()
        {
            Assert.Catch<ArgumentNullException>(() => new XmlStorage(null, null));
            Assert.Catch<ArgumentNullException>(() => new XmlStorage(null, _index));
            Assert.Catch<ArgumentNullException>(() => new XmlStorage(_driver, null));
            Assert.DoesNotThrow(() => new XmlStorage(_driver, _index));
        }

        [Test]
        public void StorageCtorInitialize_NonIndexedFiles()
        {
            var files = new[] {"file1.ext", "file2.ext", "file3.ext"};
            var items = new[] {new StorageItem {FileName = "file1.ext"}, new StorageItem {FileName = "file4.ext"}};
            var driver = Substitute.For<IStorageDriver>();
            driver.Enum().Returns(files);

            var index = Substitute.For<IStorageIndex>();
            index.Items.Returns(items);

            var storage = new XmlStorage(driver, index);

            driver.Received(2).Remove(Arg.Is<string>(x => x == files[1] || x == files[2]));
            index.Received(1).Remove(Arg.Is<StorageItem>(x => x.FileName == items[1].FileName));
        }

        [Test]
        public void WriteData()
        {
            var storage = new XmlStorage(_driver, _index);
            Assert.Throws<ArgumentNullException>(() => storage.Write<DataInfo>(null, "as"));
            Assert.Throws<ArgumentException>(() => storage.Write<DataInfo>(_info, ""));

            _driver.GetRandomName().Returns(TestFileName);
            _driver.Write(Arg.Any<string>()).Returns(x => new MemoryStream());
            Assert.DoesNotThrow(() => storage.Write(_info, _info.Url));

            _driver.Received(1).Write(Arg.Is(TestFileName));
            _index.Received(1).Add(Arg.Any<StorageItem>());
        }

        [Test]
        public void Count()
        {
            _driver.GetRandomName().Returns(TestFileName);
            _driver.Write(Arg.Any<string>()).Returns(x => new MemoryStream());

            var storage = new XmlStorage(_driver, new XmlStorageIndex(_driver));

            Assert.DoesNotThrow(() => storage.Count());
            Assert.AreEqual(0, storage.Count());

            storage.Write(_info, _info.Url);

            Assert.AreEqual(1, storage.Count());
        }

        [Test]
        public void ReadFileArgs()
        {
            var storage = new XmlStorage(_driver, new XmlStorageIndex(_driver));
            Assert.Throws<ArgumentException>(() => storage.Read<DataInfo>(""));
        }

        [Test]
        public void ReadFileNotExist()
        {
            _index.Get(Arg.Any<StorageItem>()).Returns(new StorageItem() {FileName = TestFileName});
            _driver.Exists(Arg.Is(TestFileName)).Returns(true);
            _driver.Read(Arg.Any<string>()).Returns(x => Stream.Null);

            var storage = new XmlStorage(_driver, _index);
            Assert.DoesNotThrow(() => storage.Read<DataInfo>(TestFileName));
            Assert.IsNull(storage.Read<DataInfo>(TestFileName));

            _driver.Received(2).Read(Arg.Is(TestFileName));
        }

        [Test]
        public void ReadFileNotIndexed()
        {
            var storage = new XmlStorage(_driver, new XmlStorageIndex(_driver));
            Assert.DoesNotThrow(() => storage.Read<DataInfo>(TestFileName));
            _driver.DidNotReceive().Read(Arg.Is(TestFileName));
        }

        [Test]
        public void ReadFile()
        {
            _driver.GetRandomName().Returns(TestFileName);
            _driver.Write(Arg.Is(TestFileName)).Returns(ux => File.Create(TestFileName));

            var storage = new XmlStorage(_driver, _index);
            storage.Write(_info, _info.Url);

            _driver.Exists(Arg.Is(TestFileName)).Returns(true);
            _index.Get(Arg.Is<StorageItem>(item => item.FileName == TestFileName)).Returns(new StorageItem() {FileName = TestFileName});
            _driver.Read(Arg.Is(TestFileName)).Returns(ux => File.OpenRead(TestFileName));

            Assert.DoesNotThrow(() => storage.Read<DataInfo>(TestFileName));
            Assert.IsNotNull(storage.Read<DataInfo>(TestFileName));

            _driver.Received(2).Read(Arg.Is(TestFileName));
        }

        [Test]
        public void ReadFileCorrupted()
        {
            _index.Get(Arg.Is<StorageItem>(item => item.FileName == TestFileName)).Returns(new StorageItem() { FileName = TestFileName });
            _driver.Exists(Arg.Is(TestFileName)).Returns(true);
            _driver.Read(Arg.Any<string>()).Returns(x => new MemoryStream());


            var storage = new XmlStorage(_driver, _index);
            Assert.Throws<SerializationException>(() => storage.Read<DataInfo>(TestFileName));
        }

        [Test]
        public void GetStorage()
        {
            Assert.Throws<ArgumentException>(() => XmlStorage.GetStorage("", "ext"));
            Assert.DoesNotThrow(() => XmlStorage.GetStorage("testpath", "ext"));
            Assert.NotNull(XmlStorage.GetStorage("testpath", "ext"));
        }

        [Test]
        public void Enum()
        {
            var storage = new XmlStorage(_driver, _index);
            Assert.DoesNotThrow(() => storage.Enum());
            Assert.IsEmpty(storage.Enum());

            _index.Items.Returns(new[] {new StorageItem() {Url = "test1"}, new StorageItem() {Url = "test2"}});
            Assert.IsNotEmpty(storage.Enum());
        }

        [Test]
        public void Deduplication()
        {
            _driver.Write(Arg.Any<string>()).Returns(x=>new MemoryStream());

            var index = new XmlStorageIndex(_driver);

            _driver.Exists(Arg.Any<string>()).Returns(true);
            _driver.GetLength(Arg.Is("f1")).Returns(3);
            _driver.GetLength(Arg.Is("f3")).Returns(3);
            var str1 = new MemoryStream(new byte[] {1, 2, 3});
            var str2 = new MemoryStream(new byte[] {1, 2, 3});
            _driver.Read(Arg.Is("f1")).Returns(str1);
            _driver.Read(Arg.Is("f3")).Returns(str2);

            var storage=new XmlStorage(_driver, index);

            index.Add(new StorageItem { FileName = "f1", Url = "url1" });
            index.Add(new StorageItem { FileName = "f2", Url = "url2" });
            index.Add(new StorageItem { FileName = "f3", Url = "url3" });
            storage.Deduplication();

            Assert.IsTrue(index.Items.Any(x => x.FileName == "f2"));
            Assert.IsTrue(index.Items.Count(x => x.FileName == "f1") == 2 || index.Items.Count(x => x.FileName == "f3") == 2);

        }
    }
}
