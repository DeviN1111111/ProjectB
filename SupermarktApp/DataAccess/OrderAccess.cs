using Dapper;
using Microsoft.Data.Sqlite;
using System;
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
            @"SELECT o.ProductID, DATE(o.Date) AS Date, o.UserID, COUNT(*) AS SoldCount
            FROM Orders o
            WHERE DATE(Date) >= DATE(@StartDate) AND DATE(Date) <= DATE(@EndDate)
            GROUP BY o.ProductID, DATE(o.Date), o.UserID",
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
}
