using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;
using System;
using System.Collections.Generic;

public class DatabaseFiller
{
    private const string ConnectionString = "Data Source=database.db";
    public static List<string> allTables = new List<string>() { "Cart", "Users", "Products", "Orders", "OrderHistory", "RewardItems" };

    public static void RunDatabaseMethods(int orderCount = 50)
    {
        DeleteTables();
        CreateTables();
        SeedData(orderCount);
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
                Zipcode TEXT,
                PhoneNumber TEXT,
                City TEXT,
                AccountStatus TEXT,
                AccountPoints INTEGER DEFAULT 0
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
                Location INTEGER,
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
                Quantity INTEGER NOT NULL,
                Discount REAL NOT NULL DEFAULT 0,
                RewardPrice REAL NOT NULL DEFAULT 0
            );
        ");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS Checklist (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL
            );
        ");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS RewardItems (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                ProductId INTEGER NOT NULL,
                PriceInPoints INTEGER NOT NULL
            );
        ");
    }
    
    public static void InsertUser(UserModel user)
    {
        using var db = new SqliteConnection(ConnectionString);
        string sql = @"INSERT INTO Users (Name, LastName, Email, Password, Address , Zipcode, PhoneNumber, City, AccountStatus) 
        VALUES (@Name, @LastName, @Email, @Password, @Address , @Zipcode, @PhoneNumber, @City, @AccountStatus);";
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

    public static void SeedData(int orderCount)
    {
        UserModel user = new UserModel { Name = "Mark", LastName = "Dekker", Email = "u", Password = "u", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", City = "Rotterdam" };
        UserModel user1 = new UserModel { Name = "Mark", LastName = "Dekker", Email = "testing1@gmail.com", Password = "123456", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", City = "Rotterdam" };
        UserModel user2 = new UserModel { Name = "Mark", LastName = "Dekker", Email = "testing2@gmail.com", Password = "123456", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", City = "Rotterdam" };
        UserModel admin = new UserModel { Name = "Ben", LastName = "Dekker", Email = "a", Password = "a", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", City = "Rotterdam", AccountStatus = "Admin" };
        UserModel SuperAdmin = new UserModel { Name = "Ben", LastName = "Dekker", Email = "sa", Password = "sa", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", City = "Rotterdam", AccountStatus = "SuperAdmin" };

        RewardItemsAccess.AddRewardItem(new RewardItemsModel(271, 50));
        RewardItemsAccess.AddRewardItem(new RewardItemsModel(272, 60));
        RewardItemsAccess.AddRewardItem(new RewardItemsModel(273, 30));


        var categoryProducts = new Dictionary<string, List<string>>
        {
            ["Fruits"] = new List<string> { "Apple", "Banana", "Orange", "Pear", "Grapes", "Pineapple", "Strawberry", "Watermelon", "Kiwi", "Mango", "Peach", "Plum", "Blueberry", "Raspberry", "Blackberry", "Cherry", "Cantaloupe", "Papaya", "Lemon", "Lime", "Nectarine", "Apricot", "Fig", "Pomegranate", "Tangerine", "Clementine", "Dragonfruit", "Passionfruit", "Guava", "Lychee" },
            ["Vegetables"] = new List<string> { "Carrot", "Broccoli", "Lettuce", "Spinach", "Tomato", "Cucumber", "Onion", "Pepper", "Zucchini", "Eggplant", "Cauliflower", "Celery", "Mushroom", "Garlic", "Potato", "Sweet Potato", "Pumpkin", "Beetroot", "Radish", "Kale", "Leek", "Brussels Sprouts", "Chard", "Artichoke", "Parsnip", "Turnip", "Okra", "Fennel", "Cabbage", "Rutabaga" },
            ["Beverages"] = new List<string> { "Orange Juice", "Cola", "Water", "Milkshake", "Coffee", "Tea", "Beer", "Wine", "Apple Juice", "Lemonade", "Smoothie", "Iced Tea", "Hot Chocolate", "Energy Drink", "Sparkling Water", "Ginger Ale", "Soda", "Cider", "Tonic Water", "Protein Shake", "Mango Juice", "Berry Smoothie", "Coconut Water", "Herbal Tea", "Kombucha", "Sports Drink", "Apple Cider", "Chocolate Milk", "Matcha Latte", "Iced Coffee" },
            ["Bakery"] = new List<string> { "Bread", "Croissant", "Muffin", "Baguette", "Bagel", "Donut", "Cake", "Pie", "Scone", "Pretzel", "Roll", "Brownie", "Cupcake", "Focaccia", "Brioche", "Pita", "Danish", "Strudel", "Tart", "Panettone", "Challah", "Ciabatta", "Eclair", "Madeleine", "Shortbread", "Macaron", "Kolache", "Crumpet", "Stollen", "Kouign-Amann" },
            ["Dairy"] = new List<string> { "Milk", "Cheese", "Yogurt", "Butter", "Cream", "Cottage Cheese", "Ice Cream", "Sour Cream", "Cream Cheese", "Ghee", "Kefir", "Buttermilk", "Mascarpone", "Paneer", "Whey", "Ricotta", "Clotted Cream", "Quark", "Yogurt Drink", "Skyr", "Labneh", "Evaporated Milk", "Condensed Milk", "Goat Cheese", "Feta", "Halloumi", "Mascarpone Cheese", "Provolone", "Blue Cheese", "Cheddar" },
            ["Meat"] = new List<string> { "Chicken Breast", "Beef Steak", "Pork Chop", "Bacon", "Sausage", "Lamb Chops", "Turkey Breast", "Ham", "Ground Beef", "Pork Loin", "Veal Cutlet", "Chicken Thigh", "Ribs", "Salami", "Prosciutto", "Beef Brisket", "Chicken Wings", "Duck Breast", "Venison", "Liver", "Goose Breast", "Rabbit Meat", "Bison Steak", "Pork Belly", "Lamb Shoulder", "Beef Tenderloin", "Turkey Leg", "Chicken Drumstick", "Kielbasa", "Mortadella" },
            ["Seafood"] = new List<string> { "Salmon", "Shrimp", "Tuna", "Crab", "Lobster", "Cod", "Herring", "Sardine", "Mackerel", "Trout", "Oyster", "Clam", "Scallop", "Squid", "Octopus", "Anchovy", "Tilapia", "Snapper", "Crayfish", "Prawn", "Halibut", "Pollock", "Swordfish", "Mussels", "Catfish", "Anchovy Fillet", "Sea Bass", "Clam Chowder", "Caviar", "King Crab" },
            ["Frozen"] = new List<string> { "Frozen Peas", "Frozen Pizza", "Ice Cream", "Frozen Fish", "Frozen Vegetables", "Frozen Berries", "Frozen Corn", "Frozen Fries", "Frozen Dumplings", "Frozen Chicken Nuggets", "Frozen Waffles", "Frozen Lasagna", "Frozen Meatballs", "Frozen Spinach", "Frozen Broccoli", "Frozen Strawberries", "Frozen Mango", "Frozen Blueberries", "Frozen Vegetable Mix", "Frozen Bread Rolls", "Frozen Puff Pastry", "Frozen Tater Tots", "Frozen Burrito", "Frozen Ravioli", "Frozen Fish Sticks", "Frozen Spring Rolls", "Frozen Edamame", "Frozen Peaches", "Frozen Cherries", "Frozen Cauliflower" },
            ["Snacks"] = new List<string> { "Chips", "Chocolate Bar", "Popcorn", "Nuts", "Candy", "Cookies", "Crackers", "Granola Bar", "Trail Mix", "Pretzels", "Jerky", "Rice Cakes", "Fruit Snacks", "Gum", "Marshmallows", "Peanut Butter Cups", "Chocolate Covered Nuts", "Energy Bar", "Snack Mix", "Protein Bar", "Cheese Puffs", "Beef Jerky Bites", "Caramel Popcorn", "Nut Mix", "Cereal Bar", "Fruit Leather", "Chocolate Truffles", "Puffed Rice", "Corn Nuts", "Chocolate Pretzel" },
            ["Rewards"] = new List<string> { "Efteling ticket", "Blijdorp ticket", "Walibi ticket"}
        };

        List<ProductModel> products = new List<ProductModel>();
        Random random = new Random();
        int id = 1;

        foreach (var category in categoryProducts.Keys)
        {
            foreach (var name in categoryProducts[category])
            {
                decimal basePrice = random.Next(1, 11);
                decimal[] fractions = { 0.25m, 0.5m, 0.75m, 0.99m };
                decimal fraction = fractions[random.Next(fractions.Length)];
                decimal price = basePrice + fraction;

                ProductModel product = new ProductModel
                {
                    Name = name,
                    Price = (double)price,
                    NutritionDetails = $"Nutrition details for {name}",
                    Description = $"Description for {name}",
                    Category = category,
                    Location = random.Next(1, 16),
                    Quantity = 50 + id
                };

                products.Add(product);
                id++;
            }
        }

        List<OrdersModel> orders = new List<OrdersModel>();
        for (int i = 0; i < orderCount; i++)
        {
            int productIndex = random.Next(products.Count);
            OrdersModel order = new OrdersModel
            {
                UserID = (i % 3) + 1,
                ProductID = productIndex + 1,
                Date = DateTime.Today.AddDays(-i)
            };
            orders.Add(order);
        }

        Console.Clear();
        
        AnsiConsole.Progress()
            .AutoClear(true)
            .HideCompleted(true)
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn()
            })
            .Start(ctx =>
            {
                var userTask = ctx.AddTask("Seeding Users", maxValue: 4);
                var productTask = ctx.AddTask("Seeding Products", maxValue: products.Count);
                var orderTask = ctx.AddTask("Seeding Orders", maxValue: orders.Count);

                InsertUser(user); userTask.Increment(1);
                InsertUser(user1); userTask.Increment(1);
                InsertUser(user2); userTask.Increment(1);
                InsertUser(admin); userTask.Increment(1);
                InsertUser(SuperAdmin); userTask.Increment(1);

                foreach (var product in products)
                {
                    InsertProduct(product);
                    productTask.Increment(1);
                }

                foreach (var order in orders)
                {
                    InsertOrder(order);
                    orderTask.Increment(1);
                }
            });
    }
}
