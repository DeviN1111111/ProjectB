using Dapper;
using Microsoft.Data.Sqlite;
public class CartAccess
{
    private const string ConnectionString = "Data Source=database.db";
    public static string Table = "Cart";

    public static void AddToCart(int userId, int productId, int quantity)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"INSERT INTO {Table} (UserId, ProductId, Quantity) VALUES (@UserId, @ProductId, @Quantity)";
        db.Execute(sql, new { UserId = userId, ProductId = productId, Quantity = quantity });
    }

    public static void RemoveFromCart(int userId, int productId)
    {
        using var db = new SqliteConnection(ConnectionString);
    }

    public static List<CartModel> GetAllUserProducts(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT * FROM {Table} WHERE UserId = @UserId ";
        return db.Query<CartModel>(sql,new {UserId = userId}).ToList();
    }


}