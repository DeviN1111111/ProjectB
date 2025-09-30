using Dapper;
using Microsoft.Data.Sqlite;

public static class FilterAccess
{
    private const string ConnectionString = "Data Source=database.db";

    public static void CreateTable()
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"
            CREATE TABLE IF NOT EXISTS FilterAccess (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Category TEXT,
                Name TEXT,
                Price DOUBLE,
                Kcal TEXT,
                Carbohydrates TEXT,
                Protein TEXT,
                Quantity INTEGER
            );
        ");
    }

    // public static void Register(UserModel user)
    // {
    //     using var db = new SqliteConnection(ConnectionString);
    //     db.Execute(@"INSERT INTO Users 
    //         (ID, Name, LastName, Email, Password, Adress, HouseNumber, Zipcode, PhoneNumber, City, IsAdmin)
    //         VALUES (@ID, @Name, @LastName, @Email, @Password, @Adress, @HouseNumber, @Zipcode, @PhoneNumber, @City, @IsAdmin)", user);
    // }

    // public static UserModel? Login(string Email, string Password)
    // {
    //     using var db = new SqliteConnection(ConnectionString);
    //     return db.QueryFirstOrDefault<UserModel>(
    //         @"SELECT * FROM Users 
    //           WHERE Email = @Email 
    //           AND 
    //           Password = @Password",
    //         new { Email, Password });
    // }

    // public static void ChangeAccountDetails(UserModel user)
    // {
    //     using var db = new SqliteConnection(ConnectionString);
    //     db.Execute(@"UPDATE Users 
    //         SET Name = @Name,
    //         LastName = @LastName,
    //         Email = @Email,
    //         Password = @Password
    //         Adress = @Adress,
    //         HouseNumber = @HouseNumber,
    //         Zipcode = @Zipcode,
    //         PhoneNumber = @PhoneNumber,
    //         City = @City
    //         WHERE ID = @ID", user);
    // }
}