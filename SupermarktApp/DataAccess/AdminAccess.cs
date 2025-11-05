using Dapper;
using Microsoft.Data.Sqlite;
public class AdminAccess
{
    private const string ConnectionString = "Data Source=database.db";

    public static void ChangeRole(int UserID, string AccountStatus)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"UPDATE Users 
            SET AccountStatus = @AccountStatus
            WHERE ID = @UserID", new { UserID, AccountStatus });
    }

    public static List<UserModel> GetAllUsers()
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.Query<UserModel>("SELECT * FROM USERS").ToList();
    }
    
    public static void DeleteUserByID(int UserID)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"DELETE FROM Users
            WHERE
            ID = @UserID", new { UserID });
    }
}