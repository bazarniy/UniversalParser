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
    public class XmlStorageTest
    {
        /*
         Write->файл с рандомным именем создан
         Write->файл по базовому адресу создан
             */

        [Test]
        public void TestStorageCreate()
        {
            var path = "testDir";
            var driver = Substitute.For<IStorageDriver>();

            Assert.DoesNotThrow(() => new XmlStorage(path, driver));
            driver.Received(1).DirectoryCreate(Arg.Is<string>(x => x == path));
            Assert.Throws<ArgumentNullException>(() => new XmlStorage(null, driver));
            Assert.Throws<ArgumentNullException>(() => new XmlStorage(path, null));
            Assert.Throws<ArgumentException>(() => new XmlStorage("invalid:path", driver));
        }

        [Test]
        public void TestWriteData()
        {
            var path = "testDir";
            var driver = Substitute.For<IStorageDriver>();
            var stor = new XmlStorage(path, driver);
            var info = new DataInfo("test data");

            Assert.Throws<ArgumentNullException>(() => stor.Write(null));

            driver.GetRandomFileName().Returns("randomFileName");
            Assert.DoesNotThrow(() => stor.Write(info));
            
            driver.Received(1).FileWrite(Arg.Any<DataInfo>(), Arg.Is<string>(x=>!string.IsNullOrWhiteSpace(x)));
        }
    }
}
