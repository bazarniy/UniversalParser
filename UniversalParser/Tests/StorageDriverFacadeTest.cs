namespace Tests
{
    using System;
    using System.Collections.Generic;
    using Base.Helpers;
    using Base.Utilities;
    using NSubstitute;
    using NUnit.Framework;
    using XmlStorage;

    [TestFixture]
    public class StorageDriverFacadeTest
    {
        private const string Ext = "tip";

        private IStorageDriver _baseDriver;
        private StorageDriverFacade _driver;
        private StorageDriverFacade _driverEmptyExt;

        [SetUp]
        public void TestSetup()
        {
            _baseDriver = Substitute.For<IStorageDriver>();
            _driver = new StorageDriverFacade(Ext, _baseDriver);
            _driverEmptyExt = new StorageDriverFacade("", _baseDriver);
        }

        [Test]
        public void Ctor()
        {
            Assert.DoesNotThrow(() => new StorageDriverFacade(Ext, _baseDriver));
            Assert.DoesNotThrow(() => new StorageDriverFacade("", _baseDriver));
            Assert.Throws<ArgumentNullException>(() => new StorageDriverFacade(Ext, null));
            Assert.Throws<ArgumentNullException>(() => new StorageDriverFacade("", null));
            Assert.Throws<ArgumentException>(() => new StorageDriverFacade("toolongextention", _baseDriver));
            Assert.Throws<ArgumentException>(() => new StorageDriverFacade("t.w", _baseDriver));
        }

        [Test]
        public void GetRandomName()
        {
            Assert.IsNotEmpty(_driver.GetRandomName());
            Assert.IsTrue(_driver.GetRandomName() != _driver.GetRandomName());
            Assert.IsTrue(_driver.GetRandomName().EndsWith(Ext));
            Assert.DoesNotThrow(() => PathValidator.ValidateFilePath(_driver.GetRandomName()));

            Assert.IsFalse(_driverEmptyExt.GetRandomName().Contains("."));
        }

        [Test]
        [TestCase("test", Ext)]
        [TestCase("test." + Ext, Ext)]
        [TestCase("test" + Ext, Ext)]
        [TestCase("test", "")]
        [TestCase("test." + Ext, "")]
        [TestCase("test" + Ext, "")]
        public void AddExtention(string name, string extention)
        {
            var driver = GetDriverByExtention(extention);
            TestAddExtention(name, extention, driver);
        }

        [Test]
        [TestCase("test.ex1", Ext)]
        [TestCase("test.ex1." + Ext, Ext)]
        [TestCase("test.ex1" + Ext, Ext)]
        [TestCase("test.ex1", "")]
        [TestCase("test.ex1." + Ext, "")]
        [TestCase("test.ex1" + Ext, "")]
        [TestCase("test", Ext)]
        [TestCase("test." + Ext, Ext)]
        [TestCase("test" + Ext, Ext)]
        [TestCase("test", "")]
        [TestCase("test." + Ext, "")]
        [TestCase("test" + Ext, "")]
        public void AddExtentionRecursively(string name, string extention)
        {
            var driver = new StorageDriverFacade(Ext, GetDriverByExtention(extention));
            TestAddExtention(name, Ext, driver);
        }

        private StorageDriverFacade GetDriverByExtention(string extention)
        {
            return string.IsNullOrEmpty(extention) ? _driverEmptyExt : _driver;
        }


        private void TestAddExtention(string name, string extention, StorageDriverFacade driver)
        {
            driver.Exists(name);
            driver.Read(name);
            driver.Remove(name);
            driver.Write(name);

            _baseDriver.Received(1).Exists(Arg.Is<string>(x => CheckResultName(x, name, extention)));
            _baseDriver.Received(1).Read(Arg.Is<string>(x => CheckResultName(x, name, extention)));
            _baseDriver.Received(1).Remove(Arg.Is<string>(x => CheckResultName(x, name, extention)));
            _baseDriver.Received(1).Write(Arg.Is<string>(x => CheckResultName(x, name, extention)));
        }

        private static bool CheckResultName(string name, string baseName, string extention)
        {
            if (!string.IsNullOrEmpty(extention) && !baseName.Contains("." + extention)) return name == baseName + "." + extention;
            return name == baseName;
        }

        private static readonly string[] _enumTestArray = {"x1.ololo", "test." + Ext, "test2" + Ext, "test3." + Ext, "index.xml"};

        [Test]
        public void Enum()
        {
            EnumTesting(_driver, new[] { _enumTestArray[1], _enumTestArray[3] });
        }

        [Test]
        public void EnumExtentionEmpty()
        {
            EnumTesting(_driverEmptyExt, new[] { _enumTestArray[2] });
        }

        private void EnumTesting(IStorageDriver driver, IEnumerable<string> resultArray)
        {
            _baseDriver.Enum().Returns(_enumTestArray);
            Assert.IsTrue(driver.Enum().ScrambledEquals(resultArray));
        }
    }
}
