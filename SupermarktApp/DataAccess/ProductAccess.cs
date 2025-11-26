using Dapper;
using Microsoft.Data.Sqlite;

public static class ProductAccess
{
    private const string ConnectionString = "Data Source=database.db";

    public static void AddProduct(ProductModel product)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO Products 
            (Name, Price, NutritionDetails, Description, Category, Location, Quantity, Visible)
            VALUES (@Name, @Price, @NutritionDetails, @Description, @Category, @Location, @Quantity, @Visible)", product);
    }

    public static void AddProductUnitTest(ProductModel product)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO Products 
            (ID, Name, Price, NutritionDetails, Description, Category, Location, Quantity, Visible)
            VALUES (@ID, @Name, @Price, @NutritionDetails, @Description, @Category, @Location, @Quantity, @Visible)", product);
    }

    public static List<ProductModel> SearchProductByName(string name, bool includeHidden = false)
        {
            using var db = new SqliteConnection(ConnectionString);
            string pattern = $"{name}%";
            string sql = includeHidden
                    ? @"SELECT * FROM Products 
                        WHERE Name LIKE @Name
                        OR Category LIKE @Category
                        LIMIT 10"
                    : @"SELECT * FROM Products 
                        WHERE (Name LIKE @Name
                        OR Category LIKE @Category)
                        AND Visible = 1
                        LIMIT 10";
                return db.Query<ProductModel>(sql,new { Name = pattern, Category = pattern }).ToList();
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
            Quantity = @Quantity,
            Visible = @Visible
            WHERE 
            ID = @ID", newProduct);
    }

    public static void DeleteProductByID(int ID)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"DELETE FROM Products WHERE ID = @ID", new { ID });
    }

        public static void SetProductVisibility(int productId, bool isVisible)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute("UPDATE Products SET Visible = @Visible WHERE Id = @Id;",
            new { Visible = isVisible ? 1 : 0, Id = productId });
    }
}
