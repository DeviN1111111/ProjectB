using Dapper;
using Microsoft.Data.Sqlite;

public static class UserAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();
    public static void UpdateUserPoints(int userId, int newPoints)
    {
        _connection.Execute(@"UPDATE Users 
            SET AccountPoints = @AccountPoints
            WHERE ID = @ID", new { AccountPoints = newPoints, ID = userId });
    }

    public static int GetUserPoints(int userId)
    {
        return _connection.QuerySingleOrDefault<int>(@"SELECT AccountPoints 
            FROM Users 
            WHERE ID = @ID", new { ID = userId });
    }

    public static void Insert2FACode(int userId, string code, DateTime expiry)
    {
        _connection.Execute(@"UPDATE Users 
            SET TwoFACode = @TwoFACode, TwoFAExpiry = @TwoFAExpiry 
            WHERE ID = @ID", new { TwoFACode = code, TwoFAExpiry = expiry, ID = userId });
    }

    public static string? Get2FACode(int userId)
    {
        return _connection.QuerySingleOrDefault<string?>(@"SELECT TWOFACode
            FROM Users 
            WHERE ID = @ID", new { ID = userId });
    }

    public static DateTime? Get2FAExpiry(int userId)
    {
        return _connection.QuerySingleOrDefault<DateTime?>(@"SELECT TWOFAExpiry 
            FROM Users 
            WHERE ID = @ID", new { ID = userId });
    }

    public static bool Has2FAEnabled(int userId)
    {
        return _connection.QuerySingleOrDefault<bool>(@"SELECT TWOFAEnabled 
            FROM Users 
            WHERE ID = @ID", new { ID = userId });
    }

    public static string? GetUserEmail(int userId)
    {
        return _connection.QuerySingleOrDefault<string>(@"SELECT Email 
            FROM Users 
            WHERE ID = @ID", new { ID = userId });
    }

    public static void Set2FAStatus(int userId, bool isEnabled)
    {
        _connection.Execute(@"UPDATE Users 
            SET TWOFAEnabled = @TWOFAEnabled 
            WHERE ID = @ID", new { TWOFAEnabled = isEnabled, ID = userId });
    }

    public static UserModel? GetUserByID(int userId)
    {
        return _connection.QuerySingleOrDefault<UserModel>(@"SELECT * 
            FROM Users 
            WHERE ID = @ID", new { ID = userId });
    }

    public static bool EmailExists(string email)
    {
        int count = _connection.QuerySingleOrDefault<int>(
            @"SELECT COUNT(1) 
                FROM Users 
                WHERE LOWER(Email) = LOWER(@Email)",
            new { Email = email.Trim() }
        );
        // if there is a match found
        return count > 0;
    }
    public static List<UserModel>? GetUsersByDateOfBirth(DateTime dateOfBirth)
    {
        return _connection.Query<UserModel>(@"SELECT * 
            FROM Users 
            WHERE Birthdate = @Birthdate", new { Birthdate = dateOfBirth }).ToList();
    }
    public static void UpdateLastBirthdayGiftDate(int userId, DateTime lastBirthdayGiftDate)
    {
        _connection.Execute(@"UPDATE Users 
            SET LastBirthdayGift = @LastBirthdayGift 
            WHERE ID = @ID", new { LastBirthdayGift = lastBirthdayGiftDate, ID = userId });
    }

    public static UserModel? GetUserByEmail(string email)
    {
        return _connection.QuerySingleOrDefault<UserModel>(@"SELECT *
            FROM Users
            WHERE Email = @Email", new { Email = email });
    }

    public static void ChangeProfileSettings(int userID, string newName, string newLastName, string newEmail, string newAddress, string newZipcode, string newPhoneNumber, string newCity, DateTime newBirthdate)
    {
        _connection.Execute(@"UPDATE Users
            SET
            Name = @NewName,
            LastName = @NewLastName,
            Email = @NewEmail,
            Address = @NewAddress,
            Zipcode = @NewZipcode,
            PhoneNumber = @NewPhoneNumber,
            City = @NewCity,
            Birthdate = @NewBirthdate
            WHERE 
            ID = @UserID", new { NewName = newName, NewLastName = newLastName, NewEmail = newEmail, NewAddress = newAddress, NewZipcode = newZipcode, NewPhoneNumber = newPhoneNumber, NewCity = newCity, NewBirthdate = newBirthdate ,UserID = userID });
    }
}
