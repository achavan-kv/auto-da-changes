using Blue.Cosacs.Merchandising.Calculations;
using NUnit.Framework;

namespace Blue.Cosacs.Test.Merchandising
{
    [TestFixture]
    public class AWCTests
    {
        [TestFixtureSetUp]
        public virtual void Setup()
        {
        }

        [Test]
        public void AWCStandardTest()
        {
            Assert.AreEqual(75, AWC.CalculateAWC(50, 100, 10, 10));
            Assert.AreEqual(37.5, AWC.CalculateAWC(50, 100, 10, -2));
        }

        [Test]
        public void AWCRepoTest()
        {
            Assert.AreEqual(36, AWC.CalculateAWC(40, 40, 8, 2, 50));
            Assert.AreEqual(41378.88, AWC.CalculateAWC(0, 129309 * 0.4M, 0, 1, 20));
            Assert.AreEqual(41378.88, AWC.CalculateAWC(41378.88M, 129309 * 0.4M, 1, -1, 20));
        }

        [Test]
        public void AWCNegativeTotalQuanityTest()
        {
            Assert.AreEqual(50, AWC.CalculateAWC(50, 100, 10, -11));
            Assert.AreEqual(50, AWC.CalculateAWC(50, 100, 10, -10));
        }

        [Test]
        public void AWCStockNegativeTotalQuanityPostiveTest()
        {
            Assert.AreEqual(100, AWC.CalculateAWC(50, 100, -10, 11));
            Assert.AreEqual(100, AWC.CalculateAWC(50, 100, 0, 12));
        }

        [Test]
        public void UserTest()
        {
            Assert.AreEqual(1800, AWC.CalculateAWC(1800, 1800, 21, -2));
        }

        [Test]
        public void VendorReturn()
        {
            Assert.AreEqual(66.667M, decimal.Round(AWC.CalculateAWC(75, 100, 20, -5), 3));
        }
    }
}