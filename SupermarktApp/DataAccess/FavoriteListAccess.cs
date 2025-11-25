using Dapper;
using Microsoft.Data.Sqlite;
static class FavoriteListAccess
{
    private const string ConnectionString = "Data Source=database.db";

    public static List<FavoriteListModel> GetAllFavoriteListsByUserId(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        const string sql = "SELECT * FROM FavoriteList WHERE UserId = @UserId";
        return db.Query<FavoriteListModel>(sql, new { UserId = userId }).ToList();
    }
}   