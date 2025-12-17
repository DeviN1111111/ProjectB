using Dapper;
using Microsoft.Data.Sqlite;
public class CartProductAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();
    public static string Table = "CartProduct";

    public static void AddToCartProduct(int userId, int productId, int quantity, double rewardPrice = 0)
    {
        var sql = $"INSERT INTO {Table} (UserId, ProductId, Quantity, RewardPrice) VALUES (@UserId, @ProductId, @Quantity, @RewardPrice)";
        _connection.Execute(sql, new { UserId = userId, ProductId = productId, Quantity = quantity, RewardPrice = rewardPrice });
    }

    public static List<CartProductModel> GetAllUserProducts(int userId)
    {
        var sql = $"SELECT * FROM {Table} WHERE UserId = @UserId ";
        return _connection.Query<CartProductModel>(sql, new { UserId = userId }).ToList();
    }

    public static CartProductModel? GetUserProductByProductId(int userId, int productId)
    {
        var sql = $"SELECT * FROM {Table} WHERE UserId = @UserId AND ProductId = @ProductId";
        return _connection.QueryFirstOrDefault<CartProductModel>(sql, new { UserId = userId, ProductId = productId });
    }

    public static void ClearCartProduct()
    {
        var sql = $"DELETE FROM CartProduct WHERE UserId = @UserId";
        _connection.Execute(sql, new { UserId = SessionManager.CurrentUser!.ID });
    }

    public static void RemoveFromCartProduct(int userId, int productId)
    {
        var sql = $"DELETE FROM {Table} WHERE UserId = @UserId AND ProductId = @ProductId";
        _connection.Execute(sql, new { UserId = userId, ProductId = productId });
    }
    
    public static void UpdateProductQuantity(int userId, int productId, int newQuantity)
    {
        var sql = $"UPDATE {Table} SET Quantity = @Quantity WHERE UserId = @UserId AND ProductId = @ProductId";
        _connection.Execute(sql, new { Quantity = newQuantity, UserId = userId, ProductId = productId });
    }
}