using Dapper;
using Microsoft.Data.Sqlite;

public static class LoginAccess
{
    private const string ConnectionString = "Data Source=database.db";

    public static void Register(UserModel user)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO Users 
            (Name, LastName, Email, Password, Address, Zipcode, PhoneNumber, City, AccountStatus)
            VALUES (@Name, @LastName, @Email, @Password, @Address, @Zipcode, @PhoneNumber, @City, @AccountStatus)", user);
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
}