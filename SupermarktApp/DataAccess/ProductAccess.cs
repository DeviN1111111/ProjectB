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

    public static void InsertProduct(ProductModel product)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO Products 
            (Name, Price, NutritionDetails, Description, Category, Quantity)
            VALUES (@Name, @Price, @NutritionDetails, @Description, @Category, @Quantity)", product);
    }

    public static void InsertProducts(IEnumerable<ProductModel> products)
    {
        using var db = new SqliteConnection(ConnectionString);
        foreach (var p in products)
        {
            db.Execute(@"INSERT INTO Products 
                (Name, Price, NutritionDetails, Description, Category, Quantity)
                VALUES (@Name, @Price, @NutritionDetails, @Description, @Category, @Quantity)", p);
        }
    }
    public static List<ProductModel> GetAllProducts()
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.Query<ProductModel>("SELECT * FROM Products").ToList();
    }
}
