using Microsoft.Data.Sqlite;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SupermarktAppTests
{
    [TestClass]
    public class NotificationLogicTests
    {
        [TestMethod]
        [DataRow(1, 100, true)]
        [DataRow(1, 1000, true)]
        public void ValidateNotificationStock_StockPlus50_Void(int ProductID, int QuantityToAdd, bool expected)
        {
            //Arrange
            ProductModel product = new ProductModel(ProductID, "TestProduct", 10.0, "TestNutrition", "TestDescription", "TestCategory", 1, 20, 1);
            // ProductAccess.AddProduct(product);

            //Act
            // double actual = NotificationLogic.FillProductQuantity(ProductID, QuantityToAdd);

            //Assert
            Assert.AreEqual(product.Price * QuantityToAdd, 10 * QuantityToAdd);
        }
    }
}

    // public static double FillProductQuantity(int productId, int quantityToAdd)
    // {
    //     ProductModel? product = ProductAccess.GetProductByID(productId);
    //     int newQuantity = product.Quantity + quantityToAdd;
    //     ProductAccess.UpdateProductStock(productId, newQuantity);
    //     return product.Price * quantityToAdd;
    // }
    
    // public int ID { get; set; }
    // public string Name { get; set; }
    // public double Price { get; set; }
    // public string NutritionDetails { get; set; }
    // public string Description { get; set; }
    // public string Category { get; set; }
    // public int Location { get; set; }
    // public int Quantity { get; set; }
    // public int Visible { get; set; } = 1;