using Dapper;
using Microsoft.Data.Sqlite;
static class CouponAccess
{
    private const string ConnectionString = "Data Source=database.db";
    public static void AddCoupon(int userId, double credit)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO Coupon (UserId, Credit, IsValid)
                    VALUES (@UserId, @Credit, 1)", new { UserId = userId, Credit = credit });
    }
    public static Coupon? GetCouponByUserId(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        const string sql = "SELECT * FROM Coupon WHERE UserId = @UserId LIMIT 1";
        return db.QueryFirstOrDefault<Coupon>(sql, new { UserId = userId });
    }
    public static Coupon? GetCouponById(int id)
    {
        using var db = new SqliteConnection(ConnectionString);
        const string sql = "SELECT * FROM Coupon WHERE Id = @Id";
        return db.QueryFirstOrDefault<Coupon>(sql, new { Id = id });
    }
    public static void UseCoupon(int id)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute("UPDATE Coupon SET IsValid = 0 WHERE Id = @Id AND IsValid = 1", new { Id = id });
    }
    public static void EditCoupon(Coupon coupon)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"UPDATE Coupon
                    SET UserId = @UserId,
                        Credit = @Credit,
                        IsValid = @IsValid
                    WHERE Id = @Id", coupon);
    }
    public static List<Coupon> GetAllCouponsByUserId(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);
        const string sql = "SELECT * FROM Coupon WHERE UserId = @UserId";
        return db.Query<Coupon>(sql, new { UserId = userId }).ToList();
    }
}
