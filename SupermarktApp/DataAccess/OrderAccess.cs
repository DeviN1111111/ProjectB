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

    public static IEnumerable<OrdersModel> GetOrdersAfterDate(DateTime date)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.Query<OrdersModel>(
            @"SELECT * FROM Orders
              WHERE DATE(Date) >= DATE(@Date);",
            new { Date = date }
        );
    }

    public static OrdersModel? GetMostSoldProductAfterDate(DateTime date)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QueryFirstOrDefault<OrdersModel>(
            @"SELECT ProductID, COUNT(*) AS SoldCount
              FROM Orders
              WHERE DATE(Date) >= DATE(@Date)
              GROUP BY ProductID
              ORDER BY SoldCount DESC
              LIMIT 1;",
            new { Date = date }
        );
    }

    public static int GetMostSoldCountUpToDate(DateTime date)
    {
        using var db = new SqliteConnection(ConnectionString);
        int count = db.ExecuteScalar<int>(
            @"SELECT COUNT(*) AS SoldCount
              FROM Orders
              WHERE ProductID = (
                  SELECT ProductID
                  FROM Orders
                  WHERE DATE(Date) >= DATE(@Date)
                  GROUP BY ProductID
                  ORDER BY COUNT(*) DESC
                  LIMIT 1
              );",
            new { Date = date }
        );
        return count;
    }

    public static List<ProductSalesDto> SeedProductSalesDto(DateTime fromDate)
    {
        using var db = new SqliteConnection(ConnectionString);

        var sales = db.Query<(int ProductID, DateTime Date, int UserID, int SoldCount)>(
            @"SELECT o.ProductID, DATE(o.Date) AS Date, o.UserID, COUNT(*) AS SoldCount
            FROM Orders o
            WHERE DATE(o.Date) >= DATE(@FromDate)
            GROUP BY o.ProductID, DATE(o.Date), o.UserID",
            new { FromDate = fromDate }
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

}
