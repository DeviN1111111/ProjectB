using Dapper;
using Microsoft.Data.Sqlite;

public static class UserAccess
{
    private const string ConnectionString = "Data Source=database.db";
    public static void UpdateUserPoints(int userId, int newPoints)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"UPDATE Users 
            SET AccountPoints = @AccountPoints
            WHERE ID = @ID", new { AccountPoints = newPoints, ID = userId });
    }

    public static int GetUserPoints(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QuerySingleOrDefault<int>(@"SELECT AccountPoints 
            FROM Users 
            WHERE ID = @ID", new { ID = userId });
    }
}
