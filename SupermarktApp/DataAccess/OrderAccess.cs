using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Linq;
using System.Collections.Generic;

public static class OrderAccess
{
    private const string ConnectionString = "Data Source=database.db";

    public static IEnumerable<OrdersModel> GetAllOrders()
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.Query<OrdersModel>("SELECT * FROM Orders");
    }
    public static DateTime GetDateOfFirstOrder()
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.ExecuteScalar<DateTime>(
            @"SELECT MIN(Date) FROM Orders;"
        );
    }

    public static OrdersModel? GetMostSoldProductAfterDate(DateTime startDate, DateTime endDate)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QueryFirstOrDefault<OrdersModel>(
            @"SELECT ProductID, COUNT(*) AS SoldCount
              FROM Orders
              WHERE DATE(Date) >= DATE(@StartDate) AND DATE(Date) <= DATE(@EndDate)
              GROUP BY ProductID
              ORDER BY SoldCount DESC
              LIMIT 1;",
            new { StartDate = startDate, EndDate = endDate }
        );
    }
    public static int GetMostSoldCountUpToDate(DateTime startDate, DateTime endDate)
    {
        using var db = new SqliteConnection(ConnectionString);
        int count = db.ExecuteScalar<int>(
            @"SELECT COUNT(*) AS SoldCount
              FROM Orders
              WHERE ProductID = (
                  SELECT ProductID
                  FROM Orders
                  WHERE DATE(Date) >= DATE(@StartDate) AND DATE(Date) <= DATE(@EndDate)
                  GROUP BY ProductID
                  ORDER BY COUNT(*) DESC
                  LIMIT 1
              );",
            new { StartDate = startDate, EndDate = endDate }
        );
        return count;
    }

    public static List<(int ProductID, int SoldCount)> GetTop5MostSoldProductsUpToDate(DateTime startDate, DateTime endDate)
    {
        using var db = new SqliteConnection(ConnectionString);
        var results = db.Query<(int ProductID, int SoldCount)>(
            @"SELECT ProductID, COUNT(*) AS SoldCount
            FROM Orders
            WHERE DATE(Date) >= DATE(@StartDate) AND DATE(Date) <= DATE(@EndDate)
            GROUP BY ProductID
            ORDER BY SoldCount DESC
            LIMIT 5;",
            new { StartDate = startDate, EndDate = endDate }
        ).ToList();

        return results;
    }
    public static List<ProductSalesDto> SeedProductSalesDto(DateTime startDate, DateTime endDate)
    {
        using var db = new SqliteConnection(ConnectionString);

        var sales = db.Query<(int ProductID, DateTime Date, int UserID, int SoldCount)>(
            @"SELECT ProductID, DATE(Date) AS Date, UserID, COUNT(*) AS SoldCount
            FROM Orders
            WHERE DATE(Date) >= DATE(@StartDate) AND DATE(Date) <= DATE(@EndDate)
            GROUP BY ProductID, DATE(Date), UserID",
            new { StartDate = startDate, EndDate = endDate }
        );

        var result = new List<ProductSalesDto>();

        foreach (var s in sales)
        {
            var product = ProductAccess.GetProductByID(s.ProductID);
            if (product != null)
            {
                result.Add(new ProductSalesDto
                {
                    Product = product,
                    SoldCount = s.SoldCount,
                    SaleDate = s.Date,
                    UserID = s.UserID
                });
            }
        }

        return result;
    }


    public static ProductSalesDto? GetSalesOfSingleProductByID(int productId)
    {
        using var db = new SqliteConnection(ConnectionString);

        // get productid userid and date
        var orderData = db.QueryFirstOrDefault<OrdersModel>(
            @"SELECT ProductID, UserID, Date
            FROM Orders
            WHERE ProductID = @ProductID
            LIMIT 1;",
            new { ProductID = productId }
        );

        if (orderData == null)
            return null; // return null if there is not atleast 1 order

        // count the amount of the same productid is sold
        int soldCount = db.ExecuteScalar<int>(
            @"SELECT COUNT(*) FROM Orders WHERE ProductID = @ProductID;",
            new { ProductID = productId }
        );

        // get the whole product for the dto
        var product = db.QueryFirstOrDefault<ProductModel>(
            @"SELECT * FROM Products WHERE ID = @ProductID;",
            new { ProductID = productId }
        );

        if (product == null)
            return null;

        // Create aDTO to use in the statistics
        return new ProductSalesDto
        {
            Product = product,
            SoldCount = soldCount,
            SaleDate = orderData.Date,
            UserID = orderData.UserID
        };
    }

 public static void AddToOrders(int userId, int orderId, int productId, double price)
    {
    using var db = new SqliteConnection(ConnectionString);

    db.Execute(@"
        INSERT INTO Orders (UserID, OrderId, ProductId, Price)
        VALUES (@UserID, @OrderId, @ProductId, @Price);",
        new { UserID = userId, OrderId = orderId, ProductId = productId, Price = price }
    );
    }

    public static List<OrdersModel> GetOrderssByOrderId(int orderId)
    {
        using var connection = new SqliteConnection(ConnectionString);
        var query = @"
            SELECT * FROM Orders
            WHERE OrderId = @OrderId;
        ";
        return connection.Query<OrdersModel>(query, new { OrderId = orderId }).AsList();
    }
    public static List<ProductModel> GetTop5MostBoughtProducts(int userId)
    {
        return GetMostBoughtProducts(userId, 5);
    }

    public static List<ProductModel> GetMostBoughtProducts(int userId, int limit)
    {
        using var db = new SqliteConnection(ConnectionString);

        var productCounts = db.Query<(int ProductId, int Count)>(@"
            SELECT ProductID, COUNT(ProductID) AS Count
            FROM Orders
            WHERE UserID = @UserId
            GROUP BY ProductID
            ORDER BY Count DESC;",
            new { UserId = userId }).AsList();

        var products = new List<ProductModel>();

        foreach (var item in productCounts)
        {
            var discount = db.QueryFirstOrDefault<string>(
                "SELECT * FROM Discounts WHERE ProductId = @ProductId",
                new { ProductId = item.ProductId }
            );

            var reward = RewardItemsAccess.GetRewardItemByProductId(item.ProductId);

            // Skip items that are already discounted or rewards
            if (discount != null || reward != null)
                continue;

            var product = db.QueryFirstOrDefault<ProductModel>(
                "SELECT * FROM Products WHERE Id = @Id",
                new { Id = item.ProductId });

            if (product != null)
                products.Add(product);

            if (products.Count == limit)
                break;
        }

        return products;
    }

        public static double GetTotalRevenue(DateTime startDate, DateTime endDate)
    {
        using var db = new SqliteConnection(ConnectionString);
        double revenue = db.ExecuteScalar<double>(
            @"SELECT SUM(Products.Price)
            FROM Orders
            JOIN Products ON Orders.ProductID = Products.ID
            WHERE DATE(Orders.Date) >= DATE(@StartDate)
            AND DATE(Orders.Date) <= DATE(@EndDate);
            ",
            new { StartDate = startDate, EndDate = endDate }
        );
        return revenue;
    }
    public static double GetTotalPurchaseCost(DateTime start, DateTime end)
    {
        using var db = new SqliteConnection(ConnectionString);
        double purchaseCost = db.ExecuteScalar<double>(
            @"SELECT SUM(CostPerUnit * QuantityAdded)
            FROM RestockHistory
            WHERE DATE(RestockDate) >= DATE(@Start)
            AND DATE(RestockDate) <= DATE(@End);
            ",
            new { Start = start, End = end }
        );
        return purchaseCost;
    }




}
