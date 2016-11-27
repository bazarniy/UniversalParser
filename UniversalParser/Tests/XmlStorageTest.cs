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
        private XmlStorage _storage => new XmlStorage(_path, _driver);
        private DataInfo _info;
        private string _indexPath;
        private XmlStorageIndex _fakeIndex;
        private string[] _driverFiles;


        [SetUp]
        public void TestSetup()
        {
            _path = "testDir";
            _indexPath = Path.Combine(_path, XmlStorage.IndexName);
            _driver = Substitute.For<IStorageDriver>();
            _info = new DataInfo("test data");
            _fakeIndex = new XmlStorageIndex();
            _fakeIndex.Items.AddRange(new[] {new XmlStorageItem {FileName = "file1.bin", Url = "url1"}, new XmlStorageItem {FileName = "file2.bin", Url = "url2"} });
            _driverFiles = new[] {_indexPath, "file1.bin", "file2.bin", "file3.bin"};
        }

        /*
            после записи файл и url записывается в индекс
            индекс умеет возвращать коллекцию файлов
            индекс умеет возвращать коллекцию url
            драйвер должен принимать на вход базовый путь и париться с путями сам
         */

        public static IEnumerable<TestCaseData> StorageCtorArgs
        {
            get
            {
                yield return new TestCaseData("testDir", Substitute.For<IStorageDriver>(), null);
                yield return new TestCaseData(null, null, typeof(ArgumentNullException));
                yield return new TestCaseData(null, Substitute.For<IStorageDriver>(), typeof(ArgumentNullException));
                yield return new TestCaseData("testDir", null, typeof(ArgumentNullException));
                yield return new TestCaseData("invalid:path", Substitute.For<IStorageDriver>(), typeof(ArgumentException));
            }
        }

        [Test]
        [TestCaseSource(nameof(StorageCtorArgs))]
        public void StorageCtorArguments(string path, IStorageDriver driver, Type exceptionType)
        {
            if (exceptionType == null)
            {
                Assert.DoesNotThrow(() => new XmlStorage(path, driver));
            }
            else
            {
                Assert.Catch(exceptionType, () => new XmlStorage(path, driver));
            }
        }

        [Test]
        public void StorageCtorInitialize_Calls()
        {
            var x = _storage;
            //_driver.Received(1).DirectoryCreate(Arg.Is(_path));
            _driver.Received(1).FileExist(Arg.Is(_indexPath));
        }

        [Test]
        public void StorageCtorInitialize_IndexNotExist()
        {
            _driver.FileExist(Arg.Is(_indexPath)).Returns(false);
            Assert.AreEqual(_storage.Count(), 0);
        }

        [Test]
        public void StorageCtorInitialize_IndexExist()
        {
            _driver.FileExist(Arg.Is(_indexPath)).Returns(true);
            //_driver.FileRead<XmlStorageIndex>(Arg.Is(_indexPath)).Returns(_fakeIndex);
            Assert.AreNotEqual(_storage.Count(), 0);
        }

        [Test]
        public void StorageCtorInitialize_IndexExistCorrupt()
        {
            _driver.FileExist(Arg.Is(_indexPath)).Returns(true);
            //_driver.FileRead<XmlStorageIndex>(Arg.Is(_indexPath)).Throws(new SerializationException());
            XmlStorage stor;
            Assert.DoesNotThrow(() => stor = _storage);
            Assert.AreEqual(_storage.Count(), 0);
        }

        [Test]
        public void StorageCtorInitialize_IndexClearFiles()
        {
            _driver.FileExist(Arg.Is(_indexPath)).Returns(true);
            //_driver.FileRead<XmlStorageIndex>(Arg.Is(_indexPath)).Returns(_fakeIndex);
            //_driver.FileEnum(Arg.Is(_path)).Returns(_driverFiles);

            Assert.AreNotEqual(_storage.Count(), 0);
            _driver.Received().FileRemove(Arg.Is(_driverFiles.Last()));
            _driver.DidNotReceive().FileRemove(Arg.Is<string>(x => _driverFiles.Take(3).Contains(x)));
        }

        [Test]
        public void WriteData()
        {
            Assert.Throws<ArgumentNullException>(() => _storage.Write(null));

            var testFileName = "randomFileName";

            _driver.GetRandomFileName().Returns(testFileName);
            Assert.DoesNotThrow(() => _storage.Write(_info));

            //driver.Received(1).FileWrite(Arg.Any<DataInfo>(), Arg.Is<string>(x => x.EndsWith(testFileName) && x.StartsWith(_path)));
        }

        [Test]
        public void Count()
        {
            Assert.DoesNotThrow(() => _storage.Count());
        }

        [Test]
        public void ReadFile()
        {
            var testFileName = "randomFileName";
            _driver.FileExist(Arg.Is(testFileName)).Returns(true);

            Assert.DoesNotThrow(() => _storage.GetFile(testFileName));
            //_driver.Received(1).FileRead<DataInfo>(Arg.Is<string>(x => x.EndsWith(testFileName) && x.StartsWith(_path)));
        }

        [Test]
        public void ReadCorruptedFile()
        {
            var testFileName = "randomFileName";
            _driver.FileExist(Arg.Is(testFileName)).Returns(true);
            //_driver.FileRead<DataInfo>(Arg.Any<string>()).Throws(new SerializationException());

            Assert.IsNull(_storage.GetFile(testFileName));
        }
    }
}
