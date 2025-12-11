using System.Collections.Specialized;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using Spectre.Console;
public class ShopInfoAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();

    public static void AddDay(ShopInfoModel model)
    {
        _connection.Execute(@"INSERT INTO ShopInfo 
            (Day, OpeningHour, ClosingHour)
            VALUES (@Day, @OpeningHour, @ClosingHour)", model);
    }

    public static List<ShopInfoModel> GetDescriptionAndAllDays()
    {
        var sql = $"SELECT * FROM ShopInfo";
        return _connection.Query<ShopInfoModel>(sql).ToList();
    }

    public static void UpdateShopInfoDescription(string description)
    {
        const string sql = @"
            UPDATE ShopInfo
            SET Day = @day
            WHERE OpeningHour IS NULL
            AND ClosingHour IS NULL;
        ";

        _connection.Execute(sql, new { day = description });
    }

    public static void UpdateOpeningHours(string newOpeningHour, string newClosingHour, string day)
    {
        const string sql = @"
            UPDATE ShopInfo
            SET
            OpeningHour = @openingHour,
            ClosingHour = @closingHour
            WHERE Day = @Day;
        ";

        _connection.Execute(sql, new { Day = day, openingHour = newOpeningHour, closingHour = newClosingHour });
    }

}