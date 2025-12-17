using Dapper;
using Microsoft.Data.Sqlite;

public static class ProductAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();

    public static void AddProduct(ProductModel product)
    {
        _connection.Execute(@"INSERT INTO Products 
            (Name, Price, NutritionDetails, Description, Category, Location, Quantity, Visible, CompetitorPrice)
            VALUES (@Name, @Price, @NutritionDetails, @Description, @Category, @Location, @Quantity, @Visible, @CompetitorPrice)", product);
    }

    public static void AddProductUnitTest(ProductModel product)
    {
        _connection.Execute(@"INSERT INTO Products 
            (ID, Name, Price, NutritionDetails, Description, Category, Location, Quantity, Visible, CompetitorPrice)
            VALUES (@ID, @Name, @Price, @NutritionDetails, @Description, @Category, @Location, @Quantity, @Visible, @CompetitorPrice)", product);
    }

    public static List<ProductModel> SearchProductByName(string name, bool includeHidden = false)
        {
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
                return _connection.Query<ProductModel>(sql,new { Name = pattern, Category = pattern }).ToList();
        }

    public static List<ProductModel> GetAllProducts()
    {
        return GetAllProducts(includeHidden: false);
    }

    public static List<ProductModel> GetAllProducts(bool includeHidden = false)
    {
        string sql = includeHidden
            ? "SELECT * FROM Products"
            : "SELECT * FROM Products WHERE Visible = 1";

        return _connection.Query<ProductModel>(sql).ToList();
    }

    public static double GetCompetitorPriceByID(int id)
    {
        return _connection.ExecuteScalar<double>(
            "SELECT CompetitorPrice FROM Products WHERE Id = @Id",
            new { Id = id }
        );
    }
    
    public static List<ProductModel> GetChristmasBoxEligibleProducts(bool includeHidden = false)
    {   // create a query that selects the products admin selected eligible 
        string sql = includeHidden
            ? @"SELECT * FROM Products WHERE Category = 'ChristmasBoxItem'"
            : @"SELECT * FROM Products WHERE Category = 'ChristmasBoxItem' AND Visible = 1";

        return _connection.Query<ProductModel>(sql).ToList();
    }

    public static ProductModel? GetProductByID(int id)
    {
        return _connection.QueryFirstOrDefault<ProductModel>(
            "SELECT * FROM Products WHERE Id = @Id",
            new { Id = id }
        );
    }

    public static ProductModel? GetProductByName(string name)
    {
        return _connection.QueryFirstOrDefault<ProductModel>(
            @"SELECT * FROM Products 
            WHERE Name = @Name",
            new { Name = name }
        );
    }

    public static void UpdateProductStock(int productId, int newQuantity)
    {
        _connection.Execute(
            "UPDATE Products SET Quantity = @Quantity WHERE Id = @Id",
            new { Quantity = newQuantity, Id = productId }
        );
    }
    public static void ChangeProductDetails(ProductModel newProduct)
    {
        _connection.Execute(@"UPDATE Products
            SET
            Name = @Name,
            Price = @Price,
            NutritionDetails = @NutritionDetails,
            Description = @Description,
            Category = @Category,
            Location = @Location,
            Quantity = @Quantity,
            Visible = @Visible,
            CompetitorPrice = @CompetitorPrice
            WHERE 
            ID = @ID", newProduct);
    }

    public static void DeleteProductByID(int ID)
    {
        _connection.Execute(@"UPDATE Products SET Visible = 0 WHERE ID = @ID", new { ID });
    }

    public static void SetProductVisibility(int productId, bool isVisible)
    {
        _connection.Execute("UPDATE Products SET Visible = @Visible WHERE Id = @Id;",
            new { Visible = isVisible ? 1 : 0, Id = productId });
    }
    // get product quantity by id
    public static int GetProductQuantityByID(int id)
    {
        return _connection.ExecuteScalar<int>(
            "SELECT Quantity FROM Products WHERE Id = @Id",
            new { Id = id }
        );
    }


}
