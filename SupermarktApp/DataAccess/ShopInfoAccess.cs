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
    public static void UpdateOpeningHours(string openingHour, string closingHour)
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $@"
            INSERT INTO {Table} (Id, OpeningHour, ClosingHour)
            VALUES (1, @OpeningHour, @ClosingHour)
            ON CONFLICT(Id) DO UPDATE SET
                OpeningHour = excluded.OpeningHour,
                ClosingHour = excluded.ClosingHour;
        ";
        db.Execute(sql, new { OpeningHour = openingHour, ClosingHour = closingHour });
    }
    public static (string?, string?) GetOpeningHours()
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT OpeningHour, ClosingHour FROM {Table}";
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