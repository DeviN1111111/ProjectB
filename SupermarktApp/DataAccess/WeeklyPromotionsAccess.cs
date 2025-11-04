using Dapper;
using Microsoft.Data.Sqlite;

public static class WeeklyPromotionsAccess
{
    private const string ConnectionString = "Data Source=database.db";
    public static List<WeeklyPromotionsModel> GetAllProducts()
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.Query<WeeklyPromotionsModel>("SELECT * FROM WeeklyPromotions").ToList();
    }
}