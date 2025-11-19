using Dapper;
using Microsoft.Data.Sqlite;
public class ChecklistAccess
{
    private const string ConnectionString = "Data Source=database.db";
    public static string Table = "Checklist";

    public static void AddToChecklist(int userId, int productId, int quantity)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"INSERT INTO {Table} (UserId, ProductId, Quantity) VALUES (@UserId, @ProductId, @Quantity)";
        db.Execute(sql, new { UserId = userId, ProductId = productId, Quantity = quantity });
    }

    public static List<ChecklistModel> GetAllUserProducts(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT * FROM {Table} WHERE Userid = @UserID ";
        return db.Query<ChecklistModel>(sql, new { UserId = userId }).ToList();
    }
    
    public static ChecklistModel? GetUserProductByProductId(int userId, int productId)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT * FROM {Table} WHERE UserId = @UserId AND ProductId = @ProductId";
        return db.QueryFirstOrDefault<ChecklistModel>(sql, new { UserId = userId, ProductId = productId });
    }
    public static void ClearChecklist()
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"DELETE FROM Cart WHERE UserId = @UserId";
        db.Execute(sql, new { UserId = SessionManager.CurrentUser.ID });
    }
    public static void RemoveFromChecklist(int userId, int productId)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"DELETE FROM {Table} WHERE UserId = @UserId AND ProductId = @ProductId";
        db.Execute(sql, new { UserId = userId, ProductId = productId });
    }
}