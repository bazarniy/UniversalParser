using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    using System.IO;
    using NUnit.Framework;
    using XmlStorage;

    [TestFixture]
    public class StorageDriverTest
    {
        private string _storagePath = "testStorage";
        private DiskDriver _driver => new DiskDriver(_storagePath);

        [TearDown]
        public void TestTearDown()
        {
            if (Directory.Exists(_storagePath)) Directory.Delete(_storagePath, true);
        }

        [Test]
        [TestCase("testDir", null)]
        [TestCase("", null)]
        [TestCase(null, typeof(ArgumentNullException))]
        [TestCase("invalid<:path", typeof(ArgumentException))]
        public void CtorArguments(string path, Type exceptionType)
        {
            if (exceptionType == null)
            {
                Assert.DoesNotThrow(() => new DiskDriver(path));
            }
            else
            {
                Assert.Catch(exceptionType, () => new DiskDriver(path));
            }
        }

        [Test]
        public void DirectoryCreate()
        {
            DiskDriver x;
            Assert.DoesNotThrow(() => x = _driver);
            Assert.DoesNotThrow(() => x = _driver);
        }

        [Test]
        public void IODirectoryRemove()
        {
            var dir1 = Directory.CreateDirectory(_storagePath);
            var dir2 = dir1.CreateSubdirectory(_storagePath);
            var dir3 = dir2.CreateSubdirectory(_storagePath);
            File.WriteAllText(Path.Combine(dir3.FullName, "file1"), "");
            File.WriteAllText(Path.Combine(dir2.FullName, "file1"), "");
            File.WriteAllText(Path.Combine(dir2.FullName, "file2"), "");
            File.WriteAllText(Path.Combine(dir1.FullName, "file1"), "");

            Assert.DoesNotThrow(() => Directory.Delete(_storagePath, true));
        }

        [Test]
        public void GetRandomFileName()
        {
            var x = _driver.GetRandomName();
            Assert.IsNotEmpty(x);
            Assert.IsTrue(x != _driver.GetRandomName());
        }

        [Test]
        [TestCase("testDir", null)]
        [TestCase("", typeof(ArgumentException))]
        [TestCase("testDir\\", typeof(ArgumentException))]
        [TestCase(null, typeof(ArgumentNullException))]
        [TestCase("invalid<:path", typeof(ArgumentException))]
        public void FileExistArguments(string path, Type exceptionType)
        {
            if (exceptionType != null)
            {
                Assert.Catch(exceptionType, () => _driver.Exists(path));
            }
            else
            {
                Assert.DoesNotThrow(() => _driver.Exists(path));
            }
        }

        [Test]
        [TestCase("testfile.xxx")]
        [TestCase("C:\\testfile.xxx")]
        public void FileNotExist(string path)
        {
            Assert.IsFalse(_driver.Exists(path));
        }

        [Test]
        [TestCase("testfile.xxx")]
        [TestCase("C:\\testfile.xxx")]
        public void FileExist(string path)
        {
            var d = _driver;
            const string file = "testfile.xxx";

            File.WriteAllText(Path.Combine(_storagePath, file), "");
            Assert.IsTrue(d.Exists(path));
        }

        [Test]
        public void FileEnum()
        {
            var d = _driver;
            Assert.IsEmpty(d.Enum());
            File.WriteAllText(Path.Combine(_storagePath, "test"), "");
            Assert.IsNotEmpty(d.Enum());
        }

        [Test]
        [TestCase("testDir", null)]
        [TestCase("", typeof(ArgumentException))]
        [TestCase("testDir\\", typeof(ArgumentException))]
        [TestCase(null, typeof(ArgumentNullException))]
        [TestCase("invalid<:path", typeof(ArgumentException))]
        public void FileRemove(string path, Type exceptionType)
        {
            if (exceptionType != null)
            {
                Assert.Catch(exceptionType, () => _driver.Remove(path));
            }
            else
            {
                Assert.DoesNotThrow(() => _driver.Remove(path));
            }
        }

        [Test]
        public void IOFileRemove()
        {
            var d = _driver;
            Assert.DoesNotThrow(() => File.Delete(Path.Combine(_storagePath, "trest.file")));
        }

        [Test]
        [TestCase("testfile.xxx")]
        [TestCase("C:\\testfile.xxx")]
        public void FileRemove(string path)
        {
            var d = _driver;
            const string file = "testfile.xxx";

            Assert.DoesNotThrow(() => d.Remove(path));
            File.WriteAllText(Path.Combine(_storagePath, file), "");
            Assert.DoesNotThrow(()=>d.Remove(path));
        }

        [Test]
        [TestCase("testDir", null)]
        [TestCase("", typeof(ArgumentException))]
        [TestCase("testDir\\", typeof(ArgumentException))]
        [TestCase(null, typeof(ArgumentNullException))]
        [TestCase("invalid<:path", typeof(ArgumentException))]
        public void FileWriteArguments(string path, Type exceptionType)
        {
            if (exceptionType != null)
            {
                Assert.Catch(exceptionType, () => _driver.Write(path).Close());
            }
            else
            {
                Assert.DoesNotThrow(() => _driver.Write(path).Close());
            }
        }

        [Test]
        public void FileWrite()
        {
            var d = _driver;
            const string file = "testfile.xxx";

            Assert.DoesNotThrow(() => d.Write(file).Close());
            Assert.IsTrue(File.Exists(Path.Combine(_storagePath, file)));
            File.WriteAllText(Path.Combine(_storagePath, file), "");
            Assert.DoesNotThrow(() => d.Write(file).Close());
        }

        [Test]
        public void IOFileWrite()
        {
            var d = _driver;

            File.WriteAllText(Path.Combine(_storagePath, "trest.file"), "");
            Assert.DoesNotThrow(() => File.Create(Path.Combine(_storagePath, "trest.file")).Close());
            Assert.IsTrue(File.Exists(Path.Combine(_storagePath, "trest.file")));
        }

        [Test]
        [TestCase("testDir", null)]
        [TestCase("", typeof(ArgumentException))]
        [TestCase("testDir\\", typeof(ArgumentException))]
        [TestCase(null, typeof(ArgumentNullException))]
        [TestCase("invalid<:path", typeof(ArgumentException))]
        public void FileReadArguments(string path, Type exceptionType)
        {
            if (exceptionType != null)
            {
                Assert.Catch(exceptionType, () => _driver.Read(path).Close());
            }
            else
            {
                Assert.DoesNotThrow(() => _driver.Read(path).Close());
            }
        }

        [Test]
        public void FileRead()
        {
            var d = _driver;
            const string file = "testfile.xxx";

            Assert.IsTrue(d.Read(file).Length==0);
            File.WriteAllText(Path.Combine(_storagePath, file), "");
            Assert.DoesNotThrow(() => d.Read(file).Close());
        }

        [Test]
        public void GetLength()
        {
            var d = _driver;
            const string file = "testfile.xxx";

            File.WriteAllText(Path.Combine(_storagePath, file), "123");

            Assert.IsTrue(d.GetLength(file) == 3);
            Assert.IsTrue(d.GetLength("ololo") == 0);
        }
    }
}
