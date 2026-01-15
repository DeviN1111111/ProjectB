using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Linq;
using System.Collections.Generic;

public static class OrderItemAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();

    public static IEnumerable<OrderItemsModel> GetAllOrderItems()
    {
        return _connection.Query<OrderItemsModel>("SELECT * FROM OrderItems");
    }
    public static DateTime GetDateOfFirstOrder()
    {
        return _connection.ExecuteScalar<DateTime>(
            @"SELECT MIN(Date) FROM OrderItems;"
        );
    }

    public static OrderItemsModel? GetMostSoldProductAfterDate(DateTime startDate, DateTime endDate)
    {
        return _connection.QueryFirstOrDefault<OrderItemsModel>(
            @"SELECT ProductID, COUNT(*) AS SoldCount
              FROM OrderItems
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
              FROM OrderItems
              WHERE ProductID = (
                  SELECT ProductID
                  FROM OrderItems
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
            FROM OrderItems
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
        var orderData = _connection.QueryFirstOrDefault<OrderItemsModel>(
            @"SELECT ProductID, UserID, Date
            FROM OrderItems
            WHERE ProductID = @ProductID
            LIMIT 1;",
            new { ProductID = productId }
        );

        if (orderData == null)
            return null; // return null if there is not atleast 1 order

        // count the amount of the same productid is sold
        int soldCount = _connection.ExecuteScalar<int>(
            @"SELECT COUNT(*) FROM OrderItems WHERE ProductID = @ProductID;",
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
        INSERT INTO OrderItems (UserID, OrderId, ProductId, Price)
        VALUES (@UserID, @OrderId, @ProductId, @Price);",
        new { UserID = userId, OrderId = orderId, ProductId = productId, Price = price }
    );
    }

    public static List<OrderItemsModel> GetOrderItemsByOrderId(int orderId)
    {
        var query = @"
            SELECT * FROM OrderItems
            WHERE OrderId = @OrderId;
        ";
        return _connection.Query<OrderItemsModel>(query, new { OrderId = orderId }).AsList();
    }
    public static List<ProductModel> GetTop5MostBoughtProducts(int userId)
    {
        return GetMostBoughtProducts(userId, 5);
    }

    public static List<ProductModel> GetMostBoughtProducts(int userId, int limit)
    {

        var productCounts = _connection.Query<(int ProductId, int Count)>(@"
            SELECT ProductID, COUNT(ProductID) AS Count
            FROM OrderItems
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
            FROM OrderItems
            JOIN Products ON OrderItems.ProductID = Products.ID
            WHERE DATE(OrderItems.Date) >= DATE(@StartDate)
            AND DATE(OrderItems.Date) <= DATE(@EndDate);
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
        const string sql = @"DELETE FROM OrderItems WHERE OrderId = @OrderId;";
        _connection.Execute(sql, new { OrderId = orderId });
    }
    public static int GetProductQuantityInOrder(int orderId, int productId)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM OrderItems
            WHERE OrderId = @OrderId AND ProductID = @ProductId;
        ";

        return _connection.ExecuteScalar<int>(sql, new { OrderId = orderId, ProductId = productId });
    }

    public static void RemoveProductFromOrder(int orderId, int productId)
    {
        const string sql = @"
            DELETE FROM OrderItems
            WHERE OrderId = @OrderId AND ProductID = @ProductId;
        ";

        _connection.Execute(sql, new { OrderId = orderId, ProductId = productId });
    }
    public static void RemoveProductQuantityFromOrder(int orderId, int productId, int quantity)
    {
        const string sql = @"
            DELETE FROM OrderItems
            WHERE ID IN (
                SELECT ID
                FROM OrderItems
                WHERE OrderId = @OrderId AND ProductID = @ProductId
                LIMIT @Quantity
            );
        ";

        _connection.Execute(sql, new { OrderId = orderId, ProductId = productId, Quantity = quantity });
    }

    // check is the user has already bought the item (christmas box)
    public static bool HasUserPurchasedProduct(int userId, int productId)
    {
        var sql = @"
            SELECT 1
            FROM OrderItems oi
            JOIN OrderHistory oh ON oi.OrderId = oh.Id
            WHERE oi.UserID = @UserId
              AND oi.ProductID = @ProductId
              AND oh.IsPaid = 1
            LIMIT 1;
        ";

        return _connection.ExecuteScalar<int?>(
            sql, 
            new {UserId = userId, ProductId = productId}
        ) !=null;
    }
}
