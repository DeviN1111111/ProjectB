using Dapper;
using Microsoft.Data.Sqlite;
public static class OrderHistoryAccess
{
    private const string ConnectionString = "Data Source=database.db";
    public static int AddToOrderHistory(int userId, bool isPaid=true)
    {
        using var db = new SqliteConnection(ConnectionString);
        var order = new OrderHistoryModel(userId, isPaid);
        var sql = "INSERT INTO OrderHistory (UserId, Date, IsPaid, FineDate) VALUES (@UserId, @Date, @IsPaid, @FineDate); SELECT last_insert_rowid();";
        int orderId = db.ExecuteScalar<int>(sql, order);  // Get the last inserted ID
        return orderId;
    }

    public static List<OrdersModel> GetOrdersByUserId(int userId)
    {
        using var connection = new SqliteConnection(ConnectionString);
        var query = "SELECT * FROM OrderHistory WHERE UserId = @UserId ORDER BY Date DESC;";
        return connection.Query<OrdersModel>(query, new { UserId = userId }).AsList();
    }
    public static List<OrderHistoryModel> GetAllUserOrders(int userId)
    {
        using var connection = new SqliteConnection(ConnectionString);
        var query = "SELECT * FROM OrderHistory WHERE UserId = @UserId ORDER BY Date DESC;";
        return connection.Query<OrderHistoryModel>(query, new { UserId = userId }).AsList();
    }
    public static OrderHistoryModel GetOrderById(int orderId)
    {
        using var connection = new SqliteConnection(ConnectionString);
        var query = "SELECT * FROM OrderHistory WHERE Id = @OrderID;";
        return connection.QueryFirstOrDefault<OrderHistoryModel>(query, new { OrderID = orderId })!;
    }
    public static OrderHistoryModel GetOrderByUserId(int userId)
    {
        using var connection = new SqliteConnection(ConnectionString);
        var query = "SELECT * FROM OrderHistory WHERE UserId = @UserId ORDER BY Date Desc;";
        return connection.QueryFirstOrDefault<OrderHistoryModel>(query, new { UserId = userId })!;
    }
    public static void UpdateIsPaidStatus(int orderId, DateTime? fineDate, bool isPaid, int? paymentCode)
    {
        using var db = new SqliteConnection(ConnectionString);

        var sql = @"
            UPDATE OrderHistory
            SET 
                IsPaid      = @IsPaid,
                FineDate    = CASE WHEN @IsPaid = 1 THEN NULL ELSE @FineDate END,
                PaymentCode = @PaymentCode
            WHERE Id = @OrderId;";

        db.Execute(sql, new { IsPaid = isPaid, FineDate = fineDate, OrderId = orderId, PaymentCode = paymentCode });
    }
    public static List<OrderHistoryModel> GetAllUnpaidOrders()
    {
        using var connection = new SqliteConnection(ConnectionString);
        const string query = "SELECT * FROM OrderHistory WHERE IsPaid = 0 ORDER BY Date DESC;";
        return connection.Query<OrderHistoryModel>(query).AsList();
    }
}
