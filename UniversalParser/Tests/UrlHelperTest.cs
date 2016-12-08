namespace Tests
{
    using Base.Helpers;
    using NUnit.Framework;

    [TestFixture]
    public class UrlHelperTest
    {
        [Test]
        public void IsValidDomain_Positive()
        {
            Assert.IsTrue(UrlHelpers.IsValidDomain("http://domain.com"));
        }

        [Test]
        [TestCase("domain.com")]
        [TestCase("tp://domain.com")]
        [TestCase("ftp://domain.com")]
        [TestCase("http://domain.com/")]
        [TestCase("http://domain.com/dfgd")]
        public void IsValidDomain_Negative(string domain)
        {
            Assert.IsFalse(UrlHelpers.IsValidDomain(domain));
        }
    }
}
