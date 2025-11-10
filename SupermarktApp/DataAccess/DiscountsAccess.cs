using Dapper;
using Microsoft.Data.Sqlite;
public static class DiscountsAccess
{
    private const string ConnectionString = "Data Source=database.db";
    private static SqliteConnection? _sharedConnection = new SqliteConnection(ConnectionString);


    public static void AddDiscount(DiscountsModel Discount)
    {
        _sharedConnection.Execute(@"INSERT INTO Discounts 
            (ProductId, UserId, DiscountPercentage, DiscountType, StartDate, EndDate)
            VALUES (@ProductID, @UserID, @DiscountPercentage, @DiscountType, @StartDate, @EndDate)", Discount);

        _sharedConnection.Execute(@"
        UPDATE Products
        SET
        DiscountPercentage = @DiscountPercentage,
        DiscountType = @DiscountType
        WHERE Id = @ProductID", Discount);
    }

    public static List<ProductModel> GetWeeklyDiscounts()
    {
        var products = _sharedConnection.Query<ProductModel>(
        "SELECT *FROM Products WHERE DiscountType = @DiscountType",
        new { DiscountType = "Weekly" }).ToList();

        return products;
    }

    public static List<DiscountsModel> GetPersonalDiscounts(int userID)
    {
        var products = _sharedConnection.Query<DiscountsModel>(
        "SELECT * FROM Discounts WHERE UserId = @UserID",
        new { UserID = userID }).ToList();

        return products;
    }
    public static DiscountsModel? GetDiscountsByProductID(int productID)
    {
        return _sharedConnection.QueryFirstOrDefault<DiscountsModel>(
            "SELECT * FROM Discounts WHERE ProductId = @ProductID",
            new { ProductID = productID });
    }
}