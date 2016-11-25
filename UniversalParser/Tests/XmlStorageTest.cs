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
        public void TestStorageCtorArguments(string path, IStorageDriver driver, Type exceptionType)
        {
            if (exceptionType == null) Assert.DoesNotThrow(() => new XmlStorage(path, driver));
            else Assert.Catch(exceptionType, () => new XmlStorage(path, driver));
        }

        [Test]
        public void TestStorageCtorInitialize()
        {
            var path = "testDir";
            var driver = Substitute.For<IStorageDriver>();
            var storage = new XmlStorage(path, driver);
            driver.Received(1).DirectoryCreate(Arg.Is(path));
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

            driver.Received(1).FileWrite(Arg.Any<DataInfo>(), Arg.Is<string>(x => !string.IsNullOrWhiteSpace(x)));
        }
    }
}
