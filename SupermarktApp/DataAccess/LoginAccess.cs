using Dapper;
using Microsoft.Data.Sqlite;

public static class LoginAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();

    public static void Register(UserModel user)
    {
        _connection.Execute(@"INSERT INTO Users 
            (Name, LastName, Email, Password, Address, Zipcode, PhoneNumber, Birthdate,City, TWOFAEnabled, AccountStatus)
            VALUES (@Name, @LastName, @Email, @Password, @Address, @Zipcode, @PhoneNumber, @Birthdate, @City, @TwoFAEnabled, @AccountStatus)", user);
    }

    public static UserModel? Login(string Email, string Password)
    {
        return _connection.QueryFirstOrDefault<UserModel>(
            @"SELECT * FROM Users 
              WHERE Email = @Email 
              AND 
              Password = @Password",
            new { Email, Password });
    }
    public static UserModel? GetUserByEmail(string email)
    {
        return _connection.QueryFirstOrDefault<UserModel>(
            @"SELECT * FROM Users 
              WHERE Email = @Email",
            new { Email = email });
    }
    public static void UpdateUserPassword(int userId, string newPassword)
    {
        _connection.Execute(
            @"UPDATE Users 
              SET Password = @NewPassword 
              WHERE ID = @UserId",
            new { NewPassword = newPassword, UserId = userId });
    }
}