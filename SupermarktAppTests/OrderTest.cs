using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Linq;


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

        [TestMethod]
        [DataRow("test@gmail.com", "password1")]
        public void PlaceOrder_ValidCart_CreatesOrderWithItems(string email, string password)
        {
            // Arrange - Add new test user and product to database
            var random = new Random();
            DatabaseFiller.RunDatabaseMethods();
                        DateTime birthdate = new DateTime(1990, 1, 1);
            UserModel user = new UserModel("Test", "User", email, password,
                                           "Test Address", "1234AB", "1234567890",
                                           birthdate, "Test City");

            LoginAccess.Register(user);

            var testProduct1 = new ProductModel{
                    Name = "Test Product1",
                    Price = Math.Round(random.NextDouble() * 10 + 1, 2),
                    NutritionDetails = $"Nutrition details for Test Product",
                    Description = $"Description for Test Product",
                    Category = "Test Category",
                    Location = random.Next(1, 16),
                    Quantity = random.Next(20, 300),
                };
            var testProduct2 = new ProductModel{
                    Name = "Test Product2",
                    Price = Math.Round(random.NextDouble() * 10 + 1, 2),
                    NutritionDetails = $"Nutrition details for Test Product",
                    Description = $"Description for Test Product",
                    Category = "Test Category",
                    Location = random.Next(1, 16),
                    Quantity = random.Next(20, 300),
                };
            ProductLogic.AddProduct(testProduct1.Name, testProduct1.Price, testProduct1.NutritionDetails, testProduct1.Description, testProduct1.Category, testProduct1.Location, testProduct1.Quantity, 1);
            ProductLogic.AddProduct(testProduct2.Name, testProduct2.Price, testProduct2.NutritionDetails, testProduct2.Description, testProduct2.Category, testProduct2.Location, testProduct2.Quantity, 1);

            // Act
            var result = LoginLogic.GetUserByEmail(email);
            SessionManager.CurrentUser = result;

            // Get added products from database
            var product1 = ProductAccess.GetProductByName(testProduct1.Name);
            var product2 = ProductAccess.GetProductByName(testProduct2.Name);


            CartAccess.AddToCart(result.ID, product1.ID, 2);  // Add new product to cart with quantity 2
            CartAccess.AddToCart(result.ID, product2.ID, 3);  // Add new product to cart with quantity 3
            OrderLogic.ProcessPay(CartAccess.GetAllUserProducts(result.ID), ProductAccess.GetAllProducts(), null);

            // Assert
            var orderHistory = OrderHistoryAccess.GetOrderByUserId(result.ID);
            Assert.IsNotNull(orderHistory);

            var orderItems = OrderAccess.GetOrderssByOrderId(orderHistory.Id);

            Assert.HasCount(5, orderItems); // 5 items purchased (2 of product1 and 3 of product2)
            Assert.IsTrue(orderItems.Exists(item => item.ProductID == product1.ID));  // Check that product1 is in the order items
            Assert.IsTrue(orderItems.Exists(item => item.ProductID == product2.ID));
        }
    }
}
