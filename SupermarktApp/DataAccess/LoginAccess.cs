using Dapper;
using Microsoft.Data.Sqlite;

public static class LoginAccess
{
    private const string ConnectionString = "Data Source=database.db";

    public static void Register(UserModel user)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO Users 
            (Name, LastName, Email, Password, Address, Zipcode, PhoneNumber, City, TWOFAEnabled, AccountStatus)
            VALUES (@Name, @LastName, @Email, @Password, @Address, @Zipcode, @PhoneNumber, @City, @TwoFAEnabled, @AccountStatus)", user);
    }

    public static UserModel? Login(string Email, string Password)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QueryFirstOrDefault<UserModel>(
            @"SELECT * FROM Users 
              WHERE Email = @Email 
              AND 
              Password = @Password",
            new { Email, Password });
    }
    public static UserModel? GetUserByEmail(string email)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QueryFirstOrDefault<UserModel>(
            @"SELECT * FROM Users 
              WHERE Email = @Email",
            new { Email = email });
    }
    public static void UpdateUserPassword(int userId, string newPassword)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(
            @"UPDATE Users 
              SET Password = @NewPassword 
              WHERE ID = @UserId",
            new { NewPassword = newPassword, UserId = userId });
    }
}