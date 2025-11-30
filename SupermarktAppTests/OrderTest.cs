using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace SupermarktAppTests
{
    [TestClass]
    public class OrderLogicTests
    {

        [TestMethod]
        [DataRow("test@gmail.com", "password1")]
        public void AddToCart_ValidProduct_AddsCorrectItemAndQuantity(string email, string password)
        {
            // Arrange - Add new test user and product to database
            var random = new Random();
            DatabaseFiller.RunDatabaseMethods();
                        DateTime birthdate = new DateTime(1990, 1, 1);
            UserModel user = new UserModel("Test", "User", email, password,
                                           "Test Address", "1234AB", "1234567890",
                                           birthdate, "Test City");

            LoginAccess.Register(user);

            var product = new ProductModel{
                    Name = "Test Product",
                    Price = Math.Round(random.NextDouble() * 10 + 1, 2),
                    NutritionDetails = $"Nutrition details for Test Product",
                    Description = $"Description for Test Product",
                    Category = "Test Category",
                    Location = random.Next(1, 16),
                    Quantity = random.Next(20, 300),
                };
            ProductLogic.AddProduct(product.Name, product.Price, product.NutritionDetails, product.Description, product.Category, product.Location, product.Quantity, 1);

            // Act
            var result = LoginLogic.GetUserByEmail(email);
            var addedProduct = ProductAccess.GetProductByName(product.Name);
            
            CartAccess.AddToCart(result.ID, addedProduct.ID, 2);  // Add new product to cart with quantity 2

            // Assert
            var cart = CartAccess.GetAllUserProducts(result.ID);

            Assert.HasCount(1, cart);
            Assert.AreEqual(addedProduct.ID, cart[0].ProductId);
            Assert.AreEqual(2, cart[0].Quantity);
        }


        [TestMethod]
        [DataRow("test@gmail.com", "password1")]
        public void Checkout_CartHasItems_CanProceed(string email, string password)
        {
            // Arrange - Add new test user and product to database
            var random = new Random();
            DatabaseFiller.RunDatabaseMethods();
                        DateTime birthdate = new DateTime(1990, 1, 1);
            UserModel user = new UserModel("Test", "User", email, password,
                                           "Test Address", "1234AB", "1234567890",
                                           birthdate, "Test City");

            LoginAccess.Register(user);

            var product = new ProductModel{
                    Name = "Test Product",
                    Price = Math.Round(random.NextDouble() * 10 + 1, 2),
                    NutritionDetails = $"Nutrition details for Test Product",
                    Description = $"Description for Test Product",
                    Category = "Test Category",
                    Location = random.Next(1, 16),
                    Quantity = random.Next(20, 300),
                };
            ProductLogic.AddProduct(product.Name, product.Price, product.NutritionDetails, product.Description, product.Category, product.Location, product.Quantity, 1);

            // Act
            var result = LoginLogic.GetUserByEmail(email);
            var addedProduct = ProductAccess.GetProductByName(product.Name);
            
            CartAccess.AddToCart(result.ID, addedProduct.ID, 2);  // Add new product to cart with quantity 2
            
            // Assert
            var cart = CartAccess.GetAllUserProducts(result.ID);
            Assert.IsNotEmpty(cart);  // cart contains items, so checkout is possible
        }
    }
}
