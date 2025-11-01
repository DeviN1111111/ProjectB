namespace SupermarktAppTests
{
    [TestClass]
    public class TestOrderHistoryDisplay
    {
        [TestMethod]
        public void TestOrderHistoryDisplay_ShouldPass()
        {
            int expected = 5;
            int actual = 5;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOrderHistoryDisplay_ShouldFail()
        {
            int expected = 5;
            int actual = 10;
            Assert.AreEqual(expected, actual);
        }
    }
}