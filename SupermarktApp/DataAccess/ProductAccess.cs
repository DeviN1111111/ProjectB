using Dapper;
using Microsoft.Data.Sqlite;

public static class ProductAccess
{
    private const string ConnectionString = "Data Source=database.db";

    public static void CreateTable()
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"
            CREATE TABLE IF NOT EXISTS Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                Price REAL,
                NutritionDetails TEXT,
                Description TEXT,
                Category TEXT,
                Quantity INTEGER
            );
        ");
    }

    public static void AddProduct(ProductModel product)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO Products 
            (Name, Price, NutritionDetails, Description, Category, Quantity)
            VALUES (@Name, @Price, @NutritionDetails, @Description, @Category, @Quantity)", product);
    }

    public static List<ProductModel> SearchProductByName(string name)
    {
        using var db = new SqliteConnection(ConnectionString);
        string pattern = name.Length == 2 ? $"{name}%" : $"%{name}%";
        return db.Query<ProductModel>(
            @"SELECT * FROM Products 
            WHERE Name LIKE @Name 
            LIMIT 10",
            new { Name = $"{pattern}%" }).ToList();
    }

    public static List<ProductModel> SearchProductByCategory(string category)
    {
        using var db = new SqliteConnection(ConnectionString);
        string pattern = category.Length == 2 ? $"{category}%" : $"%{category}%";
        return db.Query<ProductModel>(
            @"SELECT * FROM Products 
            WHERE Category LIKE @Category 
            LIMIT 10",
            new { Category = $"{pattern}%" }).ToList();
    }
    public static IEnumerable<ProductModel> GetAllProducts()
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.Query<ProductModel>("SELECT * FROM Products");
    }

    public static ProductModel? GetProductByID(int id)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QueryFirstOrDefault<ProductModel>(
            "SELECT * FROM Products WHERE Id = @Id",
            new { Id = id }
        );
    }

    public static ProductModel GetProductByName(string name)
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.QueryFirstOrDefault<ProductModel>(
            @"SELECT * FROM Products 
            WHERE Name = @Name",
            new { Name = name }
        );
    }
}
