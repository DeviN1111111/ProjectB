using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Linq;
using System.Collections.Generic;

public static class OrderAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();

    public static IEnumerable<OrdersModel> GetAllOrders()
    {
        return _connection.Query<OrdersModel>("SELECT * FROM Orders");
    }
    public static DateTime GetDateOfFirstOrder()
    {
        return _connection.ExecuteScalar<DateTime>(
            @"SELECT MIN(Date) FROM Orders;"
        );
    }

    public static OrdersModel? GetMostSoldProductAfterDate(DateTime startDate, DateTime endDate)
    {
        return _connection.QueryFirstOrDefault<OrdersModel>(
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
        int count = _connection.ExecuteScalar<int>(
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
        var results = _connection.Query<(int ProductID, int SoldCount)>(
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

        var sales = _connection.Query<(int ProductID, DateTime Date, int UserID, int SoldCount)>(
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

        // get productid userid and date
        var orderData = _connection.QueryFirstOrDefault<OrdersModel>(
            @"SELECT ProductID, UserID, Date
            FROM Orders
            WHERE ProductID = @ProductID
            LIMIT 1;",
            new { ProductID = productId }
        );

        if (orderData == null)
            return null; // return null if there is not atleast 1 order

        // count the amount of the same productid is sold
        int soldCount = _connection.ExecuteScalar<int>(
            @"SELECT COUNT(*) FROM Orders WHERE ProductID = @ProductID;",
            new { ProductID = productId }
        );

        // get the whole product for the dto
        var product = _connection.QueryFirstOrDefault<ProductModel>(
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

    _connection.Execute(@"
        INSERT INTO Orders (UserID, OrderId, ProductId, Price)
        VALUES (@UserID, @OrderId, @ProductId, @Price);",
        new { UserID = userId, OrderId = orderId, ProductId = productId, Price = price }
    );
    }

    public static List<OrdersModel> GetOrdersByOrderId(int orderId)
    {
        var query = @"
            SELECT * FROM Orders
            WHERE OrderId = @OrderId;
        ";
        return _connection.Query<OrdersModel>(query, new { OrderId = orderId }).AsList();
    }
    public static List<ProductModel> GetTop5MostBoughtProducts(int userId)
    {
        return GetMostBoughtProducts(userId, 5);
    }

    public static List<ProductModel> GetMostBoughtProducts(int userId, int limit)
    {

        var productCounts = _connection.Query<(int ProductId, int Count)>(@"
            SELECT ProductID, COUNT(ProductID) AS Count
            FROM Orders
            WHERE UserID = @UserId
            GROUP BY ProductID
            ORDER BY Count DESC;",
            new { UserId = userId }).AsList();

        var products = new List<ProductModel>();

        foreach (var item in productCounts)
        {
            var discount = _connection.QueryFirstOrDefault<string>(
                "SELECT * FROM Discounts WHERE ProductId = @ProductId",
                new { ProductId = item.ProductId }
            );

            var reward = RewardItemsAccess.GetRewardItemByProductId(item.ProductId);

            // Skip items that are already discounted or rewards
            if (discount != null || reward != null)
                continue;

            var product = _connection.QueryFirstOrDefault<ProductModel>(
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
        double revenue = _connection.ExecuteScalar<double>(
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
        double purchaseCost = _connection.ExecuteScalar<double>(
            @"SELECT SUM(CostPerUnit * QuantityAdded)
            FROM RestockHistory
            WHERE DATE(RestockDate) >= DATE(@Start)
            AND DATE(RestockDate) <= DATE(@End);
            ",
            new { Start = start, End = end }
        );
        return purchaseCost;
    }
    public static void RemoveAllProductsFromOrder(int orderId)
    {
        const string sql = @"DELETE FROM Orders WHERE OrderId = @OrderId;";
        _connection.Execute(sql, new { OrderId = orderId });
    }
    public static int GetProductQuantityInOrder(int orderId, int productId)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM Orders
            WHERE OrderId = @OrderId AND ProductID = @ProductId;
        ";

        return _connection.ExecuteScalar<int>(sql, new { OrderId = orderId, ProductId = productId });
    }

    public static void RemoveProductFromOrder(int orderId, int productId)
    {
        const string sql = @"
            DELETE FROM Orders
            WHERE OrderId = @OrderId AND ProductID = @ProductId;
        ";

        _connection.Execute(sql, new { OrderId = orderId, ProductId = productId });
    }
    public static void RemoveProductQuantityFromOrder(int orderId, int productId, int quantity)
    {
        const string sql = @"
            DELETE FROM Orders
            WHERE ID IN (
                SELECT ID
                FROM Orders
                WHERE OrderId = @OrderId AND ProductID = @ProductId
                LIMIT @Quantity
            );
        ";

        _connection.Execute(sql, new { OrderId = orderId, ProductId = productId, Quantity = quantity });
    }
}
