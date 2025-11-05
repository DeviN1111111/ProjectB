

namespace SupermarktAppTests
{
    [TestClass]
    public class ProductVisibilityTests
    {
        [TestMethod]
        public void SetProductVisibility_ShouldHideProduct_WhenFalse()
        {
            // Arrange
            var product = new ProductModel
            {
                ID = 1,
                Name = "Apple",
                Visible = 1
            };

            // Act
            product.Visible = 0;

            // Assert
            Assert.AreEqual(0, product.Visible, "Product should be hidden when visibility is set to false.");
        }

        [TestMethod]
        public void SetProductVisibility_ShouldShowProduct_WhenTrue()
        {
            // Arrange
            var product = new ProductModel
            {
                ID = 2,
                Name = "Banana",
                Visible = 1
            };

            // Act
            product.Visible = 1;

            // Assert
            Assert.AreEqual(1, product.Visible, "Product should be visible when visibility is set to true.");
        }
    }
}
