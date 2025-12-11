using Dapper;
using Microsoft.Data.Sqlite;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

public class DatabaseFiller
{
    private const string ConnectionString = "Data Source=database.db";
    private static readonly IDatabaseFactory _dbFactory = new SqliteDatabaseFactory(ConnectionString);
    private static SqliteConnection? _sharedConnection;

    public static List<string> allTables = new List<string>()
    {
        "Cart", "Users", "Products", "Orders", "RewardItems",
        "Checklist", "OrderHistory",  "ShopInfo", "ShopReviews", "Discounts", "Coupon",
        "FavoriteLists", "FavoriteListProducts", "RestockHistory"
    };

    public static void RunDatabaseMethods(int orderCount = 50)
    {
        try { Console.Clear(); } catch { }
        AnsiConsole.MarkupLine("[bold yellow]Starting database setup...[/]");

        var users = BuildUsers();
        var categories = BuildCategories();

        int totalProducts = categories.Values.Sum(list => list.Count);
        int orderHistoryCount = Math.Max(1, orderCount / 10);

        _sharedConnection = _dbFactory.GetConnection();

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
                var seedUsersTask = ctx.AddTask("[cyan]Seeding Users[/]", maxValue: users.Count);
                var seedProductsTask = ctx.AddTask("[cyan]Seeding Products[/]", maxValue: totalProducts);
                var seedOrdersTask = ctx.AddTask("[cyan]Seeding Orders[/]", maxValue: orderHistoryCount);

                DeleteTables(_ => deleteTask.Increment(1));
                deleteTask.StopTask();

                CreateTables();
                createTask.Increment(1);
                createTask.StopTask();

                AnsiConsole.MarkupLine("[grey]→ Seeding data...[/]");
                SeedData(orderCount, users, categories,
                    usersProgress => seedUsersTask.Increment(usersProgress),
                    productsProgress => seedProductsTask.Increment(productsProgress),
                    ordersProgress => seedOrdersTask.Increment(ordersProgress)
                );

                seedUsersTask.StopTask();
                seedProductsTask.StopTask();
                seedOrdersTask.StopTask();
            });

        AnsiConsole.MarkupLine("[bold green]✅ Database setup complete![/]");
        try
        {
            _sharedConnection?.Close();
            _sharedConnection?.Dispose();
            _sharedConnection = null;
        }
        catch { }
    }

    public static void DeleteTables(Action<int>? reportProgress = null)
    {
        _sharedConnection!.Execute("PRAGMA foreign_keys = OFF;");
        foreach (string table in allTables)
        {
            _sharedConnection!.Execute($"DROP TABLE IF EXISTS {table};");
            reportProgress?.Invoke(1);
        }
        _sharedConnection!.Execute("PRAGMA foreign_keys = ON;");
    }

    public static void CreateTables()
    {
        _sharedConnection!.Execute("PRAGMA foreign_keys = ON;");

        _sharedConnection!.Execute(@"
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

        _sharedConnection!.Execute(@"
            CREATE TABLE IF NOT EXISTS Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price REAL NOT NULL,
                NutritionDetails TEXT,
                Description TEXT,
                Category TEXT,
                Location INTEGER,
                ExpiryDate DATETIME,
                Quantity INTEGER NOT NULL DEFAULT 0,
                Visible INTEGER NOT NULL DEFAULT 1
            );");

        _sharedConnection!.Execute(@"
            CREATE TABLE IF NOT EXISTS Discounts (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            ProductId INT NOT NULL,
            UserId INT NULL,
            DiscountPercentage REAL NOT NULL,
            DiscountType TEXT NOT NULL,
            StartDate DATETIME NULL,
            EndDate DATETIME NULL
            );");

        _sharedConnection!.Execute(@"
            CREATE TABLE IF NOT EXISTS OrderHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Date DATETIME NOT NULL DEFAULT (datetime('now')),
                IsPaid BOOLEAN NOT NULL DEFAULT 1,
                FineDate DATETIME DEFAULT NULL,
                PaymentCode INTEGER,
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            );");

        _sharedConnection!.Execute(@"
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

        _sharedConnection!.Execute(@"
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

        _sharedConnection!.Execute(@"
            CREATE TABLE IF NOT EXISTS Checklist (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL
            );");

        _sharedConnection!.Execute(@"
            CREATE TABLE IF NOT EXISTS RewardItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ProductId INTEGER NOT NULL,
                PriceInPoints INTEGER NOT NULL
            );");

        _sharedConnection!.Execute(@"
            CREATE TABLE IF NOT EXISTS ShopInfo (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Day TEXT,
                OpeningHour TEXT,
                ClosingHour TEXT
            );");

        _sharedConnection!.Execute(@"
            CREATE TABLE IF NOT EXISTS ShopReviews (
                Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                UserId INTEGER NOT NULL,
                Stars INTEGER NOT NULL,
                Text TEXT, 
                CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE
            );
        ");
        _sharedConnection!.Execute(@"
            CREATE TABLE IF NOT EXISTS Coupon (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTERGER NOT NULL,
                Credit DOUBLE NOT NULL,
                IsValid BOOLEAN NOT NULL DEFAULT 1,
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            );
        ");
        _sharedConnection!.Execute(@"
            CREATE TABLE IF NOT EXISTS FavoriteLists (
                Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId   INTEGER NOT NULL,
                Name     TEXT NOT NULL,
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            );
        ");
        _sharedConnection!.Execute(@"
            CREATE TABLE IF NOT EXISTS FavoriteListProducts (
                Id            INTEGER PRIMARY KEY AUTOINCREMENT,
                FavoriteListId INTEGER NOT NULL,
                ProductId      INTEGER NOT NULL,
                Quantity       INTEGER NOT NULL,
                FOREIGN KEY (FavoriteListId) REFERENCES FavoriteLists(Id) ON DELETE CASCADE,
                FOREIGN KEY (ProductId)      REFERENCES Products(Id)
            );
        ");
        _sharedConnection!.Execute(@"
            CREATE TABLE IF NOT EXISTS RestockHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ProductID INTEGER NOT NULL,
                QuantityAdded INTEGER NOT NULL,
                RestockDate DATETIME NOT NULL DEFAULT (datetime('now')),
                CostPerUnit REAL NOT NULL,
                FOREIGN KEY (ProductID) REFERENCES Products(ID) ON DELETE CASCADE
            );");
    }

    public static void SeedData(
        int orderCount,
        List<UserModel>? users = null,
        Dictionary<string, List<string>>? categories = null,
        Action<int>? reportUserProgress = null,
        Action<int>? reportProductProgress = null,
        Action<int>? reportOrderProgress = null)
    {
        var random = new Random();

        if (users == null) users = BuildUsers();
        if (categories == null) categories = BuildCategories();

        var products = new List<ProductModel>();
        foreach (var entry in categories)
        {
            string category = entry.Key;
            foreach (string name in entry.Value)
            {
                var price = GetPriceForCategory(category, random);
                var nutrition = GenerateNutritionDetails(name, category, random);
                var description = GenerateDescription(name, category);
                var expiry = GetExpiryDate(category, random);

                products.Add(new ProductModel
                {
                    Name = name,
                    Price = Math.Round(price, 2),
                    NutritionDetails = nutrition,
                    Description = description,
                    Category = category,
                    Location = random.Next(1, 16),
                    Quantity = random.Next(20, 300),
                    Visible = 1,
                    ExpiryDate = expiry
                });
            }
        }

        foreach (var user in users)
        {
            InsertUser(user);
            reportUserProgress?.Invoke(1);
        }

        foreach (var product in products)
        {
            InsertProduct(product);
            reportProductProgress?.Invoke(1);
        }

        DiscountsLogic.AddExpiryDateDiscounts(1, 50); // Add default expiry date discounts

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

        ProductAccess.SetProductVisibility(451, true);
        ProductAccess.SetProductVisibility(452, true);
        ProductAccess.SetProductVisibility(453, true);

        var birthdayPresent = new ProductModel(
            name: "Birthday Present",
            price: 0.00,
            nutritionDetails: "N/A",
            description: "A free gift to celebrate your birthday! Enjoy your special day.",
            category: "Gifts",
            location: 0,
            quantity: 999,
            visible: 0
        );
        ProductAccess.AddProduct(birthdayPresent);

        List<ShopInfoModel> listWeek = new List<ShopInfoModel>
        {
            new ShopInfoModel(@"
            Welcome to our supermarket — where freshness comes first.
            Our bakery opens early with warm, freshly baked bread, and all our vegetables are kept perfectly cooled throughout the day.
            Most restocking takes place in the evening, so the shelves are full and ready for you every morning.", null!, null!),
            new ShopInfoModel("Monday", "12:00", "22:00"),
            new ShopInfoModel("Tuesday", "12:00", "22:00"),
            new ShopInfoModel("Wednesday", "12:00", "22:00"),
            new ShopInfoModel("Thursday", "12:00", "22:00"),
            new ShopInfoModel("Friday", "12:00", "22:00"),
            new ShopInfoModel("Saturday", "12:00", "22:00"),
            new ShopInfoModel("Sunday", "12:00", "22:00")
        };

        foreach (var week in listWeek)
        {
            ShopInfoAccess.AddDay(week);
        }

        var weeklyDiscounts = new List<DiscountsModel>();
        var chosen = new HashSet<int>();
        int productCount = products.Count;
        while (chosen.Count < 5 && chosen.Count < productCount)
        {
            int pick = random.Next(1, productCount + 1);
            if (chosen.Add(pick))
            {
                double percent = Math.Round(random.NextDouble() * 25 + 5, 2);
                weeklyDiscounts.Add(new DiscountsModel(pick, percent, "Weekly", DateTime.Now, DateTime.Now.AddDays(7)));
            }
        }

        foreach (var discount in weeklyDiscounts)
        {
            DiscountsAccess.AddDiscount(discount);
        }

        for (int i = 0; i < Math.Min(50, products.Count); i++)
        {
            var restockEntry = new RestockHistoryModel
            {
                ProductID = i + 1,
                QuantityAdded = random.Next(1, 25),
                RestockDate = DateTime.Today.AddDays(-random.Next(1, 100)),
                CostPerUnit = Math.Round(products[i].Price * (random.NextDouble() * 0.5 + 0.5), 2)
            };

            InsertRestockHistory(restockEntry);
        }

        var orderHistoryList = new List<OrderHistoryModel>();
        int orderHistoryCount = Math.Max(1, orderCount / 10);
        for (int i = 0; i < orderHistoryCount; i++)
        {
            var history = new OrderHistoryModel
            {
                UserId = (i % users.Count) + 1,
                Date = DateTime.Today.AddDays(-i * (36500 / Math.Max(1, orderCount)))
            };

            int orderHistoryId = InsertOrderHistory(history);
            orderHistoryList.Add(history);

            int itemCount = random.Next(1, 20);
            for (int j = 0; j < itemCount; j++)
            {
                var product = products[random.Next(products.Count)];
                InsertOrderItem(history.UserId, orderHistoryId, products.IndexOf(product) + 1, product.Price, history.Date);
            }

            reportOrderProgress?.Invoke(1);
        }

        var sampleReviews = new List<ShopReviewModel>
        {
            new() { UserId = 1, Stars = 5, Text = "Absolutely love this place! Always fresh produce and friendly staff." },
            new() { UserId = 2, Stars = 4, Text = "Great selection and clean aisles. Prices could be a bit better though." },
            new() { UserId = 3, Stars = 5, Text = "Their bakery section is divine. The croissants are next-level!" },
            new() { UserId = 1, Stars = 3, Text = "Good overall, but sometimes crowded in the evenings." },
            new() { UserId = 4, Stars = 5, Text = "Fantastic service, especially during weekends. Highly recommend!" }
        };

        foreach (var review in sampleReviews)
        {
            InsertShopReview(review);
        }
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
        INSERT INTO Products (Name, Price, NutritionDetails, Description, Category, Location, Quantity, ExpiryDate, Visible)
        VALUES (@Name, @Price, @NutritionDetails, @Description, @Category, @Location, @Quantity, @ExpiryDate, @Visible);", product);
    }

    public static void InsertRestockHistory(RestockHistoryModel restockEntry)
    {
        _sharedConnection!.Execute(@"
        INSERT INTO RestockHistory (ProductID, QuantityAdded, RestockDate, CostPerUnit)
        VALUES (@ProductID, @QuantityAdded, @RestockDate, @CostPerUnit);", restockEntry);
    }

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

    public static void InsertShopReview(ShopReviewModel review)
    {
        _sharedConnection?.Execute(@"
            INSERT INTO ShopReviews (UserId, Stars, Text)
            VALUES (@UserId, @Stars, @Text);
        ", review);
    }

    private static List<UserModel> BuildUsers()
    {
        var random = new Random();
        return new List<UserModel>
        {
            new() { Name = "Mark", LastName = "Dekker", Email = "u", Password = "u", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "3143256787", Birthdate = new DateTime(2005, 11, 13), City = "Rotterdam"},
            new() { Name = "Mark", LastName = "Dekker", Email = "chouchenghong@gmail.com", Password = "u", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "3143256897", Birthdate = new DateTime(2005, 11, 13), City = "Rotterdam"},
            new() { Name = "Mark", LastName = "Dekker", Email = "devinnijhof@gmail.com", Password = "u", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "3143256797", Birthdate = new DateTime(random.Next(1950, 2005), random.Next(1, 13), random.Next(1, 29)), City = "Rotterdam", TwoFAEnabled = true },
            new() { Name = "Mark", LastName = "Dekker", Email = "testing2@gmail.com", Password = "123456", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "3143257897", Birthdate = new DateTime(random.Next(1950, 2005), random.Next(1, 13), random.Next(1, 29)), City = "Rotterdam" },
            new() { Name = "Ben", LastName = "Dekker", Email = "a", Password = "a", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "3143567897", Birthdate = new DateTime(random.Next(1950, 2005), random.Next(1, 13), random.Next(1, 29)), City = "Rotterdam", TwoFAEnabled = false, AccountStatus = "Admin" },
            new() { Name = "Ben", LastName = "Dekker", Email = "sa", Password = "sa", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "3142567897", Birthdate = new DateTime(random.Next(1950, 2005), random.Next(1, 13), random.Next(1, 29)), City = "Rotterdam", TwoFAEnabled = false, AccountStatus = "SuperAdmin" }
        };
    }

    private static Dictionary<string, List<string>> BuildCategories()
    {
        return new Dictionary<string, List<string>>
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
    }

    private static double GetPriceForCategory(string category, Random r)
    {
        switch (category)
        {
            case "Fruits": return r.NextDouble() * 3 + 0.5;
            case "Vegetables": return r.NextDouble() * 3 + 0.5;
            case "Bakery": return r.NextDouble() * 4 + 0.8;
            case "Dairy": return r.NextDouble() * 6 + 0.5;
            case "Meat": return r.NextDouble() * 12 + 3;
            case "Seafood": return r.NextDouble() * 16 + 4;
            case "Frozen": return r.NextDouble() * 6 + 1;
            case "Beverages": return r.NextDouble() * 4 + 0.5;
            case "Snacks": return r.NextDouble() * 3 + 0.5;
            case "Pantry": return r.NextDouble() * 7 + 1;
            case "Personal Care": return r.NextDouble() * 8 + 1;
            case "Cleaning Supplies": return r.NextDouble() * 8 + 1;
            case "Baby Products": return r.NextDouble() * 10 + 1;
            case "Pet Supplies": return r.NextDouble() * 8 + 1;
            case "Health & Wellness": return r.NextDouble() * 12 + 1;
            default: return r.NextDouble() * 5 + 1;
        }
    }

    private static DateTime GetExpiryDate(string category, Random r)
    {
        switch (category)
        {
            case "Fruits":
            case "Vegetables":
                return DateTime.Today.AddDays(r.Next(3, 15));
            case "Bakery":
                return DateTime.Today.AddDays(r.Next(2, 8));
            case "Meat":
            case "Seafood":
                return DateTime.Today.AddDays(r.Next(1, 10));
            case "Dairy":
                return DateTime.Today.AddDays(r.Next(7, 22));
            case "Frozen":
                return DateTime.Today.AddDays(r.Next(90, 365));
            case "Pantry":
            case "Personal Care":
            case "Cleaning Supplies":
            case "Baby Products":
            case "Pet Supplies":
            case "Health & Wellness":
                return DateTime.Today.AddDays(r.Next(365, 3650));
            case "Beverages":
                return DateTime.Today.AddDays(r.Next(30, 365));
            case "Snacks":
                return DateTime.Today.AddDays(r.Next(90, 540));
            default:
                return DateTime.Today.AddDays(r.Next(30, 365));
        }
    }

    private static string GenerateNutritionDetails(string name, string category, Random r)
    {
        int calories, protein, fat, carbs;

        switch (category)
        {
            case "Fruits":
                calories = r.Next(30, 90);
                protein = r.Next(0, 2);
                fat = r.Next(0, 1);
                carbs = r.Next(6, 25);
                break;
            case "Vegetables":
                calories = r.Next(10, 60);
                protein = r.Next(0, 4);
                fat = r.Next(0, 2);
                carbs = r.Next(2, 15);
                break;
            case "Meat":
                calories = r.Next(120, 300);
                protein = r.Next(20, 30);
                fat = r.Next(5, 25);
                carbs = 0;
                break;
            case "Seafood":
                calories = r.Next(70, 220);
                protein = r.Next(15, 30);
                fat = r.Next(1, 18);
                carbs = 0;
                break;
            case "Dairy":
                calories = r.Next(40, 250);
                protein = r.Next(3, 10);
                fat = r.Next(1, 20);
                carbs = r.Next(0, 12);
                break;
            case "Bakery":
            case "Snacks":
                calories = r.Next(100, 500);
                protein = r.Next(1, 8);
                fat = r.Next(3, 30);
                carbs = r.Next(10, 70);
                break;
            default:
                calories = r.Next(50, 300);
                protein = r.Next(0, 10);
                fat = r.Next(0, 20);
                carbs = r.Next(0, 60);
                break;
        }

        return $"Per 100g: {calories} kcal, Protein {protein}g, Fat {fat}g, Carbs {carbs}g";
    }

    private static string GenerateDescription(string name, string category)
    {
        switch (category)
        {
            case "Fruits": return $"Fresh {name}, ideal for snacking and smoothies.";
            case "Vegetables": return $"Fresh {name}, perfect for salads and cooking.";
            case "Bakery": return $"Baked daily {name}, enjoy while warm.";
            case "Dairy": return $"{name} sourced from trusted dairies, great for everyday use.";
            case "Meat": return $"{name} — high-quality cut, suitable for grilling and roasting.";
            case "Seafood": return $"Fresh {name}, recommended to consume within a few days after purchase.";
            case "Frozen": return $"Frozen {name}, keep in freezer for long-term storage.";
            case "Beverages": return $"{name} — refreshing and made for daily consumption.";
            case "Snacks": return $"{name} — tasty snack, great for parties and on-the-go.";
            default: return $"{name} in category {category}.";
        }
    }
}