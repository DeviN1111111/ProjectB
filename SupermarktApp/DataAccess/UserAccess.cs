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

    public static void Insert2FACode(int userId, string code, DateTime expiry)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"UPDATE Users 
            SET TwoFACode = @TwoFACode, TwoFAExpiry = @TwoFAExpiry 
            WHERE ID = @ID", new { TwoFACode = code, TwoFAExpiry = expiry, ID = userId });
    }

    public static string? Get2FACode(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QuerySingleOrDefault<string?>(@"SELECT TWOFACode
            FROM Users 
            WHERE ID = @ID", new { ID = userId });
    }

    public static DateTime? Get2FAExpiry(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QuerySingleOrDefault<DateTime?>(@"SELECT TWOFAExpiry 
            FROM Users 
            WHERE ID = @ID", new { ID = userId });
    }

    public static bool Has2FAEnabled(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QuerySingleOrDefault<bool>(@"SELECT TWOFAEnabled 
            FROM Users 
            WHERE ID = @ID", new { ID = userId });
    }

    public static string? GetUserEmail(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QuerySingleOrDefault<string>(@"SELECT Email 
            FROM Users 
            WHERE ID = @ID", new { ID = userId });
    }

    public static void Set2FAStatus(int userId, bool isEnabled)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"UPDATE Users 
            SET TWOFAEnabled = @TWOFAEnabled 
            WHERE ID = @ID", new { TWOFAEnabled = isEnabled, ID = userId });
    }

    public static UserModel? GetUserByID(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QuerySingleOrDefault<UserModel>(@"SELECT * 
            FROM Users 
            WHERE ID = @ID", new { ID = userId });
    }

    public static bool EmailExists(string email)
    {
        using var db = new SqliteConnection(ConnectionString);
        int count = db.QuerySingleOrDefault<int>(
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
        using var db = new SqliteConnection(ConnectionString);
        return db.Query<UserModel>(@"SELECT * 
            FROM Users 
            WHERE Birthdate = @Birthdate", new { Birthdate = dateOfBirth }).ToList();
    }
    public static void UpdateLastBirthdayGiftDate(int userId, DateTime lastBirthdayGiftDate)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"UPDATE Users 
            SET LastBirthdayGift = @LastBirthdayGift 
            WHERE ID = @ID", new { LastBirthdayGift = lastBirthdayGiftDate, ID = userId });
    }

    public static UserModel? GetUserByEmail(string email)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QuerySingleOrDefault<UserModel>(@"SELECT *
            FROM Users
            WHERE Email = @Email", new { Email = email });
    }
}
