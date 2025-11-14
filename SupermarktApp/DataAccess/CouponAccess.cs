using Dapper;
using Microsoft.Data.Sqlite;
static class CouponAccess
{
    private const string ConnectionString = "Data Source=database.db";
    
    public static void AddCoupon(int userId, double credit)
    {
        var random = new Random();
        var code = random.Next(100000, 999999);
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO Coupon (UserId, Credit, IsValid, Code)
                    VALUES (@UserId, @Credit, 0, @Code)", new { UserId = userId, Credit = credit, Code = code });
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
    public static Coupon? GetCouponByCode(int code)
    {
        using var db = new SqliteConnection(ConnectionString);
        const string sql = "SELECT * FROM Coupon WHERE Code = @Code LIMIT 1";
        return db.QueryFirstOrDefault<Coupon>(sql, new { Code = code });
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
