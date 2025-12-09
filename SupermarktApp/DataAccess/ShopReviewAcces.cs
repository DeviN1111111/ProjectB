using System.Diagnostics;
using Dapper;
using Microsoft.Data.Sqlite;

public class ShopReviewAcces
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();
    public static string Table = "ShopReviews";

    public static void AddReview(int userId, int stars, string text, DateTime? createdAt)
    {
            var sql = $@"
                INSERT INTO {Table} (UserId, Stars, Text, CreatedAt)
                VALUES (@UserId, @Stars, @Text, @CreatedAt);
            ";
        _connection.Execute(sql, new { UserId = userId, Stars = stars, Text = text, CreatedAt = createdAt ?? DateTime.UtcNow });
    }

    public static List<ShopReviewModel> GetAllReviews(int userId)
    {
        var sql = $"SELECT * FROM {Table} WHERE UserId = @UserId ";
        return _connection.Query<ShopReviewModel>(sql, new { UserId = userId }).ToList();
    }

    public static List<ShopReviewModel> GetAllReviews()
    {
        var sql = $"SELECT * FROM {Table} ORDER BY CreatedAt DESC";
        return _connection.Query<ShopReviewModel>(sql).ToList();
    }

    public static void DeleteReviewByID(int reviewID)
    {
        var sql = $"DELETE FROM {Table} WHERE ID = @ID";
        _connection.Execute(sql, new { ID = reviewID });
    }
}