﻿namespace Tests
{
    using System;
    using System.IO;
    using Base.Utilities;
    using NUnit.Framework;

    [TestFixture]
    public sealed class PathValidatorTest
    {
        [Test]
        public void TestValidateFilePath_InvalidCharactersError()
        {
            var wrongFilePath = string.Join(string.Empty, Path.GetInvalidFileNameChars());
            Assert.Catch(typeof(ArgumentException), () => PathValidator.ValidateFilePath(wrongFilePath));

            var wrongDirectoryPath = string.Join(string.Empty, PathValidator.InvalidFolderCharacters());
            Assert.Catch(typeof(ArgumentException), () => PathValidator.ValidateFilePath(wrongDirectoryPath + "\\file.ext"));

            Assert.Catch(typeof(ArgumentException), () => PathValidator.ValidateFilePath(wrongDirectoryPath + "\\" + wrongFilePath));
        }

        [Test]
        [TestCase(null, typeof(ArgumentNullException))]
        [TestCase("", typeof(ArgumentException))]
        [TestCase(@"\\", typeof(ArgumentException))]
        [TestCase(@"\\file.ext", typeof(ArgumentException))]
        [TestCase(@"X:\file.ext", typeof(ArgumentException))]
        [TestCase(@"C:\", typeof(ArgumentException))]
        [TestCase(@"C:\folder\", typeof(ArgumentException))]
        [TestCase(@"C:\file.ext;", typeof(ArgumentException))]
        [TestCase(@"file.ext;", typeof(ArgumentException))]
        public void TestValidateFilePath_Negative(string path, Type expectedException)
        {
            Assert.Catch(expectedException, () => PathValidator.ValidateFilePath(path));
        }

        [Test]
        [TestCase("xxx.ext")]
        [TestCase("xxx")]
        [TestCase(@"xxx\sss")]
        [TestCase(@"C:\xxx.ext")]
        [TestCase(@"C:\folder\xxx.ext")]
        [TestCase(@"C:\folder\xxx")]
        [TestCase(@"C:\folder1\folder2\xxx.ext")]
        public void TestValidateFilePath_Positive(string path)
        {
            Assert.DoesNotThrow(() => PathValidator.ValidateFilePath(path));
        }

        [Test]
        public void TestValidateFolderPath_InvalidCharactersError()
        {
            var wrongDirectoryPath = string.Join(string.Empty, PathValidator.InvalidFolderCharacters());
            Assert.Catch(typeof(ArgumentException), () => PathValidator.ValidateFolderPath(wrongDirectoryPath));
        }

        [Test]
        [TestCase(null, typeof(ArgumentNullException))]
        [TestCase("", typeof(ArgumentException))]
        [TestCase(@"\\", typeof(ArgumentException))]
        [TestCase(@"\\file.ext", typeof(ArgumentException))]
        [TestCase(@"X:\file.ext", typeof(ArgumentException))]
        [TestCase(@"C:\;", typeof(ArgumentException))]
        [TestCase(@"folder\;", typeof(ArgumentException))]
        public void TestValidateFolderPath_Negative(string path, Type expectedException)
        {
            Assert.Catch(expectedException, () => PathValidator.ValidateFolderPath(path));
        }

        [Test]
        [TestCase("xxx.ext")]
        [TestCase("xxx")]
        [TestCase(@"xxx\sss")]
        [TestCase(@"C:\")]
        [TestCase(@"C:\xxx.ext")]
        [TestCase(@"C:\folder\xxx.ext")]
        [TestCase(@"C:\folder\xxx")]
        [TestCase(@"C:\folder1\folder2\xxx.ext")]
        [TestCase(@"C:\folder\")]
        public void TestValidateFolderPath_Positive(string path)
        {
            Assert.DoesNotThrow(() => PathValidator.ValidateFolderPath(path));
        }

        [Test]
        [TestCase("xxx.ext", "xxx.ext")]
        [TestCase("xxx", "xxx")]
        [TestCase(@"xxx\sss", "sss")]
        [TestCase(@"C:\", "")]
        [TestCase(@"C:\xxx.ext", "xxx.ext")]
        [TestCase(@"C:\folder\xxx.ext", "xxx.ext")]
        [TestCase(@"C:\folder\xxx", "xxx")]
        [TestCase(@"C:\folder1\folder2\xxx.ext", "xxx.ext")]
        [TestCase(@"C:\folder\", "")]
        public void TestValidateGetFileName(string path, string result)
        {
            Assert.AreEqual(PathValidator.GetLastPathSegment(path), result);
        }

        [Test]
        [TestCase("xxx.ext", typeof(ArgumentException))]
        [TestCase(@"\\", typeof(ArgumentException))]
        [TestCase(@"\\file.ext", typeof(ArgumentException))]
        [TestCase(@"X:\file.ext", typeof(ArgumentException))]
        [TestCase(@"C:\", typeof(ArgumentException))]
        [TestCase(@"C:\folder\", typeof(ArgumentException))]
        [TestCase(@"C:\file.ext;", typeof(ArgumentException))]
        [TestCase(@"file.ext;", typeof(ArgumentException))]
        [TestCase(@"xxx\sss", typeof(ArgumentException))]
        [TestCase(@"C:\xxx.ext", typeof(ArgumentException))]
        [TestCase(@"C:\folder\xxx.ext", typeof(ArgumentException))]
        [TestCase(@"C:\folder\xxx", typeof(ArgumentException))]
        [TestCase(@"C:\folder1\folder2\xxx.ext", typeof(ArgumentException))]
        [TestCase("ext;", typeof(ArgumentException))]
        [TestCase("ex>t", typeof(ArgumentException))]
        public void TestValidateExtention_Negative(string path, Type expectedException)
        {
            Assert.Catch(expectedException, () => PathValidator.ValidateExtention(path));
        }

        [Test]
        [TestCase("")]
        [TestCase("xxx")]
        public void TestValidateExtention_Positive(string path)
        {
            Assert.DoesNotThrow(() => PathValidator.ValidateExtention(path));
        }
    }
}