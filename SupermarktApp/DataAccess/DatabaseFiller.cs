using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

public class DatabaseFiller
{
    private const string ConnectionString = "Data Source=database.db";
    public static List<string> allTables = new List<string>()
    {
        "Cart", "Users", "Products", "Orders", "OrderItem", "RewardItems", "Checklist", "OrderHistory", "WeeklyPromotions"
    };

    public static void RunDatabaseMethods(int orderCount = 50)
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold yellow]Starting database setup...[/]");

        AnsiConsole.Progress()
            .AutoClear(false)
            .HideCompleted(false)
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn()
            })
            .Start(ctx =>
            {
                var deleteTask = ctx.AddTask("[red]Deleting tables[/]", maxValue: allTables.Count);
                var createTask = ctx.AddTask("[green]Creating tables[/]", maxValue: 1);
                var seedUsersTask = ctx.AddTask("[cyan]Seeding Users[/]", maxValue: 5);
                var seedProductsTask = ctx.AddTask("[cyan]Seeding Products[/]", maxValue: 459);
                var seedOrdersTask = ctx.AddTask("[cyan]Seeding Orders[/]", maxValue: orderCount);
                var seedPromotionsTask = ctx.AddTask("[magenta]Seeding Promotions[/]", maxValue: 5); // New task

                // Delete tables
                DeleteTables(_ => deleteTask.Increment(1));
                deleteTask.StopTask();

                // Create tables
                CreateTables();
                createTask.Increment(1);
                createTask.StopTask();

                // Seed data
                AnsiConsole.MarkupLine("[grey]→ Seeding data...[/]");
                SeedData(orderCount,
                    usersProgress => seedUsersTask.Increment(usersProgress),
                    productsProgress => seedProductsTask.Increment(productsProgress),
                    ordersProgress => seedOrdersTask.Increment(1)
                );

                seedUsersTask.StopTask();
                seedProductsTask.StopTask();
                seedOrdersTask.StopTask();

                // Seed weekly promotions with progress
                var products = ProductAccess.GetAllProducts();
                Random random = new Random();
                var selectedProducts = products.OrderBy(p => Guid.NewGuid()).Take(5).ToList();

                foreach (var product in selectedProducts)
                {
                    double discount = random.Next(1, 7);
                    if (discount >= product.Price) discount = Math.Max(0.5, product.Price - 0.5);
                    discount = Math.Round(discount, 2);

                    InsertWeeklyPromotions(new WeeklyPromotionsModel(product.ID, discount));
                    seedPromotionsTask.Increment(1); // increment progress
                }

                seedPromotionsTask.StopTask();
            });

        AnsiConsole.MarkupLine("[bold green]✅ Database setup complete![/]");
    }

    public static void DeleteTables(Action<int>? reportProgress = null)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute("PRAGMA foreign_keys = OFF;");
        int completed = 0;
        foreach (string table in allTables)
        {
            db.Execute($"DROP TABLE IF EXISTS {table};");
            completed++;
            reportProgress?.Invoke(1);
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
            );");

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
            );");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS WeeklyPromotions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ProductID INTEGER NOT NULL,
                Discount REAL NOT NULL
            );
        ");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS OrderHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Date DATETIME NOT NULL DEFAULT (datetime('now')),
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            );");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS Orders (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserID INTEGER NOT NULL,
                ProductID INTEGER NOT NULL,
                Date DATETIME NOT NULL DEFAULT (datetime('now')),
                FOREIGN KEY (UserID) REFERENCES Users(ID) ON DELETE CASCADE,
                FOREIGN KEY (ProductID) REFERENCES Products(ID) ON DELETE CASCADE
            );");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS OrderItem (
                Id INTEGER PRIMARY KEY,
                OrderId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL,
                Price REAL NOT NULL,
                FOREIGN KEY (OrderId) REFERENCES OrderHistory(Id) ON DELETE CASCADE,
                FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
                UNIQUE(OrderId, ProductId)
            );");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS Cart (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL,
                Discount REAL NOT NULL DEFAULT 0,
                RewardPrice REAL NOT NULL DEFAULT 0,
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
                UNIQUE(UserId, ProductId)
            );");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS Checklist (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL
            );");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS RewardItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ProductId INTEGER NOT NULL,
                PriceInPoints INTEGER NOT NULL
            );");
    }

    public static void SeedData(
        int orderCount,
        Action<int>? reportUserProgress = null,
        Action<int>? reportProductProgress = null,
        Action<int>? reportOrderProgress = null)
    {
        var random = new Random();

        // USERS
        var users = new List<UserModel>
        {
            new() { Name = "Mark", LastName = "Dekker", Email = "u", Password = "u", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", City = "Rotterdam" },
            new() { Name = "Mark", LastName = "Dekker", Email = "testing1@gmail.com", Password = "123456", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", City = "Rotterdam" },
            new() { Name = "Mark", LastName = "Dekker", Email = "testing2@gmail.com", Password = "123456", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", City = "Rotterdam" },
            new() { Name = "Ben", LastName = "Dekker", Email = "a", Password = "a", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", City = "Rotterdam", AccountStatus = "Admin" },
            new() { Name = "Ben", LastName = "Dekker", Email = "sa", Password = "sa", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", City = "Rotterdam", AccountStatus = "SuperAdmin" }
        };

        // PRODUCT CATEGORIES
        var categories = new Dictionary<string, List<string>>
        {
            ["Fruits"] = new List<string> 
            { 
                "Apple", "Banana", "Orange", "Pear", "Grapes", "Pineapple", "Strawberry", "Watermelon", "Kiwi", "Mango", 
                "Peach", "Plum", "Blueberry", "Raspberry", "Blackberry", "Cherry", "Cantaloupe", "Papaya", "Lemon", "Lime", 
                "Nectarine", "Apricot", "Fig", "Pomegranate", "Tangerine", "Clementine", "Dragonfruit", "Passionfruit", "Guava", "Lychee"
            },

            ["Vegetables"] = new List<string> 
            { 
                "Carrot", "Broccoli", "Lettuce", "Spinach", "Tomato", "Cucumber", "Onion", "Pepper", "Zucchini", "Eggplant", 
                "Cauliflower", "Celery", "Mushroom", "Garlic", "Potato", "Sweet Potato", "Pumpkin", "Beetroot", "Radish", "Kale", 
                "Leek", "Brussels Sprouts", "Chard", "Artichoke", "Parsnip", "Turnip", "Okra", "Fennel", "Cabbage", "Rutabaga"
            },

            ["Beverages"] = new List<string> 
            { 
                "Orange Juice", "Cola", "Water", "Milkshake", "Coffee", "Tea", "Beer", "Wine", "Apple Juice", "Lemonade", 
                "Smoothie", "Iced Tea", "Hot Chocolate", "Energy Drink", "Sparkling Water", "Ginger Ale", "Soda", "Cider", 
                "Tonic Water", "Protein Shake", "Mango Juice", "Berry Smoothie", "Coconut Water", "Herbal Tea", "Kombucha", 
                "Sports Drink", "Apple Cider", "Chocolate Milk", "Matcha Latte", "Iced Coffee"
            },

            ["Bakery"] = new List<string> 
            { 
                "Bread", "Croissant", "Muffin", "Baguette", "Bagel", "Donut", "Cake", "Pie", "Scone", "Pretzel", 
                "Roll", "Brownie", "Cupcake", "Focaccia", "Brioche", "Pita", "Danish", "Strudel", "Tart", "Panettone", 
                "Challah", "Ciabatta", "Eclair", "Madeleine", "Shortbread", "Macaron", "Kolache", "Crumpet", "Stollen", "Kouign-Amann"
            },

            ["Dairy"] = new List<string> 
            { 
                "Milk", "Cheese", "Yogurt", "Butter", "Cream", "Cottage Cheese", "Ice Cream", "Sour Cream", "Cream Cheese", "Ghee", 
                "Kefir", "Buttermilk", "Mascarpone", "Paneer", "Whey", "Ricotta", "Clotted Cream", "Quark", "Yogurt Drink", "Skyr", 
                "Labneh", "Evaporated Milk", "Condensed Milk", "Goat Cheese", "Feta", "Halloumi", "Mascarpone Cheese", "Provolone", "Blue Cheese", "Cheddar"
            },

            ["Meat"] = new List<string> 
            { 
                "Chicken Breast", "Beef Steak", "Pork Chop", "Bacon", "Sausage", "Lamb Chops", "Turkey Breast", "Ham", "Ground Beef", "Pork Loin", 
                "Veal Cutlet", "Chicken Thigh", "Ribs", "Salami", "Prosciutto", "Beef Brisket", "Chicken Wings", "Duck Breast", "Venison", "Liver", 
                "Goose Breast", "Rabbit Meat", "Bison Steak", "Pork Belly", "Lamb Shoulder", "Beef Tenderloin", "Turkey Leg", "Chicken Drumstick", "Kielbasa", "Mortadella"
            },

            ["Seafood"] = new List<string> 
            { 
                "Salmon", "Shrimp", "Tuna", "Crab", "Lobster", "Cod", "Herring", "Sardine", "Mackerel", "Trout", 
                "Oyster", "Clam", "Scallop", "Squid", "Octopus", "Anchovy", "Tilapia", "Snapper", "Crayfish", "Prawn", 
                "Halibut", "Pollock", "Swordfish", "Mussels", "Catfish", "Anchovy Fillet", "Sea Bass", "Caviar", "King Crab", "Haddock"
            },

            ["Frozen"] = new List<string> 
            { 
                "Frozen Peas", "Frozen Pizza", "Ice Cream", "Frozen Fish", "Frozen Vegetables", "Frozen Berries", "Frozen Corn", "Frozen Fries", 
                "Frozen Dumplings", "Frozen Chicken Nuggets", "Frozen Waffles", "Frozen Lasagna", "Frozen Meatballs", "Frozen Spinach", "Frozen Broccoli", 
                "Frozen Strawberries", "Frozen Mango", "Frozen Blueberries", "Frozen Vegetable Mix", "Frozen Bread Rolls", "Frozen Puff Pastry", 
                "Frozen Tater Tots", "Frozen Burrito", "Frozen Ravioli", "Frozen Fish Sticks", "Frozen Spring Rolls", "Frozen Edamame", "Frozen Peaches", "Frozen Cherries", "Frozen Cauliflower"
            },

            ["Snacks"] = new List<string> 
            { 
                "Chips", "Chocolate Bar", "Popcorn", "Nuts", "Candy", "Cookies", "Crackers", "Granola Bar", "Trail Mix", "Pretzels", 
                "Jerky", "Rice Cakes", "Fruit Snacks", "Gum", "Marshmallows", "Peanut Butter Cups", "Chocolate Covered Nuts", "Energy Bar", 
                "Snack Mix", "Protein Bar", "Cheese Puffs", "Beef Jerky Bites", "Caramel Popcorn", "Nut Mix", "Cereal Bar", "Fruit Leather", 
                "Chocolate Truffles", "Puffed Rice", "Corn Nuts", "Chocolate Pretzel"
            },

            ["Pantry"] = new List<string>
            {
                "Pasta", "Rice", "Flour", "Sugar", "Salt", "Pepper", "Olive Oil", "Vinegar", "Canned Beans", "Canned Tuna",
                "Canned Tomatoes", "Tomato Sauce", "Peanut Butter", "Jam", "Honey", "Maple Syrup", "Soy Sauce", "Ketchup",
                "Mustard", "Mayonnaise", "Hot Sauce", "Cooking Oil", "Curry Paste", "Coconut Milk", "Oats", "Baking Powder",
                "Breadcrumbs", "Spices Mix", "Cereal", "Instant Noodles"
            },

            ["Personal Care"] = new List<string>
            {
                "Shampoo", "Conditioner", "Soap", "Body Wash", "Toothpaste", "Toothbrush", "Deodorant", "Hand Sanitizer",
                "Face Wash", "Lotion", "Shaving Cream", "Razor", "Mouthwash", "Perfume", "Cotton Swabs", "Wet Wipes",
                "Hair Gel", "Hair Spray", "Face Cream", "Sunscreen", "Lip Balm", "Nail Polish", "Makeup Remover", "Toilet Paper",
                "Tissues", "Body Spray", "Hand Cream", "Bath Salts", "Foot Cream", "Aftershave"
            },

            ["Cleaning Supplies"] = new List<string>
            {
                "Dish Soap", "Laundry Detergent", "Fabric Softener", "Bleach", "All-Purpose Cleaner", "Glass Cleaner",
                "Toilet Cleaner", "Floor Cleaner", "Disinfectant Wipes", "Sponges", "Paper Towels", "Garbage Bags",
                "Mop", "Broom", "Dustpan", "Scrub Brush", "Laundry Pods", "Air Freshener", "Oven Cleaner", "Carpet Cleaner",
                "Descaler", "Drain Cleaner", "Stain Remover", "Ironing Spray", "Surface Polish", "Scented Candles", "Vacuum Bags", "Dryer Sheets", "Cleaning Gloves", "Bucket"
            },

            ["Baby Products"] = new List<string>
            {
                "Diapers", "Baby Wipes", "Baby Lotion", "Baby Powder", "Baby Shampoo", "Baby Soap", "Baby Oil", "Baby Food",
                "Baby Formula", "Pacifier", "Baby Bottle", "Teething Ring", "Baby Blanket", "Baby Cream", "Wet Wipes", "Baby Snacks",
                "Baby Bib", "Baby Washcloth", "Baby Rattle", "Baby Toothbrush", "Baby Cup", "Infant Cereal", "Baby Lotion Bar", "Baby Bath Oil", "Baby Hair Brush", "Baby Nail Scissors", "Diddy oil", "Baby Milk Powder", "Baby Spoon", "Baby Rice Cereal", "Baby Shampoo Bar"
            },

            ["Pet Supplies"] = new List<string>
            {
                "Dog Food", "Cat Food", "Bird Seed", "Fish Flakes", "Cat Litter", "Dog Treats", "Cat Treats", "Pet Shampoo",
                "Dog Leash", "Cat Collar", "Pet Bed", "Pet Toy", "Pet Bowl", "Dog Chew Bone", "Catnip", "Pet Carrier",
                "Dog Jacket", "Cat Brush", "Hamster Wheel", "Pet Vitamins", "Aquarium Filter", "Dog Toothbrush", "Pet Cage", "Pet Blanket", "Pet Grooming Kit", "Pet Deodorizer", "Pet Nail Clippers", "Reptile Lamp", "Cat Scratcher", "Bird Cage"
            },

            ["Health & Wellness"] = new List<string>
            {
                "Vitamins", "Painkillers", "Cough Syrup", "First Aid Kit", "Thermometer", "Bandages", "Hand Sanitizer", "Disinfectant Spray",
                "Cotton Balls", "Medical Gloves", "Antibiotic Cream", "Allergy Pills", "Digestive Aid", "Eye Drops", "Nasal Spray", "Multivitamin Gummies",
                "Protein Powder", "Electrolyte Drink", "Herbal Supplements", "Omega 3 Capsules", "Calcium Tablets", "Vitamin C", "Vitamin D", "Magnesium Supplement", "Zinc Tablets", "Pain Relief Gel", "Heating Pad", "Cold Pack", "Face Mask", "Sanitary Pads"
            },

            ["Rewards"] = new List<string> 
            { 
                "Efteling Ticket", "Blijdorp Ticket", "Walibi Ticket", "Cinema Voucher", "Dinner Voucher", "Theme Park Pass", "Museum Entry", "Gift Card"
            }
        };

        int totalProducts = categories.Values.Sum(list => list.Count); // 459 products in total IF U ADD ANY OTHER PRODUCTS UPDATE THE seedProductsTask MAX VALUE IN RunDatabaseMethods :D

        var products = new List<ProductModel>();
        foreach (var entry in categories)
        {
            string category = entry.Key;
            foreach (string name in entry.Value)
            {
                products.Add(new ProductModel
                {
                    Name = name,
                    Price = Math.Round(random.NextDouble() * 10 + 1, 2),
                    NutritionDetails = $"Nutrition details for {name}",
                    Description = $"Description for {name}",
                    Category = category,
                    Location = random.Next(1, 16),
                    Quantity = random.Next(20, 300),
                });
            }
        }

        // ORDERS
        var orders = new List<OrdersModel>();
        for (int i = 0; i < orderCount; i++)
        {
            orders.Add(new OrdersModel
            {
                UserID = (i % users.Count) + 1,
                ProductID = random.Next(products.Count) + 1,
                Date = DateTime.Today.AddDays(-i)
            });
        }

        // INSERT USERS
        foreach (var user in users)
        {
            InsertUser(user);
            reportUserProgress?.Invoke(1);
        }

        // INSERT PRODUCTS
        foreach (var product in products)
        {
            InsertProduct(product);
            reportProductProgress?.Invoke(1);
        }

        // INSERT ORDERS
        foreach (var order in orders)
        {
            InsertOrder(order);
            reportOrderProgress?.Invoke(1);
        }

        // REWARD ITEMS
        RewardItemsAccess.AddRewardItem(new RewardItemsModel(1, 50));
        RewardItemsAccess.AddRewardItem(new RewardItemsModel(2, 60));
        RewardItemsAccess.AddRewardItem(new RewardItemsModel(3, 30));
    }

    public static void InsertUser(UserModel user)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO Users (Name, LastName, Email, Password, Address, Zipcode, PhoneNumber, City, AccountStatus)
                     VALUES (@Name, @LastName, @Email, @Password, @Address, @Zipcode, @PhoneNumber, @City, @AccountStatus);", user);
    }

    public static void InsertProduct(ProductModel product)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO Products (Name, Price, NutritionDetails, Description, Category, Location, Quantity)
                     VALUES (@Name, @Price, @NutritionDetails, @Description, @Category, @Location, @Quantity);", product);
    }

    public static void InsertOrder(OrdersModel order)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO Orders (UserID, ProductID, Date)
                     VALUES (@UserID, @ProductID, @Date);", order);
    }

    public static void InsertWeeklyPromotions(WeeklyPromotionsModel model)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO WeeklyPromotions (ProductID, Discount) 
                     VALUES (@ProductID, @Discount);", model);
    }

    public static int InsertOrderHistory(OrderHistoryModel order)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Open();
        db.Execute("PRAGMA foreign_keys = ON;");
        string sql = @" 
            INSERT INTO OrderHistory (UserId, Date)
            VALUES (@UserId, @Date);
            SELECT last_insert_rowid();
        ";
        int orderId = db.ExecuteScalar<int>(sql, order);
        return orderId;
    }

    public static void InsertOrderItem(int orderId, int productId, int quantity, double price)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Open();
        db.Execute(@"
            INSERT INTO OrderItem (OrderId, ProductId, Quantity, Price)
            VALUES (@OrderId, @ProductId, @Quantity, @Price)
            ON CONFLICT(OrderId, ProductId)
            DO UPDATE SET
                Quantity = excluded.Quantity,
                Price = excluded.Price;",
            new { OrderId = orderId, ProductId = productId, Quantity = quantity, Price = price }
        );
    }

    public static void SeedWeeklyPromotions()
    {
        var products = ProductAccess.GetAllProducts();
        if (products.Count < 5)
            return;

        Random random = new Random();

        var selectedProducts = products
            .OrderBy(p => Guid.NewGuid())
            .Take(5)
            .ToList();

        foreach (var product in selectedProducts)
        {
            double discount = random.Next(1, 7); // 1–6 euros
            if (discount >= product.Price)
            {
                discount = Math.Max(0.5, product.Price - 0.5);
            }
            discount = Math.Round(discount, 2);
            InsertWeeklyPromotions(new WeeklyPromotionsModel(product.ID, discount));
        }
    }
}
