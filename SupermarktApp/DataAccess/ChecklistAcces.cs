using Dapper;
using Microsoft.Data.Sqlite;
public class ChecklistAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();
    public static string Table = "Checklist";

    public static void AddToChecklist(int userId, int productId, int quantity)
    {
        var sql = $"INSERT INTO {Table} (UserId, ProductId, Quantity) VALUES (@UserId, @ProductId, @Quantity)";
        _connection.Execute(sql, new { UserId = userId, ProductId = productId, Quantity = quantity });
    }

    public static List<ChecklistModel> GetAllUserProducts(int userID)
    {
        var sql = $"SELECT * FROM {Table} WHERE UserId = @UserID";
        return _connection.Query<ChecklistModel>(sql, new { UserID = userID }).ToList();
    }
    
    public static ChecklistModel? GetUserProductByProductId(int userId, int productId)
    {
        var sql = $"SELECT * FROM {Table} WHERE UserId = @UserId AND ProductId = @ProductId";
        return _connection.QueryFirstOrDefault<ChecklistModel>(sql, new { UserId = userId, ProductId = productId });
    }
    public static void ClearChecklist()
    {
        var sql = $"DELETE FROM CartProduct WHERE UserId = @UserId";
        _connection.Execute(sql, new { UserId = SessionManager.CurrentUser!.ID });
    }
    public static void RemoveFromChecklist(int userId, int productId)
    {
        var sql = $"DELETE FROM {Table} WHERE UserId = @UserId AND ProductId = @ProductId";
        _connection.Execute(sql, new { UserId = userId, ProductId = productId });
    }
}