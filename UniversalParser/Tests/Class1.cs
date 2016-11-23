using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    using System.Xml.Serialization;
    using Base.Utilities;
    using NUnit.Framework;

    [TestFixture]
    public sealed class PathValidatorTest
    {
        [Test]
        [TestCase(null, typeof(ArgumentNullException))]
        [TestCase("", typeof(ArgumentException))]
        [TestCase(@"\\file.ext", typeof(ArgumentException))]
        [TestCase(@"X:\file.ext", typeof(ArgumentException))]
        public void TestValidateFilePath_Negative(string path, Type expectedException)
        {
            Assert.Catch(expectedException, () => PathValidator.ValidateFilePath(path));
        }

        [Test]
        [TestCase("xxx.ext")]
        [TestCase(@"C:\xxx.ext")]
        public void TestValidateFilePath_Positive(string path)
        {
            Assert.DoesNotThrow(() => PathValidator.ValidateFilePath(path));
        }
    }
}
