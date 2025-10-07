using System;
using Dapper;
using Microsoft.Data.Sqlite;

public static class SupermarketDatabase
{
    private const string ConnectionString = "Data Source=database.db";

    public static void CreateTables()
    {
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
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
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
    }

    public static void SeedDatabase()
    {
        using var db = new SqliteConnection(ConnectionString);

        db.Execute(@"
            INSERT INTO Users (Name, LastName, Email, Password, City, IsAdmin)
            VALUES 
            ('Alice', 'Smith', 'alice@example.com', 'password123', 'New York', 0),
            ('Bob', 'Johnson', 'bob@example.com', 'securepass', 'Los Angeles', 1)
            ON CONFLICT(Email) DO NOTHING;
        ");

        db.Execute(@"
            INSERT INTO Products (Name, Price, Description, Category, Quantity)
            VALUES
            ('Apple', 0.99, 'Fresh red apple', 'Fruit', 100),
            ('Banana', 0.59, 'Organic yellow bananas', 'Fruit', 120),
            ('Orange', 1.20, 'Juicy orange', 'Fruit', 90),
            ('Milk', 2.50, '1L whole milk', 'Dairy', 50),
            ('Cheese', 4.20, 'Cheddar cheese block', 'Dairy', 40),
            ('Yogurt', 1.50, 'Greek yogurt cup', 'Dairy', 70),
            ('Bread', 1.10, 'Whole grain bread', 'Bakery', 30),
            ('Croissant', 2.00, 'Buttery croissant', 'Bakery', 25),
            ('Chicken Breast', 6.99, 'Boneless chicken breast', 'Meat', 60),
            ('Beef Steak', 12.50, 'Sirloin beef steak', 'Meat', 35),
            ('Salmon', 9.99, 'Fresh salmon fillet', 'Seafood', 40),
            ('Shrimp', 8.49, 'Frozen shrimp pack', 'Seafood', 55),
            ('Rice', 3.20, '5lb bag of rice', 'Pantry', 80),
            ('Pasta', 1.75, 'Spaghetti pasta', 'Pantry', 95),
            ('Olive Oil', 7.99, 'Extra virgin olive oil', 'Pantry', 45),
            ('Tomato Sauce', 2.30, 'Canned tomato sauce', 'Pantry', 100),
            ('Eggs', 3.10, '12-pack eggs', 'Dairy', 110),
            ('Butter', 2.80, 'Salted butter stick', 'Dairy', 60),
            ('Coffee', 5.50, 'Ground coffee pack', 'Beverages', 50),
            ('Tea', 4.00, 'Green tea box', 'Beverages', 75)
            ON CONFLICT(Name) DO NOTHING;
        ");

        var random = new Random();
        for (int i = 1; i <= 100; i++)
        {
            var product = new
            {
                Name = $"Product{i}",
                Price = Math.Round(random.NextDouble() * 100, 2),
                Description = $"Auto-generated product {i}",
                Category = "General",
                Quantity = random.Next(1, 200)
            };

            db.Execute(@"
                INSERT INTO Products (Name, Price, Description, Category, Quantity)
                VALUES (@Name, @Price, @Description, @Category, @Quantity)
                ON CONFLICT(Name) DO NOTHING;", product);
        }

        db.Execute(@"
            INSERT INTO Orders (UserId, ProductId)
            VALUES (
                (SELECT Id FROM Users WHERE Email='alice@example.com'),
                (SELECT Id FROM Products WHERE Name='Apple')
            );
        ");

        db.Execute(@"
            INSERT INTO OrderHistory (OrderId, TotalPrice)
            VALUES (
                (SELECT Id FROM Orders LIMIT 1),
                0.99
            );
        ");
    }
    public static void SeedTestOrders(int numberOfOrders = 500)
    {
        using var db = new SqliteConnection(ConnectionString);
        var random = new Random();

        // Get all user IDs
        var userIds = db.Query<int>("SELECT Id FROM Users").AsList();

        // Get all product IDs and their prices
        var products = db.Query<(int Id, double Price)>("SELECT Id, Price FROM Products").AsList();

        for (int i = 0; i < numberOfOrders; i++)
        {
            // Random user and product
            var userId = userIds[random.Next(userIds.Count)];
            var product = products[random.Next(products.Count)];

            // Insert order
            db.Execute(@"
                INSERT INTO Orders (UserId, ProductId, Date)
                VALUES (@UserId, @ProductId, @Date);",
                new { UserId = userId, ProductId = product.Id, Date = DateTime.Now.AddDays(-random.Next(0, 60)) } // random past 60 days
            );

            // Get last inserted order ID
            var orderId = db.QuerySingle<int>("SELECT last_insert_rowid();");

            // Insert into OrderHistory
            db.Execute(@"
                INSERT INTO OrderHistory (OrderId, Date, TotalPrice)
                VALUES (@OrderId, @Date, @TotalPrice);",
                new { OrderId = orderId, Date = DateTime.Now.AddDays(-random.Next(0, 60)), TotalPrice = product.Price }
            );
        }
    }

}
