using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace SupermarktAppTests
{
    [TestClass]
    public class OrderTest
    {
        [TestClass]
        public class OrderLogicTests
        {
            [TestMethod]
            public void DeliveryFee_ShouldBeZero_WhenTotalIs25OrMore()
            {
                double result = OrderLogic.DeliveryFee(30);
                Assert.AreEqual(0, result);
            }

            [TestMethod]
            public void DeliveryFee_ShouldBeFive_WhenTotalIsBelow25()
            {
                double result = OrderLogic.DeliveryFee(10);
                Assert.AreEqual(5, result);
            }

            [TestMethod]
            public void DeliveryFee_ShouldBeZero_WhenTotalIsZero()
            {
                double result = OrderLogic.DeliveryFee(0);
                Assert.AreEqual(0, result);
            }
        }
    }
}