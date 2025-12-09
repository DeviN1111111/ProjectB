using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SupermarktAppTests
{
    [TestClass]
    public class ReturnItemTest
    {
        [TestMethod]
        public void CheckReturnableOrders_FiltersCorrectly()
        {
            var today = new DateTime(2025, 2, 5);

            var orders = new List<OrderHistoryModel>
            {
                new() { Date = today, IsPaid = true },
                new() { Date = today.AddDays(-2), IsPaid = true },
                new() { Date = today.AddDays(-4), IsPaid = true },
                new() { Date = today.AddDays(-1), IsPaid = false }                
            };

            var result = ReturnItemLogic.CheckReturnableOrders(orders, today);

            Assert.HasCount(2, result);
        }
        [TestMethod]
        public void CheckReturnableProducts_ReturnsProductsForGivenOrder()
        {
            var userId = 1;

            // Create an orderHistory row
            var orderHistoryId = OrderHistoryAccess.AddToOrderHistory(userId, isPaid: true);

            // Create two products
            var p1 = new ProductModel("Banana", 0.98, "", "", "Fruit", 1, 10, 1);
            var p2 = new ProductModel("Apple", 1.50, "", "", "Fruit", 1, 10, 1);

            ProductAccess.AddProduct(p1);
            ProductAccess.AddProduct(p2);

            // Reload them so we have their generated IDs
            var dbP1 = ProductAccess.GetProductByName("Banana")!;
            var dbP2 = ProductAccess.GetProductByName("Apple")!;

            // Add order lines for this order
            OrderAccess.AddToOrders(userId, orderHistoryId, dbP1.ID, dbP1.Price);
            OrderAccess.AddToOrders(userId, orderHistoryId, dbP2.ID, dbP2.Price);

            var orderHistory = OrderHistoryAccess.GetOrderById(orderHistoryId);

            var result = ReturnItemLogic.CheckReturnableProducts(orderHistory);
            var resultIds = result.Select(p => p.ID).ToList();

            Assert.HasCount(2, result);
            CollectionAssert.AreEquivalent(new List<int> { dbP1.ID, dbP2.ID }, resultIds);
        }
    }
}