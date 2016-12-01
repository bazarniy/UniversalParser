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
            Assert.DoesNotThrow(() => new XmlStorage(_driver));
            Assert.DoesNotThrow(() => new XmlStorage(_driver, null));
            Assert.DoesNotThrow(() => new XmlStorage(_driver, _index));
        }

        /*[Test]
        public void StorageCtorInitialize_IndexClearFiles()
        {
            _driver.Exists(Arg.Is(_indexPath)).Returns(true);
            //_driver.Read<XmlStorageIndex>(Arg.Is(_indexPath)).Returns(_fakeIndex);
            //_driver.Enum(Arg.Is(_path)).Returns(_driverFiles);

            Assert.AreNotEqual(_storage.Count(), 0);
            _driver.Received().Remove(Arg.Is(_driverFiles.Last()));
            _driver.DidNotReceive().Remove(Arg.Is<string>(x => _driverFiles.Take(3).Contains(x)));
        }*/

        [Test]
        public void WriteData()
        {
            var storage = new XmlStorage(_driver);
            Assert.Throws<ArgumentNullException>(() => storage.Write(null));

            _driver.GetRandomName().Returns(TestFileName);
            _driver.Write(Arg.Any<string>()).Returns(x => new MemoryStream());
            Assert.DoesNotThrow(() => storage.Write(_info));

            _driver.Received(1).Write(Arg.Is(TestFileName));
        }

        [Test]
        public void Count()
        {
            _driver.GetRandomName().Returns(TestFileName);
            _driver.Write(Arg.Any<string>()).Returns(x => new MemoryStream());

            var storage = new XmlStorage(_driver);

            Assert.DoesNotThrow(() => storage.Count());
            Assert.AreEqual(0, storage.Count());

            storage.Write(_info);

            Assert.AreEqual(1, storage.Count());
        }

        [Test]
        public void ReadFileArgs()
        {
            var storage = new XmlStorage(_driver);
            Assert.Throws<ArgumentException>(() => storage.GetFile(""));
        }

        [Test]
        public void ReadFileNotExist()
        {
            _index.Exists(Arg.Is(TestFileName)).Returns(true);
            _driver.Exists(Arg.Is(TestFileName)).Returns(true);
            _driver.Read(Arg.Any<string>()).Returns(x => Stream.Null);

            var storage = new XmlStorage(_driver, _index);
            Assert.DoesNotThrow(() => storage.GetFile(TestFileName));
            Assert.IsNull(storage.GetFile(TestFileName));

            _driver.Received(2).Read(Arg.Is(TestFileName));
        }

        [Test]
        public void ReadFileNotIndexed()
        {
            var storage = new XmlStorage(_driver);
            Assert.DoesNotThrow(() => storage.GetFile(TestFileName));
            _driver.DidNotReceive().Read(Arg.Is(TestFileName));
        }

        [Test]
        public void ReadFile()
        {
            _driver.GetRandomName().Returns(TestFileName);
            _driver.Write(Arg.Is(TestFileName)).Returns(ux => File.Create(TestFileName));

            var storage = new XmlStorage(_driver);
            storage.Write(_info);

            _driver.Exists(Arg.Is(TestFileName)).Returns(true);
            _driver.Read(Arg.Is(TestFileName)).Returns(ux => File.OpenRead(TestFileName));

            Assert.DoesNotThrow(() => storage.GetFile(TestFileName));
            Assert.IsNotNull(storage.GetFile(TestFileName));

            _driver.Received(2).Read(Arg.Is(TestFileName));
        }

        [Test]
        public void ReadFileCorrupted()
        {
            _index.Exists(Arg.Is(TestFileName)).Returns(true);
            _driver.Exists(Arg.Is(TestFileName)).Returns(true);
            _driver.Read(Arg.Any<string>()).Returns(x => new MemoryStream());


            var storage = new XmlStorage(_driver, _index);
            Assert.Throws<SerializationException>(() => storage.GetFile(TestFileName));
        }
    }
}
