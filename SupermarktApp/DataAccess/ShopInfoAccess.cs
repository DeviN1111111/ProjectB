using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using Spectre.Console;
public class ShopInfoAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();
    public static string Table = "ShopInfo";
    public static ShopInfoModel? GetShopInfo()
    {
        var sql = $"SELECT * FROM {Table} LIMIT 1";
        return _connection.QueryFirstOrDefault<ShopInfoModel>(sql);
    }

    public static void UpdateShopInfo(ShopInfoModel shopInfo)
    {

        const string sql = @"
            UPDATE ShopInfo SET
                Description = @Description,
                OpeningHourMonday = @OpeningHourMonday,
                ClosingHourMonday = @ClosingHourMonday,
                OpeningHourTuesday = @OpeningHourTuesday,
                ClosingHourTuesday = @ClosingHourTuesday,
                OpeningHourWednesday = @OpeningHourWednesday,
                ClosingHourWednesday = @ClosingHourWednesday,
                OpeningHourThursday = @OpeningHourThursday,
                ClosingHourThursday = @ClosingHourThursday,
                OpeningHourFriday = @OpeningHourFriday,
                ClosingHourFriday = @ClosingHourFriday,
                OpeningHourSaturday = @OpeningHourSaturday,
                ClosingHourSaturday = @ClosingHourSaturday,
                OpeningHourSunday = @OpeningHourSunday,
                ClosingHourSunday = @ClosingHourSunday
            WHERE Id = @Id;";

        var affected = _connection.Execute(sql, shopInfo);

        if (affected == 0)
        {
            _connection.Execute(@"
                INSERT INTO ShopInfo (
                    Description,
                    OpeningHourMonday,  ClosingHourMonday,
                    OpeningHourTuesday, ClosingHourTuesday,
                    OpeningHourWednesday, ClosingHourWednesday,
                    OpeningHourThursday, ClosingHourThursday,
                    OpeningHourFriday,  ClosingHourFriday,
                    OpeningHourSaturday, ClosingHourSaturday,
                    OpeningHourSunday,  ClosingHourSunday
                )
                VALUES (
                    @Description,
                    @OpeningHourMonday,  @ClosingHourMonday,
                    @OpeningHourTuesday, @ClosingHourTuesday,
                    @OpeningHourWednesday, @ClosingHourWednesday,
                    @OpeningHourThursday, @ClosingHourThursday,
                    @OpeningHourFriday,  @ClosingHourFriday,
                    @OpeningHourSaturday, @ClosingHourSaturday,
                    @OpeningHourSunday,  @ClosingHourSunday
                );", shopInfo);
        }
    }
}