using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SupermarktAppTests
{
    [TestClass]
    public class NotificationLogicTests
    {
        [TestMethod]
        [DataRow(10000, 50)]
        [DataRow(100000, 500)]
        public void ValidateNotificationStock_StockPlus50_Void(int ProductID, int QuantityToAdd)
        {
            //Arrange
            DatabaseFiller.RunDatabaseMethods();
            string productName = "TestProduct";
            ProductModel product = new ProductModel(ProductID, productName, 10.0, "TestNutrition", "TestDescription", "TestCategory", 1, 0, 1);
            ProductAccess.AddProductUnitTest(product);

            //Act
            NotificationLogic.FillProductQuantity(ProductID, QuantityToAdd);
            ProductModel updated = ProductAccess.GetProductByName(productName);

            //Assert
            Assert.AreEqual(QuantityToAdd, updated.Quantity);
            // Assert.AreEqual(updated.Name, product.Name);
        }
    }
}