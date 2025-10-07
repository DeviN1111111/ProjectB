using Dapper;
using Microsoft.Data.Sqlite;
public class DatabaseFiller
{
    private const string ConnectionString = "Data Source=database.db";
    public static List<string> allTables = new List<string>() { "Cart", "Users", "Products", "Orders", "OrderHistory" };

    public static void RunDatabaseMethods()
    {
        DeleteTables();
        CreateTables();
        SeedData();
    }

    public static void DeleteTables()
    {

        using var db = new SqliteConnection(ConnectionString);
        db.Execute("PRAGMA foreign_keys = OFF;");
        foreach (string table in allTables)
        {
            db.Execute($"DROP TABLE IF EXISTS {table};");
        }
        db.Execute("PRAGMA foreign_keys = ON;");
    }

    public static void CreateTables()
    {
        Console.WriteLine("Creating tables...");
        using var db = new SqliteConnection(ConnectionString);
        db.Execute("PRAGMA foreign_keys = ON;");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                LastName TEXT NOT NULL,
                Email TEXT UNIQUE NOT NULL,
                Password TEXT NOT NULL,
                Address TEXT,
                HouseNumber INTEGER,
                Zipcode TEXT,
                PhoneNumber TEXT,
                City TEXT,
                IsAdmin INTEGER NOT NULL DEFAULT 0
            );
        ");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                NutritionDetails TEXT,
                Description TEXT,
                Category TEXT,
                Location TEXT,
                Quantity INTEGER NOT NULL DEFAULT 0
            );
        ");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS Orders (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserID INTEGER NOT NULL,
                ProductID INTEGER NOT NULL,
                Date DATETIME NOT NULL DEFAULT (datetime('now')),
                FOREIGN KEY (UserID) REFERENCES Users(ID) ON DELETE CASCADE,
                FOREIGN KEY (ProductID) REFERENCES Products(ID) ON DELETE CASCADE
            );
        ");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS OrderHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderId INTEGER NOT NULL,
                Date DATETIME NOT NULL DEFAULT (datetime('now')),
                TotalPrice REAL NOT NULL,
                FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE
            );
        ");
        db.Execute(@"
            CREATE TABLE IF NOT EXISTS Cart (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL
            );
        ");
    }
    
    public static void InsertUser(UserModel user)
    {
        using var db = new SqliteConnection(ConnectionString);

        string sql = @"INSERT INTO Users (Name, LastName, Email, Password, Address , Zipcode, PhoneNumber, City) 
        VALUES (@Name, @LastName, @Email, @Password, @Address , @Zipcode, @PhoneNumber, @City);";

        db.Execute(sql, user);
    }
    public static void InsertProduct(ProductModel product)
    {
        using var db = new SqliteConnection(ConnectionString);

        string sql = @"INSERT INTO Products (Name, Price, NutritionDetails, Description, Category, Location, Quantity)
        VALUES (@Name, @Price, @NutritionDetails, @Description, @Category, @Location, @Quantity);";

        db.Execute(sql, product);
    }
    
    public static void InsertOrder(OrdersModel order)
    {
        using var db = new SqliteConnection(ConnectionString);

        string sql = @"INSERT INTO Orders (UserID, ProductID, Date) 
        VALUES (@UserID, @ProductID, @Date);";

        db.Execute(sql, order);
    }

    public static void SeedData()
    {
        // users
        UserModel user = new UserModel { Name = "Mark", LastName = "Dekker", Email = "test@gmail.com", Password = "123456", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", City = "Rotterdam" };
        UserModel admin = new UserModel { Name = "Ben", LastName = "Dekker", Email = "admin@gmail.com", Password = "123456", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", City = "Rotterdam" };

        // Orders
        List<OrdersModel> orders = new List<OrdersModel>
        {
            // Orders for User 1
            new OrdersModel(1, 1),
            new OrdersModel(1, 2),
            new OrdersModel(1, 3),
            new OrdersModel(1, 4),
            new OrdersModel(1, 5),
            new OrdersModel(1, 6),
            new OrdersModel(1, 7),
            new OrdersModel(1, 8),
            new OrdersModel(1, 9),
            new OrdersModel(1, 10),
            new OrdersModel(1, 11),
            new OrdersModel(1, 12),
            new OrdersModel(1, 13),
            new OrdersModel(1, 14),
            new OrdersModel(1, 15),
            new OrdersModel(1, 16),
            new OrdersModel(1, 17),
            new OrdersModel(1, 18),
            new OrdersModel(1, 19),
            new OrdersModel(1, 20),
        };

        // products
    List <ProductModel> products = new List<ProductModel>
        {
            // Fruits
            new ProductModel { Name = "Apple", Price = 0.5, NutritionDetails = "52 kcal per 100g", Description = "Fresh red apple", Category = "Fruits", Location = "Rotterdam", Quantity = 50 },
            new ProductModel { Name = "Banana", Price = 0.3, NutritionDetails = "89 kcal per 100g", Description = "Ripe yellow banana", Category = "Fruits", Location = "Rotterdam", Quantity = 60 },
            new ProductModel { Name = "Orange", Price = 0.6, NutritionDetails = "47 kcal per 100g", Description = "Juicy orange", Category = "Fruits", Location = "Rotterdam", Quantity = 40 },
            new ProductModel { Name = "Strawberry", Price = 1.2, NutritionDetails = "33 kcal per 100g", Description = "Sweet fresh strawberries", Category = "Fruits", Location = "Rotterdam", Quantity = 35 },
            new ProductModel { Name = "Kiwi", Price = 0.8, NutritionDetails = "41 kcal per 100g", Description = "Green kiwi fruit", Category = "Fruits", Location = "Rotterdam", Quantity = 45 },

            // Vegetables
            new ProductModel { Name = "Carrot", Price = 0.2, NutritionDetails = "41 kcal per 100g", Description = "Organic orange carrots", Category = "Vegetables", Location = "Rotterdam", Quantity = 70 },
            new ProductModel { Name = "Broccoli", Price = 1.5, NutritionDetails = "55 kcal per 100g", Description = "Fresh broccoli florets", Category = "Vegetables", Location = "Rotterdam", Quantity = 30 },
            new ProductModel { Name = "Tomato", Price = 0.4, NutritionDetails = "18 kcal per 100g", Description = "Juicy red tomatoes", Category = "Vegetables", Location = "Rotterdam", Quantity = 80 },
            new ProductModel { Name = "Cucumber", Price = 0.7, NutritionDetails = "15 kcal per 100g", Description = "Crisp green cucumber", Category = "Vegetables", Location = "Rotterdam", Quantity = 55 },
            new ProductModel { Name = "Potato", Price = 0.3, NutritionDetails = "77 kcal per 100g", Description = "Dutch white potatoes", Category = "Vegetables", Location = "Rotterdam", Quantity = 100 },

            // Dairy
            new ProductModel { Name = "Milk", Price = 1.2, NutritionDetails = "42 kcal per 100ml", Description = "Full-cream milk", Category = "Dairy", Location = "Rotterdam", Quantity = 40 },
            new ProductModel { Name = "Cheese", Price = 2.5, NutritionDetails = "402 kcal per 100g", Description = "Aged cheddar cheese", Category = "Dairy", Location = "Rotterdam", Quantity = 25 },
            new ProductModel { Name = "Butter", Price = 1.8, NutritionDetails = "717 kcal per 100g", Description = "Unsalted Dutch butter", Category = "Dairy", Location = "Rotterdam", Quantity = 30 },

            // Bakery
            new ProductModel { Name = "Bread", Price = 1.0, NutritionDetails = "265 kcal per 100g", Description = "Whole wheat bread loaf", Category = "Bakery", Location = "Rotterdam", Quantity = 40 },
            new ProductModel { Name = "Muffin", Price = 2.0, NutritionDetails = "377 kcal per 100g", Description = "Blueberry muffin", Category = "Bakery", Location = "Rotterdam", Quantity = 20 },
            new ProductModel { Name = "Croissant", Price = 1.5, NutritionDetails = "406 kcal per 100g", Description = "Buttery croissant", Category = "Bakery", Location = "Rotterdam", Quantity = 35 },

            // Meat
            new ProductModel { Name = "Chicken Breast", Price = 5.0, NutritionDetails = "165 kcal per 100g", Description = "Boneless chicken breast", Category = "Meat", Location = "Rotterdam", Quantity = 25 },
            new ProductModel { Name = "Beef Steak", Price = 7.0, NutritionDetails = "250 kcal per 100g", Description = "Fresh beef steak", Category = "Meat", Location = "Rotterdam", Quantity = 15 },
            new ProductModel { Name = "Salmon Fillet", Price = 6.5, NutritionDetails = "208 kcal per 100g", Description = "Atlantic salmon fillet", Category = "Meat", Location = "Rotterdam", Quantity = 20 },

            // Beverages
            new ProductModel { Name = "Orange Juice", Price = 2.0, NutritionDetails = "45 kcal per 100ml", Description = "Freshly squeezed orange juice", Category = "Beverages", Location = "Rotterdam", Quantity = 30 }
        };
        // add users 
            InsertUser(user);
            InsertUser(admin);
        // add all products to the database
        foreach (var product in products)
        {
            InsertProduct(product);
        }
        foreach (var order in orders)
        {
            InsertOrder(order);
        }
    }
}