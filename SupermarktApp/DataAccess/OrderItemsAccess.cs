using Dapper;
using Microsoft.Data.Sqlite;
public static class OrderItemsAccess
{
    private const string ConnectionString = "Data Source=database.db";
    public static void AddToOrderItems(int orderId, int productId, int quantity, double price)
    {
        using var db = new SqliteConnection(ConnectionString);

        db.Execute(
            // Insert a new order item or update it if it already exists
            @"INSERT INTO OrderItem (OrderId, ProductId, Quantity, Price)
            VALUES (@OrderId, @ProductId, @Quantity, @Price)
            ON CONFLICT(OrderId, ProductId) 
            DO UPDATE SET Quantity = @Quantity, Price = @Price;",
            new { OrderId = orderId, ProductId = productId, Quantity = quantity, Price = price }
        );
    }

    public static List<OrderItemModel> GetOrderItemsByOrderId(int orderId)
    {
        using var connection = new SqliteConnection(ConnectionString);
        var query = @"
            SELECT Id AS OrderItemId, OrderId, ProductId, Quantity, Price
            FROM OrderItem
            WHERE OrderId = @OrderId;
        ";
        return connection.Query<OrderItemModel>(query, new { OrderId = orderId }).AsList();
    }

}