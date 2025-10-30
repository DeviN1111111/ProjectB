using Dapper;
using Microsoft.Data.Sqlite;
public static class OrderItemsAccess
{
    private const string ConnectionString = "Data Source=database.db";
    public static void AddToOrderItems(int userId, int productId, int quantity, Double price)
    {
        // Implementation for adding to order items
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(
            // if already in order items, update quantity, price
            @"INSERT INTO OrderItem (UserID, ProductID, Quantity, Price)
              VALUES (@UserID, @ProductID, @Quantity, @Price)
              ON CONFLICT(UserID, ProductID) DO UPDATE SET Quantity = @Quantity, Price = @Price;",
            new { UserID = userId, ProductID = productId, Quantity = quantity, Price = price }
        );
    }

    public static List<OrderItemModel> GetOrderItemsByUserId(int userId)
    {
        // Implementation for retrieving order items by user ID
        using var db = new SqliteConnection(ConnectionString);
        return db.Query<OrderItemModel>(@"SELECT * FROM OrderItem WHERE UserID = @UserID;", new { UserID = userId }).ToList();
    }
}