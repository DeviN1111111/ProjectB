using Dapper;
using Microsoft.Data.Sqlite;
public static class OrderHistoryAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();
    public static int AddToOrderHistory(int userId, bool isPaid=true)
    {
        var order = new OrderHistoryModel(userId, isPaid);
        var sql = "INSERT INTO OrderHistory (UserId, Date, IsPaid, FineDate) VALUES (@UserId, @Date, @IsPaid, @FineDate); SELECT last_insert_rowid();";
        int orderId = _connection.ExecuteScalar<int>(sql, order);  // Get the last inserted ID
        return orderId;
    }

    public static List<OrdersModel> GetOrdersByUserId(int userId)
    {
        var query = "SELECT * FROM OrderHistory WHERE UserId = @UserId ORDER BY Date DESC;";
        return _connection.Query<OrdersModel>(query, new { UserId = userId }).AsList();
    }
    public static List<OrderHistoryModel> GetAllUserOrders(int userId)
    {
        var query = "SELECT * FROM OrderHistory WHERE UserId = @UserId ORDER BY Date DESC;";
        return _connection.Query<OrderHistoryModel>(query, new { UserId = userId }).AsList();
    }
    public static OrderHistoryModel GetOrderById(int orderId)
    {
        var query = "SELECT * FROM OrderHistory WHERE Id = @OrderID;";
        return _connection.QueryFirstOrDefault<OrderHistoryModel>(query, new { OrderID = orderId })!;
    }
    public static OrderHistoryModel GetOrderByUserId(int userId)
    {
        var query = "SELECT * FROM OrderHistory WHERE UserId = @UserId ORDER BY Date Desc;";
        return _connection.QueryFirstOrDefault<OrderHistoryModel>(query, new { UserId = userId })!;
    }
    public static void UpdateIsPaidStatus(int orderId, DateTime? fineDate, bool isPaid, int? paymentCode)
    {

        var sql = @"
            UPDATE OrderHistory
            SET 
                IsPaid      = @IsPaid,
                FineDate    = CASE WHEN @IsPaid = 1 THEN NULL ELSE @FineDate END,
                PaymentCode = @PaymentCode
            WHERE Id = @OrderId;";

        _connection.Execute(sql, new { IsPaid = isPaid, FineDate = fineDate, OrderId = orderId, PaymentCode = paymentCode });
    }
    public static List<OrderHistoryModel> GetAllUnpaidOrders()
    {
        const string query = "SELECT * FROM OrderHistory WHERE IsPaid = 0 ORDER BY Date DESC;";
        return _connection.Query<OrderHistoryModel>(query).AsList();
    }
    public static void DeleteOrderHistory(int orderId)
    {
        const string sql = @"DELETE FROM OrderHistory WHERE Id = @OrderId;";
        _connection.Execute(sql, new { OrderId = orderId });
    }
}
