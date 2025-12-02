using System.Diagnostics;
using Dapper;
using Microsoft.Data.Sqlite;

public class ShopReviewAcces
{
    private const string ConnectionString = "Data Source=database.db";
    public static string Table = "ShopReviews";

    public static void AddReview(int userId, int stars, string text, DateTime? createdAt)
    {
        using var db = new SqliteConnection(ConnectionString);
            var sql = $@"
                INSERT INTO {Table} (UserId, Stars, Text, CreatedAt)
                VALUES (@UserId, @Stars, @Text, @CreatedAt);
            ";
        db.Execute(sql, new { UserId = userId, Stars = stars, Text = text, CreatedAt = createdAt ?? DateTime.UtcNow });
    }

    public static List<ShopReviewModel> GetAllReviews(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT * FROM {Table} WHERE UserId = @UserId ";
        return db.Query<ShopReviewModel>(sql, new { UserId = userId }).ToList();
    }

    public static List<ShopReviewModel> GetAllReviews()
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT * FROM {Table} ORDER BY CreatedAt DESC";
        return db.Query<ShopReviewModel>(sql).ToList();
    }

    public static void DeleteReviewByID(int reviewID)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"DELETE FROM {Table} WHERE ID = @ID";
        db.Execute(sql, new { ID = reviewID });
    }
}