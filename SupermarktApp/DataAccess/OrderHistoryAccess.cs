using Dapper;
using Microsoft.Data.Sqlite;
public static class OrderHistoryAccess
{
    private const string ConnectionString = "Data Source=database.db";
    public static int AddToOrderHistory(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        var order = new OrderHistoryModel(userId);
        var sql = "INSERT INTO OrderHistory (UserId, Date) VALUES (@UserId, @Date); SELECT last_insert_rowid();";
        int orderId = db.ExecuteScalar<int>(sql, order);  // Get the last inserted ID
        return orderId;
    }

        public static List<OrdersModel> GetOrdersByUserId(int userId)
    {
        using var connection = new SqliteConnection(ConnectionString);
        var query = "SELECT * FROM OrderHistory WHERE UserId = @UserId ORDER BY Date DESC;";
        return connection.Query<OrdersModel>(query, new { UserId = userId }).AsList();
    }
}