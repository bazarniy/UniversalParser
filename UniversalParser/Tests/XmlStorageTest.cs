using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    using System.IO;
    using System.Runtime.Serialization;
    using Base;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using NUnit.Framework;
    using XmlStorage;

    [TestFixture]
    public class XmlStorageTest
    {
        private string _path;
        private IStorageDriver _driver;
        private XmlStorage _storage => new XmlStorage(_driver);
        private DataInfo _info;
        private string _indexPath;
        private XmlStorageIndex _fakeIndex;
        private string[] _driverFiles;


        [SetUp]
        public void TestSetup()
        {
            _path = "testDir";
            //_indexPath = XmlStorage.IndexName;
            _driver = Substitute.For<IStorageDriver>();
            _info = new DataInfo("test data");
            _fakeIndex = new XmlStorageIndex(_driver);
            //_fakeIndex.Items.AddRange(new[] {new StorageItem {FileName = "file1.bin", Url = "url1"}, new StorageItem {FileName = "file2.bin", Url = "url2"} });
            _driverFiles = new[] {_indexPath, "file1.bin", "file2.bin", "file3.bin"};
        }

        /*
            после записи файл и url записывается в индекс
            индекс умеет возвращать коллекцию файлов
            индекс умеет возвращать коллекцию url
            драйвер должен принимать на вход базовый путь и париться с путями сам
         */

        [Test]
        public void StorageCtorArguments()
        {
            Assert.Catch<ArgumentNullException>(() => new XmlStorage(null));
            Assert.DoesNotThrow(() => new XmlStorage(Substitute.For<IStorageDriver>()));
        }

        [Test]
        public void StorageCtorInitialize_IndexClearFiles()
        {
            _driver.Exists(Arg.Is(_indexPath)).Returns(true);
            //_driver.Read<XmlStorageIndex>(Arg.Is(_indexPath)).Returns(_fakeIndex);
            //_driver.Enum(Arg.Is(_path)).Returns(_driverFiles);

            Assert.AreNotEqual(_storage.Count(), 0);
            _driver.Received().Remove(Arg.Is(_driverFiles.Last()));
            _driver.DidNotReceive().Remove(Arg.Is<string>(x => _driverFiles.Take(3).Contains(x)));
        }

        [Test]
        public void WriteData()
        {
            Assert.Throws<ArgumentNullException>(() => _storage.Write(null));

            const string testFileName = "randomFileName";

            _driver.GetRandomName().Returns(testFileName);
            _driver.Write(Arg.Any<string>()).Returns(x => new MemoryStream());
            Assert.DoesNotThrow(() => _storage.Write(_info));

            _driver.Received(1).Write(Arg.Is(testFileName));
        }

        [Test]
        public void Count()
        {
            _driver.GetRandomName().Returns("randomFileName");
            _driver.Write(Arg.Any<string>()).Returns(x => new MemoryStream());

            var stor = _storage;

            Assert.DoesNotThrow(() => stor.Count());
            Assert.AreEqual(0, stor.Count());

            stor.Write(_info);

            Assert.AreEqual(1, stor.Count());
        }

        [Test]
        public void ReadFileArgs()
        {
            Assert.Throws<ArgumentException>(() => _storage.GetFile(""));
        }

        [Test]
        public void ReadFileNotExist()
        {
            var testFileName = "randomFileName";
            _driver.Exists(Arg.Is(testFileName)).Returns(true);
            _driver.Read(Arg.Any<string>()).Returns(x => Stream.Null);

            Assert.DoesNotThrow(() => _storage.GetFile(testFileName));
            Assert.IsNull(_storage.GetFile(testFileName));

            _driver.Received(2).Read(Arg.Is(testFileName));
        }

        [Test]
        public void ReadFileNotIndexed()
        {
            var testFileName = "randomFileName";
            Assert.DoesNotThrow(() => _storage.GetFile(testFileName));
            _driver.DidNotReceive().Read(Arg.Is(testFileName));
        }

        [Test]
        public void ReadFile()
        {
            var testFileName = "randomFileName";

            _driver.GetRandomName().Returns(testFileName);
            _driver.Write(Arg.Is(testFileName)).Returns(ux => File.Create(testFileName));

            var stor = _storage;
            stor.Write(_info);

            _driver.Exists(Arg.Is(testFileName)).Returns(true);
            _driver.Read(Arg.Is(testFileName)).Returns(ux => File.OpenRead(testFileName));

            Assert.DoesNotThrow(() => stor.GetFile(testFileName));
            Assert.IsNotNull(stor.GetFile(testFileName));

            _driver.Received(2).Read(Arg.Is(testFileName));
        }

        [Test]
        public void ReadFileCorrupted()
        {
            var testFileName = "randomFileName";
            _driver.Exists(Arg.Is(testFileName)).Returns(true);
            _driver.Read(Arg.Any<string>()).Returns(x => new MemoryStream());

            Assert.Throws<SerializationException>(() => _storage.GetFile(testFileName));
        }
    }
}
