using Dapper;
using Microsoft.Data.Sqlite;
static class CouponAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();

    public static void AddCoupon(int userId, double credit)
    {
        _connection.Execute(@"INSERT INTO Coupon (UserId, Credit, IsValid)
                    VALUES (@UserId, @Credit, 1)", new { UserId = userId, Credit = credit });
    }
    public static Coupon? GetCouponByUserId(int userId)
    {
        const string sql = "SELECT * FROM Coupon WHERE UserId = @UserId LIMIT 1";
        return _connection.QueryFirstOrDefault<Coupon>(sql, new { UserId = userId });
    }
    public static Coupon? GetCouponById(int id)
    {
        const string sql = "SELECT * FROM Coupon WHERE Id = @Id";
        return _connection.QueryFirstOrDefault<Coupon>(sql, new { Id = id });
    }
    public static void UseCoupon(int id)
    {
        _connection.Execute("UPDATE Coupon SET IsValid = 0 WHERE Id = @Id AND IsValid = 1", new { Id = id });
    }
    public static void EditCoupon(Coupon coupon)
    {
        _connection.Execute(@"UPDATE Coupon
                    SET UserId = @UserId,
                        Credit = @Credit,
                        IsValid = @IsValid
                    WHERE Id = @Id", coupon);
    }
    public static List<Coupon> GetAllCouponsByUserId(int userId)
    {
        const string sql = "SELECT * FROM Coupon WHERE UserId = @UserId";
        return _connection.Query<Coupon>(sql, new { UserId = userId }).ToList();
    }
}
