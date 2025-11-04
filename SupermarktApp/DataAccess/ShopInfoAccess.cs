using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using Spectre.Console;
public class ShopInfoAccess
{
    private const string ConnectionString = "Data Source=database.db";
    public static string Table = "ShopInfo";
    public static ShopInfoModel? GetShopInfo()
    {
        using var db = new SqliteConnection(ConnectionString);
        var sql = $"SELECT * FROM {Table} LIMIT 1";
        return db.QueryFirstOrDefault<ShopInfoModel>(sql);
    }

    public static void UpdateShopInfo(ShopInfoModel shopInfo)
    {
        using var db = new SqliteConnection(ConnectionString);

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

        var affected = db.Execute(sql, shopInfo);

        if (affected == 0)
        {
            db.Execute(@"
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