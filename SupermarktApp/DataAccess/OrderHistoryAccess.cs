// using Dapper;
// using Microsoft.Data.Sqlite;
// public class OrderHistoryAccess
// {
//     private const string ConnectionString = "Data Source=database.db";

//     // All order history 
//     public static IEnumerable<OrderHistoryModel> GetAllOrders()
//     {
//         using var db = new SqliteConnection(ConnectionString);
//         return db.Query<OrderHistoryModel>("SELECT * FROM OrderHistory");
//     }

//     // Add to order history after checkout
//     public static void AddToOrderHistory(OrderHistoryModel order)
//     {
//         using var db = new SqliteConnection(ConnectionString);
//         db.Execute("INSERT INTO OrderHistory (UserId, ProductId, Quantity, OrderDate) VALUES (@UserId, @ProductId, @Quantity, @OrderDate)", order);
//     }

//     // Get all order history for a user
//     public static IEnumerable<OrderHistoryModel> GetOrderHistoryByUserId(int userId)
//     {
//         using var db = new SqliteConnection(ConnectionString);
//         return db.Query<OrderHistoryModel>("SELECT * FROM OrderHistory WHERE UserId = @UserId", new { UserId = userId });
//     }

// }