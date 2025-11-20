using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Collections.Generic;

namespace SupermarktAppTests
{
    [TestClass]
    public class OrderLogicTests
    {

        [TestMethod]
        
        public void AddToCart_NewProduct_AddsToCart()
        {   
            DatabaseFiller.RunDatabaseMethods();
            // Arrange
            var product = new ProductModel { ID = 1, Name = "TestProduct", Quantity = 10, Price = 5.0};
            int initialQuantity = 2;

            // Act
            OrderLogic.AddToCart(product, initialQuantity);

            // Assert
            var allUserProducts = CartAccess.GetAllUserProducts(SessionManager.CurrentUser.ID);
            Assert.AreEqual(1, allUserProducts.Count);
            Assert.AreEqual(product.ID, allUserProducts[0].ProductId);
            Assert.AreEqual(initialQuantity, allUserProducts[0].Quantity);
        }

        [TestMethod]
        public void AddToCart_ExistingProduct_IncreasesQuantityUpToLimit()
        {
            // Arrange
            var product = new ProductModel { ID = 2, Name = "LimitTest", Quantity = 100, Price = 2.0 };
            // First add an item with quantity 50
            OrderLogic.AddToCart(product, 50);

            // Act
            OrderLogic.AddToCart(product, 60); // Should be capped at 99

            // Assert
            var allUserProducts = CartAccess.GetAllUserProducts(SessionManager.CurrentUser.ID);
            Assert.AreEqual(1, allUserProducts.Count);
            Assert.AreEqual(product.ID, allUserProducts[0].ProductId);
            Assert.AreEqual(99, allUserProducts[0].Quantity);
        }

    }
}
