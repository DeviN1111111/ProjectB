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
    }

    public static List<DiscountsModel> GetWeeklyDiscounts()
    {
        var products = _sharedConnection.Query<DiscountsModel>(
        "SELECT * FROM Discounts WHERE DiscountType = @DiscountType",
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
    public static List<DiscountsModel> GetAllExpiryDiscounts()
    {
        var products = _sharedConnection.Query<DiscountsModel>(
        "SELECT * FROM Discounts WHERE DiscountType = @DiscountType",
        new { DiscountType = "Expiry" }).ToList();

        return products;
    }
    public static DiscountsModel GetPeronsalDiscountByProductAndUserID(int productID, int userID)
    {
        var products = _sharedConnection.QueryFirstOrDefault<DiscountsModel>(
        "SELECT * FROM Discounts WHERE UserId = @UserID AND ProductId = @ProductID",
        new { UserID = userID, ProductID = productID });
        return products;
    }
    public static DiscountsModel GetWeeklyDiscountByProductID(int productID)
    {
        return _sharedConnection.QueryFirstOrDefault<DiscountsModel>(
            "SELECT * FROM Discounts WHERE DiscountType = @DiscountType AND ProductId = @ProductID",
            new { DiscountType = "Weekly", ProductID = productID }
        );
    }

    public static DiscountsModel? GetDiscountsByProductID(int productID)
    {
        return _sharedConnection.QueryFirstOrDefault<DiscountsModel>(
            "SELECT * FROM Discounts WHERE ProductId = @ProductID",
            new { ProductID = productID });
    }
    
    public static void RemoveDiscountByProductID(int productID)
    {
        _sharedConnection.Execute(
            "DELETE FROM Discounts WHERE ProductId = @ProductID",
            new { ProductID = productID });

        _sharedConnection.Execute(@"
        UPDATE Products
        SET
        DiscountPercentage = 0,
        DiscountType = @None
        WHERE Id = @ProductID", new { ProductID = productID, None = "None" });
    }

    public static void RemoveAllPersonalDiscountsByUserID(int userID)
    {
        _sharedConnection.Execute( // delete all Personal discounts in the Discount table 
            "DELETE FROM Discounts WHERE UserId = @userID AND DiscountType = 'Personal'",
            new { userID }
        );
    }

    public static List<DiscountsModel> GetAllWeeklyDiscounts()
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.Query<DiscountsModel>(
            "SELECT * FROM Discounts WHERE DiscountType = @DiscountType",
            new { DiscountType = "Weekly" }).ToList();
    }

    public static void RemoveDiscountByID(int discountID)
    {
        _sharedConnection.Execute(
            "DELETE FROM Discounts WHERE ID = @DiscountID",
            new { DiscountID = discountID });
    }

    public static DiscountsModel GetDiscountByProductID(int productID)
    {
        return _sharedConnection.QueryFirstOrDefault<DiscountsModel>(
            "SELECT * FROM Discounts WHERE ProductId = @ProductID",
            new { ProductID = productID });
    }
    public static List<DiscountsModel> GetAllDiscountByProductID(int productID)
    {
        return _sharedConnection.Query<DiscountsModel>(
            "SELECT * FROM Discounts WHERE ProductId = @ProductID",
            new { ProductID = productID }).ToList();
    }
}