using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

public class DatabaseFiller
{
    private const string ConnectionString = "Data Source=database.db";
    private static SqliteConnection? _sharedConnection;

    public static List<string> allTables = new List<string>()
    {
        "Cart", "Users", "Products", "Orders", "RewardItems", "Checklist", "OrderHistory", "Discounts", "ShopInfo", "Coupon"
    };

    public static void RunDatabaseMethods(int orderCount = 50)
    {
        try { Console.Clear(); } catch { /* ignore if no console */ }
        AnsiConsole.MarkupLine("[bold yellow]Starting database setup...[/]");

        _sharedConnection = new SqliteConnection(ConnectionString);
        _sharedConnection.Open();

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
                var seedProductsTask = ctx.AddTask("[cyan]Seeding Products[/]", maxValue: 453);
                var seedOrdersTask = ctx.AddTask("[cyan]Seeding Orders[/]", maxValue: orderCount);

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
            });

        AnsiConsole.MarkupLine("[bold green]✅ Database setup complete![/]");
        _sharedConnection.Close();
        _sharedConnection.Dispose();
        _sharedConnection = null;
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
                Birthdate DATETIME,
                City TEXT,
                AccountStatus TEXT,
                AccountPoints INTEGER DEFAULT 0,
                TWOFAEnabled INTEGER DEFAULT 0,
                TWOFACode TEXT,
                TWOFAExpiry DATETIME,
                LastBirthdayGift DATETIME NULL
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
                Quantity INTEGER NOT NULL DEFAULT 0,
                Visible INTEGER NOT NULL DEFAULT 1,
                DiscountPercentage REAL NOT NULL DEFAULT 0,
                DiscountType TEXT NOT NULL DEFAULT 'None'
            );");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS Discounts (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            ProductId INT NOT NULL,
            UserId INT NULL,
            DiscountPercentage REAL NOT NULL,
            DiscountType TEXT NOT NULL,
            StartDate DATETIME NULL,
            EndDate DATETIME NULL
            );");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS OrderHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Date DATETIME NOT NULL DEFAULT (datetime('now')),
                IsPaid BOOLEAN NOT NULL DEFAULT 1,
                FineDate DATETIME DEFAULT NULL,
                PaymentCode INTEGER,
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            );");

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS Orders (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserID INTEGER NOT NULL,
                OrderId INTEGER NOT NULL,
                ProductID INTEGER NOT NULL,
                Price REAL NOT NULL,
                Date DATETIME NOT NULL DEFAULT (datetime('now')),
                FOREIGN KEY (UserID) REFERENCES Users(ID) ON DELETE CASCADE,
                FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
                FOREIGN KEY (ProductID) REFERENCES Products(ID) ON DELETE CASCADE,
                FOREIGN KEY (OrderId) REFERENCES OrderHistory(Id) ON DELETE CASCADE
            );");


        db.Execute(@"
            CREATE TABLE IF NOT EXISTS Cart (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL,
                RewardPrice DOUBLE NOT NULL DEFAULT 0,
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

        db.Execute(@"
            CREATE TABLE IF NOT EXISTS ShopInfo (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Description TEXT,
                OpeningHourMonday TEXT,
                ClosingHourMonday TEXT,
                OpeningHourTuesday TEXT,
                ClosingHourTuesday TEXT,
                OpeningHourWednesday TEXT,
                ClosingHourWednesday TEXT,
                OpeningHourThursday TEXT,
                ClosingHourThursday TEXT,
                OpeningHourFriday TEXT,
                ClosingHourFriday TEXT,
                OpeningHourSaturday TEXT,
                ClosingHourSaturday TEXT,
                OpeningHourSunday TEXT,
                ClosingHourSunday TEXT
            );
        ");
        db.Execute(@"
            CREATE TABLE IF NOT EXISTS Coupon (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTERGER NOT NULL,
                Credit DOUBLE NOT NULL,
                IsValid BOOLEAN NOT NULL DEFAULT 1,
                FOREIGN KEY (UserID) REFERENCES Users(ID) ON DELETE CASCADE
            );
        ");
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
            new() { Name = "Mark", LastName = "Dekker", Email = "u", Password = "u", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", Birthdate = new DateTime(2005, 11, 13), City = "Rotterdam"},
            new() { Name = "Mark", LastName = "Dekker", Email = "devinnijhof@gmail.com", Password = "u", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", Birthdate = new DateTime(random.Next(1950, 2005), random.Next(1, 13), random.Next(1, 29)), City = "Rotterdam", TwoFAEnabled = true }, // 2FA TEST ACCOUNT
            new() { Name = "Mark", LastName = "Dekker", Email = "testing2@gmail.com", Password = "123456", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", Birthdate = new DateTime(random.Next(1950, 2005), random.Next(1, 13), random.Next(1, 29)), City = "Rotterdam" },
            new() { Name = "Ben", LastName = "Dekker", Email = "a", Password = "a", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", Birthdate = new DateTime(random.Next(1950, 2005), random.Next(1, 13), random.Next(1, 29)), City = "Rotterdam", TwoFAEnabled = false, AccountStatus = "Admin" },
            new() { Name = "Ben", LastName = "Dekker", Email = "sa", Password = "sa", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", Birthdate = new DateTime(random.Next(1950, 2005), random.Next(1, 13), random.Next(1, 29)), City = "Rotterdam", TwoFAEnabled = false, AccountStatus = "SuperAdmin" }
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
                "Face Wash", "Lotion", "Shaving Cream", "Razor", "Mouthwash", "Perfume", "Cotton Swabs",
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
                "Efteling Ticket", "Blijdorp Ticket", "Walibi Ticket"
            }
        };
        

        int totalProducts = categories.Values.Sum(list => list.Count); // 453 products in total IF U ADD ANY OTHER PRODUCTS UPDATE THE seedProductsTask MAX VALUE IN RunDatabaseMethods :D

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


        // REWARD ITEMS
        RewardItemsAccess.AddRewardItem(new RewardItemsModel(451, 50));
        RewardItemsAccess.AddRewardItem(new RewardItemsModel(452, 60));
        RewardItemsAccess.AddRewardItem(new RewardItemsModel(453, 30));

        ProductModel RewardItem1 = ProductAccess.GetProductByID(451)!;
        ProductModel RewardItem2 = ProductAccess.GetProductByID(452)!;
        ProductModel RewardItem3 = ProductAccess.GetProductByID(453)!;

        RewardItem1.Price = 0;
        RewardItem2.Price = 0;
        RewardItem3.Price = 0;

        ProductAccess.ChangeProductDetails(RewardItem1);
        ProductAccess.ChangeProductDetails(RewardItem2);
        ProductAccess.ChangeProductDetails(RewardItem3);

        // MAKE REWARD ITEMS NON VISIBLE IN ORDER SEARCH
        ProductAccess.SetProductVisibility(451, false);
        ProductAccess.SetProductVisibility(452, false);
        ProductAccess.SetProductVisibility(453, false);

        // Add birthday Item
        var birthdayPresent = new ProductModel(
            name: "Birthday Present",
            price: 0.00, // Free gift
            nutritionDetails: "N/A",
            description: "A free gift to celebrate your birthday! Enjoy your special day.",
            category: "Gifts",
            location: 0,
            quantity: 999,
            visible: 0
        );
        ProductAccess.AddProduct(birthdayPresent);

        // SEED DISCOUNTS
        var weeklyDiscounts = new List<DiscountsModel>
        {
            new DiscountsModel(1, 10, "Weekly", DateTime.Now, DateTime.Now.AddDays(7)),
            new DiscountsModel(2, 15, "Weekly", DateTime.Now, DateTime.Now.AddDays(7)),
            new DiscountsModel(3, 20, "Weekly", DateTime.Now, DateTime.Now.AddDays(7)),
            new DiscountsModel(4, 5, "Weekly", DateTime.Now, DateTime.Now.AddDays(7)),
            new DiscountsModel(5, 12, "Weekly", DateTime.Now, DateTime.Now.AddDays(7))
        };

        // Voeg Weekly kortingen toe
        foreach (var discount in weeklyDiscounts)
        {
            DiscountsAccess.AddDiscount(discount);
        }
        
        // ORDER HISTORY
        var orderHistoryList = new List<OrderHistoryModel>();
        for (int i = 0; i < orderCount/15; i++)
        {
            var history = new OrderHistoryModel
            {
                UserId = (i % users.Count) + 1,
                Date = DateTime.Today.AddDays(-i * (36500 / orderCount))
            };

            int orderHistoryId = InsertOrderHistory(history);
            orderHistoryList.Add(history);

            int itemCount = random.Next(1,3); // 1–2 items per order
            for (int j = 0; j < itemCount; j++)
            {
                var product = products[random.Next(products.Count)];
                InsertOrderItem(history.UserId, orderHistoryId, products.IndexOf(product) + 1, product.Price, history.Date);
            }

            // ✅ increment once per order
            reportOrderProgress?.Invoke(1);
        }


        // Seed default shop info
        var defaultShopInfo = new ShopInfoModel
        {
            Description = @"
            Welcome to our supermarket — where freshness comes first.
            Our bakery opens early with warm, freshly baked bread, and all our vegetables are kept perfectly cooled throughout the day.
            Most restocking takes place in the evening, so the shelves are full and ready for you every morning.",

            OpeningHourMonday = "07:00",
            ClosingHourMonday = "22:00",

            OpeningHourTuesday = "07:00",
            ClosingHourTuesday = "22:00",

            OpeningHourWednesday = "07:00",
            ClosingHourWednesday = "22:00",

            OpeningHourThursday = "07:00",
            ClosingHourThursday = "22:00",

            OpeningHourFriday = "07:00",
            ClosingHourFriday = "22:00",

            OpeningHourSaturday = "08:00",
            ClosingHourSaturday = "20:00",

            OpeningHourSunday = "08:00",
            ClosingHourSunday = "20:00"
        };

        // Save or update in DB
        ShopInfoAccess.UpdateShopInfo(defaultShopInfo);
    }

    public static void InsertUser(UserModel user)
    {
        _sharedConnection!.Execute(@"
        INSERT INTO Users (Name, LastName, Email, Password, Address, Zipcode, PhoneNumber, Birthdate, City, AccountStatus)
        VALUES (@Name, @LastName, @Email, @Password, @Address, @Zipcode, @PhoneNumber, @Birthdate, @City, @AccountStatus);", user);
    }

    public static void InsertProduct(ProductModel product)
    {
        _sharedConnection!.Execute(@"
        INSERT INTO Products (Name, Price, NutritionDetails, Description, Category, Location, Quantity)
        VALUES (@Name, @Price, @NutritionDetails, @Description, @Category, @Location, @Quantity);", product);
    }

    // public static void InsertOrder(OrdersModel order)
    // {
    //     _sharedConnection.Execute(@"
    //     INSERT INTO Orders (UserID, ProductID, Date)
    //     VALUES (@UserID, @ProductID, @Date);", order);
    // }

    public static int InsertOrderHistory(OrderHistoryModel order)
    {
        _sharedConnection!.Execute("PRAGMA foreign_keys = ON;");
        string sql = @" 
            INSERT INTO OrderHistory (UserId, Date)
            VALUES (@UserId, @Date);
            SELECT last_insert_rowid();
        ";
        int orderId = _sharedConnection!.ExecuteScalar<int>(sql, order);
        return orderId;
    }

    public static void InsertOrderItem(int UserID, int orderId, int productId, double price, DateTime date)
    {
        _sharedConnection!.Execute(@"
            INSERT INTO Orders (UserID, OrderId, ProductId, Price, Date)
            VALUES (@UserID, @OrderId, @ProductId, @Price, @Date);",
            new { UserID = UserID, OrderId = orderId, ProductId = productId, Price = price, Date = date }
        );
    }
}