using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}