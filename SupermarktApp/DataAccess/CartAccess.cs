using Dapper;
using Microsoft.Data.Sqlite;
public class CartAccess
{
    private const string ConnectionString = "Data Source=database.db";
    public static string Table = "Cart";

    public static void AddToCart(int userId, int productId, int quantity, double discount = 0)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"INSERT INTO {Table} (UserId, ProductId, Quantity, Discount) VALUES (@UserId, @ProductId, @Quantity, @Discount)";
        db.Execute(sql, new { UserId = userId, ProductId = productId, Quantity = quantity, Discount = discount });
    }

    public static List<CartModel> GetAllUserProducts(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT * FROM {Table} WHERE UserId = @UserId ";
        return db.Query<CartModel>(sql, new { UserId = userId }).ToList();
    }

    public static void ClearCart()
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"DELETE FROM Cart WHERE UserId = @UserId";
        db.Execute(sql, new { UserId = SessionManager.CurrentUser.ID });
    }

    public static void RemoveFromCart(int userId, int productId)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"DELETE FROM {Table} WHERE UserId = @UserId AND ProductId = @ProductId";
        db.Execute(sql, new { UserId = userId, ProductId = productId });
    }
}