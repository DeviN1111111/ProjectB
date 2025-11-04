using Dapper;
using Microsoft.Data.Sqlite;

public static class ProductAccess
{
    private const string ConnectionString = "Data Source=database.db";

    public static void AddProduct(ProductModel product)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO Products 
            (Name, Price, NutritionDetails, Description, Category, Location, Quantity)
            VALUES (@Name, @Price, @NutritionDetails, @Description, @Category, @Location, @Quantity)", product);
    }

    public static List<ProductModel> SearchProductByName(string name)
        {
            using var db = new SqliteConnection(ConnectionString);
            string pattern = $"{name}%";
            return db.Query<ProductModel>(
                @"SELECT * FROM Products 
                WHERE Name LIKE @Name
                OR
                Category LIKE @Category
                LIMIT 10",
                new { Name = pattern, Category = pattern }).ToList();
        }

    public static List<ProductModel> GetAllProducts()
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.Query<ProductModel>("SELECT * FROM Products").ToList();
    }

    public static ProductModel? GetProductByID(int id)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QueryFirstOrDefault<ProductModel>(
            "SELECT * FROM Products WHERE Id = @Id",
            new { Id = id }
        );
    }

    public static WeeklyPromotionsModel? GetProductByIDinWeeklyPromotions(int id)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QueryFirstOrDefault<WeeklyPromotionsModel>(
            "SELECT * FROM WeeklyPromotions WHERE Id = @Id",
            new { Id = id }
        );
    }

    public static ProductModel? GetProductByName(string name)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QueryFirstOrDefault<ProductModel>(
            @"SELECT * FROM Products 
            WHERE Name = @Name",
            new { Name = name }
        );
    }

    public static void UpdateProductStock(int productId, int newQuantity)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(
            "UPDATE Products SET Quantity = @Quantity WHERE Id = @Id",
            new { Quantity = newQuantity, Id = productId }
        );
    }
    public static void ChangeProductDetails(ProductModel newProduct)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"UPDATE Products
            SET
            Name = @Name,
            Price = @Price,
            NutritionDetails = @NutritionDetails,
            Description = @Description,
            Category = @Category,
            Location = @Location,
            Quantity = @Quantity
            WHERE 
            ID = @ID", newProduct);
    }

    public static void DeleteProductByID(int ID)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"DELETE FROM PRODUCTS
            WHERE
            ID = @ID", new { ID });
    }
}
