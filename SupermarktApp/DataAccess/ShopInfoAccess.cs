using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;
public class ShopInfoAccess
{
    private const string ConnectionString = "Data Source=database.db";
    public static string Table = "ShopInfo";

    public static void UpdateDescription(string description)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $@"
            INSERT INTO {Table} (Id, Description)
            VALUES (1, @Description)
            ON CONFLICT(Id) DO UPDATE SET
                Description = excluded.Description;
        ";
        db.Execute(sql, new { Description = description });
    }
    public static string? GetDescription()
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT Description FROM {Table}";
        return db.QueryFirstOrDefault<string?>(sql);
    }
    public static void UpdateOpeningHoursMonday(string openingHour, string closingHour)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $@"
            INSERT INTO {Table} (Id, OpeningHourMonday, ClosingHourMonday)
            VALUES (1, @OpeningHourMonday, @ClosingHourMonday)
            ON CONFLICT(Id) DO UPDATE SET
                OpeningHourMonday = excluded.OpeningHourMonday,
                ClosingHourMonday = excluded.ClosingHourMonday;
        ";
        db.Execute(sql, new { OpeningHourMonday = openingHour, ClosingHourMonday = closingHour });
    }
    public static (string?, string?) GetOpeningHoursMonday()
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT OpeningHourMonday, ClosingHourMonday FROM {Table}";
        return db.QueryFirstOrDefault<(string?, string?)>(sql);
    }
    public static void UpdateOpeningHoursTuesday(string openingHour, string closingHour)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $@"
            INSERT INTO {Table} (Id, OpeningHourTuesday, ClosingHourTuesday)
            VALUES (1, @OpeningHourTuesday, @ClosingHourTuesday)
            ON CONFLICT(Id) DO UPDATE SET
                OpeningHourTuesday = excluded.OpeningHourTuesday,
                ClosingHourTuesday = excluded.ClosingHourTuesday;
        ";
        db.Execute(sql, new { OpeningHourTuesday = openingHour, ClosingHourTuesday = closingHour });
    }
    public static (string?, string?) GetOpeningHoursTuesday()
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT OpeningHourTuesday, ClosingHourTuesday FROM {Table}";
        return db.QueryFirstOrDefault<(string?, string?)>(sql);
    }
    public static void UpdateOpeningHoursWednesday(string openingHour, string closingHour)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $@"
            INSERT INTO {Table} (Id, OpeningHourWednesday, ClosingHourWednesday)
            VALUES (1, @OpeningHourWednesday, @ClosingHourWednesday)
            ON CONFLICT(Id) DO UPDATE SET
                OpeningHourWednesday = excluded.OpeningHourWednesday,
                ClosingHourWednesday = excluded.ClosingHourWednesday;
        ";
        db.Execute(sql, new { OpeningHourWednesday = openingHour, ClosingHourWednesday = closingHour });
    }
    public static (string?, string?) GetOpeningHoursWednesday()
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT OpeningHourWednesday, ClosingHourWednesday FROM {Table}";
        return db.QueryFirstOrDefault<(string?, string?)>(sql);
    }
    public static void UpdateOpeningHoursThursday(string openingHour, string closingHour)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $@"
            INSERT INTO {Table} (Id, OpeningHourThursday, ClosingHourThursday)
            VALUES (1, @OpeningHourThursday, @ClosingHourThursday)
            ON CONFLICT(Id) DO UPDATE SET
                OpeningHourThursday = excluded.OpeningHourThursday,
                ClosingHourThursday = excluded.ClosingHourThursday;
        ";
        db.Execute(sql, new { OpeningHourThursday = openingHour, ClosingHourThursday = closingHour });
    }
    public static (string?, string?) GetOpeningHoursThursday()
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT OpeningHourThursday, ClosingHourThursday FROM {Table}";
        return db.QueryFirstOrDefault<(string?, string?)>(sql);
    }
    public static void UpdateOpeningHoursFriday(string openingHour, string closingHour)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $@"
            INSERT INTO {Table} (Id, OpeningHourFriday, ClosingHourFriday)
            VALUES (1, @OpeningHourFriday, @ClosingHourFriday)
            ON CONFLICT(Id) DO UPDATE SET
                OpeningHourFriday = excluded.OpeningHourFriday,
                ClosingHourFriday = excluded.ClosingHourFriday;
        ";
        db.Execute(sql, new { OpeningHourFriday = openingHour, ClosingHourFriday = closingHour });
    }
    public static (string?, string?) GetOpeningHoursFriday()
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT OpeningHourFriday, ClosingHourFriday FROM {Table}";
        return db.QueryFirstOrDefault<(string?, string?)>(sql);
    }
    public static void UpdateOpeningHoursSaturday(string openingHour, string closingHour)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $@"
            INSERT INTO {Table} (Id, OpeningHourSaturday, ClosingHourSaturday)
            VALUES (1, @OpeningHourSaturday, @ClosingHourSaturday)
            ON CONFLICT(Id) DO UPDATE SET
                OpeningHourSaturday = excluded.OpeningHourSaturday,
                ClosingHourSaturday = excluded.ClosingHourSaturday;
        ";
        db.Execute(sql, new { OpeningHourSaturday = openingHour, ClosingHourSaturday = closingHour });
    }
    public static (string?, string?) GetOpeningHoursSaturday()
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT OpeningHourSaturday, ClosingHourSaturday FROM {Table}";
        return db.QueryFirstOrDefault<(string?, string?)>(sql);
    }
    public static void UpdateOpeningHoursSunday(string openingHour, string closingHour)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $@"
            INSERT INTO {Table} (Id, OpeningHourSunday, ClosingHourSunday)
            VALUES (1, @OpeningHourSunday, @ClosingHourSunday)
            ON CONFLICT(Id) DO UPDATE SET
                OpeningHourSunday = excluded.OpeningHourSunday,
                ClosingHourSunday = excluded.ClosingHourSunday;
        ";
        db.Execute(sql, new { OpeningHourSunday = openingHour, ClosingHourSunday = closingHour });
    }
    public static (string?, string?) GetOpeningHoursSunday()
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT OpeningHourSunday, ClosingHourSunday FROM {Table}";
        return db.QueryFirstOrDefault<(string?, string?)>(sql);
    }
}