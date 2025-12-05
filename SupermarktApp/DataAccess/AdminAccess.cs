using Dapper;
using Microsoft.Data.Sqlite;
public class AdminAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();

    public static void ChangeRole(int UserID, string AccountStatus)
    {
        _connection.Execute(@"UPDATE Users 
            SET AccountStatus = @AccountStatus
            WHERE ID = @UserID", new { UserID, AccountStatus });
    }

    public static List<UserModel> GetAllUsers()
    {
        return _connection.Query<UserModel>("SELECT * FROM USERS").ToList();
    }
    
    public static void DeleteUserByID(int UserID)
    {
        _connection.Execute(@"DELETE FROM Users
            WHERE
            ID = @UserID", new { UserID });
    }
}