using Spectre.Console;
using System;
using System.Globalization;

public static class ManagementUI
{
    public static void DisplayMenu()
    {
        while (true)
        {
            Console.Clear();
            Utils.PrintTitle("Management");

            var items = new List<string>
            {
                "Products",
                "Shop Settings",
                "Discounts",
                "Coupons",
                "Reviews",
                "Go back"
            };

            if (SessionManager.CurrentUser?.AccountStatus == "SuperAdmin")
            {
                items.Insert(0, "Manage Users");
            }
                
            var mainChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Choose a category:[/]")
                    .HighlightStyle(new Style(ColorUI.Hover))
                    .AddChoices(items));

  
            switch (mainChoice)
            {
                case "Products":
                    ShowProductMenu();
                    break;
                case "Shop Settings":
                    ShowShopSettingsMenu();
                    break;
                case "Manage Users":
                    ManageAdminUI.DisplayMenu();
                    break;
                case "Discounts":
                    ShowDiscountMenu();
                    break;
                case "Coupons":
                    ShowCouponMenu();
                    break;
                case "Reviews":
                    ShowReviewMenu();
                    break;
                case "Go back":
                    return;
            }
        }
    }
    static void ShowProductMenu()
    {
        var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]Product Options[/]")
            .AddChoices(
                "Add new product",
                "Delete product",
                "Edit product details",
                "Back"));

        switch (choice)
        {
            case "Add new product":
                AddProduct();
                break;

            case "Delete product":
                DeleteProduct();
                break;

            case "Edit product details":
                ChangeProductDetails();
                break;

            case "Back":
                return;
        }
    }

    static void ShowShopSettingsMenu()
    {
        var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]Shop Settings[/]")
            .AddChoices(
                "Edit Shop Description",
                "Edit Opening Hours",
                "Back"));

        switch (choice)
        {
            case "Edit Shop Description":
                ShopDetailsUI.PromptDescription();
                break;

            case "Edit Opening Hours":
                ShopDetailsUI.PromptOpeningHours();
                break;

            case "Back":
                return;
        }
    }

    static void ShowDiscountMenu()
    {
        var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]Discount Options[/]")
            .AddChoices(
                "Edit Weekly Discounts",
                "Edit Expiry Date Discounts",
                "Back"));

        switch (choice)
        {
            case "Edit Weekly Discounts":
                ShowWeeklyDiscountMenu();
                break;

            case "Edit Expiry Date Discounts":
                ShowExpiryDateDiscountMenu();
                break;

            case "Back":
                return;
        }
    }

    static void ShowExpiryDateDiscountMenu()
    {
        Console.Clear();

        Utils.PrintTitle("Expiry Date Discounts");

        var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]Expiry Date Discount Options[/]")
            .AddChoices(
                "Set up expiry date discount",
                "Back"));

        switch (choice)
        {
            case "Set up expiry date discount":
                SetUpExpiryDateDiscount();
                break;
            case "Back":
                return;
        }
    }

    static void SetUpExpiryDateDiscount()
    {
        Console.Clear();

        int days;
        double discount;

        Utils.PrintTitle("Set up Expiry Date Discounts");

        AnsiConsole.MarkupLine("[yellow]Set up the Expiry Discount values: [/]");

        while (true)
        {
            days = AnsiConsole.Prompt(new TextPrompt<int>("Enter the number of days before expiry to apply the discount:")
                .DefaultValue(3));

            if(days < 0)
            {
                AnsiConsole.MarkupLine("[red]Number of days must be 0 or higher.[/]");
                continue;
            }
            else if (days > 365)
            {
                AnsiConsole.MarkupLine("[red]Number of days must be less than or equal to 365.[/]");
                continue;
            }
            else
            {
                break;
            }
        }

        while (true)
        {
            discount = AnsiConsole.Prompt(new TextPrompt<double>("Enter discount percentage to apply (e.g. 15 for 15%):")
                .DefaultValue(50.0));

            if(discount <= 0 || discount > 100)
            {
                AnsiConsole.MarkupLine("[red]Discount percentage must be greater than 0 and at most 100.[/]");
                continue;
            }
            else
            {
                break;
            }
        }

        DiscountsLogic.AddExpiryDateDiscounts(days, discount);

        AnsiConsole.MarkupLine($"[yellow]Successfully updated expiry discounts to start {days} days before expiry with {Math.Round(discount, 2)}% discount.[/]");
        AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
        Console.ReadKey();
    }
    static void ShowWeeklyDiscountMenu()
    {
        var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]Discount Options[/]")
            .AddChoices(
                "Add discount on a specific date",
                "Delete discount on a specific date",
                "Back"));

        switch (choice)
        {
            case "Add discount on a specific date":
                DiscountSpecificDate();
                break;

            case "Delete discount on a specific date":
                DeleteDiscountSpecificDate();
                break;

            case "Back":
                return;
        }
    }

    static void ShowCouponMenu()
    {
        var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]Coupon Options[/]")
            .AddChoices(
                "Create Coupon",
                "Edit Coupons",
                "Back"));

        switch (choice)
        {
            case "Create Coupon":
                CreateCouponForUser();
                break;

            case "Edit Coupons":
                EditCoupons();
                break;

            case "Back":
                return;
        }
    }

    static void ShowReviewMenu()
    {
        var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]Review Options[/]")
            .AddChoices(
                "Delete Reviews",
                "Go Back"));

        switch (choice)
        {
            case "Delete Reviews":
                DeleteReviews();
                break;

            case "Go Back":
                return;
        }
    }

    public static void CreateCouponForUser()
    {
        while (true)
        {
            Console.Clear();
            Utils.PrintTitle("Create Coupon");

            List<UserModel> allUsers = AdminLogic.GetAllUsers();

            var choices = allUsers
                .Select(user =>
                {
                    var label = $"User [yellow]{user.Name}[/] - [blue]{user.Email}[/]";
                    return (User: user, Label: label);
                })
                .ToList();

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an user to create a coupon for")
                     .HighlightStyle(new Style(ColorUI.Hover))
                    .PageSize(10)
                    .AddChoices(choices.Select(c => c.Label).Concat(new[] { "Go back" })));

            if (selection == "Go back")
                break;

            var chosen = choices.FirstOrDefault(c => c.Label == selection);
            if (chosen.User == null)
            {
                AnsiConsole.MarkupLine("[red]User not found.[/]");
                AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
                Console.ReadKey();
                continue;
            }

            var credit = AnsiConsole.Prompt(new TextPrompt<double>("Enter coupon credit:"));
            if (credit <= 0 || credit > 500)
            {
                AnsiConsole.MarkupLine("[red]Credit must be greater than 0 and less than 500.[/]");
                AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
                Console.ReadKey();
                continue;
            }

            CouponLogic.CreateCoupon(chosen.User.ID, credit);
            AnsiConsole.MarkupLine("[green]Coupon created successfully.[/]");
            AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
            Console.ReadKey();
            return;
        }
    }
    public static void EditCoupons()
    {
        while (true)
            {
            Console.Clear();
            Utils.PrintTitle("Edit Coupons");
            
            var allUsers = AdminLogic.GetAllUsers();
            var userSelection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a user to edit their coupons")
                    .HighlightStyle(new Style(ColorUI.Hover))
                    .PageSize(10)
                    .AddChoices(allUsers.Select(u => $"[yellow]{u.Name}[/] - [blue]{u.Email}[/]").Concat(new[] { "Go back" })));
            if (userSelection == "Go back") break;

            var selectedUser = allUsers.FirstOrDefault(u => $"[yellow]{u.Name}[/] - [blue]{u.Email}[/]" == userSelection);
            if (selectedUser == null)
            {
                AnsiConsole.MarkupLine("[red]User not found.[/]");
                AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
                Console.ReadKey();
                continue;
            }

            var userCoupons = CouponLogic.GetAllCoupons(selectedUser.ID);
            if (userCoupons.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]This user has no coupons.[/]");
                AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
                Console.ReadKey();
                continue;
            }

            var couponSelection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a coupon to edit")
                    .HighlightStyle(new Style(ColorUI.Hover))
                    .PageSize(10)
                    .AddChoices(userCoupons.Select(c => $"Coupon #{c.Id} - €[green]{Math.Round(c.Credit, 2)}[/] - {(c.IsValid ? "[green]Valid[/]" : "[red]Invalid[/]")}").Concat(new[] { "Go back" })));
            if (couponSelection == "Go back") return;

            var selectedCoupon = userCoupons.FirstOrDefault(c => $"Coupon #{c.Id} - €[green]{Math.Round(c.Credit, 2)}[/] - {(c.IsValid ? "[green]Valid[/]" : "[red]Invalid[/]")}" == couponSelection);
            if (selectedCoupon == null)
            {
                AnsiConsole.MarkupLine("[red]Coupon not found.[/]");
                AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
                Console.ReadKey();
                continue;
            }

            var newCredit = AnsiConsole.Prompt(new TextPrompt<double>("New credit (greater than 0 and less than 500):").DefaultValue(selectedCoupon.Credit));
            if (newCredit <= 0 || newCredit > 500)
            {
                AnsiConsole.MarkupLine("[red]Credit must be greater than 0 and less than 500.[/]");
                AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
                Console.ReadKey();
                continue;
            }
            var validityChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Set validity:")
                    .HighlightStyle(new Style(ColorUI.Hover))
                    .AddChoices(new[] { "Valid", "Invalid" }));
            var newIsValid = validityChoice == "Valid";
            Console.Clear();
            Utils.PrintTitle("Confirm Changes");

            AnsiConsole.MarkupLine($"Id: [yellow]{selectedCoupon.Id}[/]");
            AnsiConsole.MarkupLine($"UserId: [blue]{selectedCoupon.UserId}[/]");
            AnsiConsole.MarkupLine($"Credit: [red]{selectedCoupon.Credit}[/] -> [green]{newCredit}[/]");
            AnsiConsole.MarkupLine($"IsValid: [red]{selectedCoupon.IsValid}[/] -> [green]{newIsValid}[/]");
            var confirm = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .HighlightStyle(new Style(ColorUI.Hover))
                    .AddChoices(new[] { "Confirm", "Cancel" }));
            if (confirm == "Confirm")
            {
                selectedCoupon.Credit = newCredit;
                selectedCoupon.IsValid = newIsValid;
                CouponLogic.EditCoupon(selectedCoupon);
                AnsiConsole.MarkupLine("[green]Coupon updated successfully.[/]");
                AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
                Console.ReadKey();
            }
        }
    }
    public static void ChangeProductDetails()
    {
        ProductModel EditProduct = SearchUI.SearchProductByNameOrCategory();
        if (EditProduct == null)
        {
            return;
        }
        Console.Clear();
        Utils.PrintTitle("Change Product Details");

        var name = AnsiConsole.Prompt(new TextPrompt<string>("New name of product:").DefaultValue(EditProduct.Name));
        var price = AnsiConsole.Prompt(new TextPrompt<double>("New price of product:").DefaultValue(EditProduct.Price));
        var nutritionDetails = AnsiConsole.Prompt(new TextPrompt<string>("New nutritionDetails of product:").DefaultValue(EditProduct.NutritionDetails));
        var description = AnsiConsole.Prompt(new TextPrompt<string>("New description of product:").DefaultValue(EditProduct.Description));
        var category = AnsiConsole.Prompt(new TextPrompt<string>("New category of product:").DefaultValue(EditProduct.Category));
        int location;
        do
        {
            location = AnsiConsole.Prompt(new TextPrompt<int>("New location of product: (Max 43)").DefaultValue(EditProduct.Location));
        } while (ValidaterLogic.ValidateLocationProduct(location) == false);
        int quantity;
        while (true)
        {
            quantity = AnsiConsole.Prompt(new TextPrompt<int>("New quantity of product:").DefaultValue(EditProduct.Quantity));
            if (quantity < 0)
                AnsiConsole.MarkupLine("[red]Quantity cannot be negative.[/]");
            else
                break;
        }

        var visible = AnsiConsole.Prompt(new TextPrompt<int>("New visibility of product (1 = visible, 0 = hidden):").DefaultValue(EditProduct.Visible));

        Console.Clear();
        Utils.PrintTitle("Compare Product Changes");

        ProductDetailsUI.CompareTwoProducts(EditProduct, new ProductModel(name, price, nutritionDetails, description, category, location, quantity, visible));
        AnsiConsole.MarkupLine($"Are you sure you want to save changes to [red]{EditProduct.Name}[/]?");

        var confirm = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(ColorUI.Hover))
                .AddChoices(new[] { "Confirm", "Cancel" }));

        switch (confirm)
        {
            case "Confirm":
                Console.Clear();
                Utils.PrintTitle("Change Product Details");
                ProductLogic.ChangeProductDetails(EditProduct.ID, name, price, nutritionDetails, description, category, location, quantity, visible);
                AnsiConsole.MarkupLine($"[green]Successfully changed product details from: [red]{EditProduct.Name}[/].[/]");
                AnsiConsole.MarkupLine("Press [green]Any KEY[/] to continue.");
                Console.ReadKey();
                break;
            case "Cancel":
                return;
        }
    }
    public static void AddProduct()
    {
        Console.Clear();
        Utils.PrintTitle("Add Product");

        var name = AnsiConsole.Prompt(new TextPrompt<string>("New name of product:"));
        var price = AnsiConsole.Prompt(new TextPrompt<double>("New price of product:"));
        var nutritionDetails = AnsiConsole.Prompt(new TextPrompt<string>("New nutritionDetails of product:"));
        var description = AnsiConsole.Prompt(new TextPrompt<string>("New description of product:"));
        var category = AnsiConsole.Prompt(new TextPrompt<string>("New category of product:"));
        int location;
        do
        {
            location = AnsiConsole.Prompt(new TextPrompt<int>("New location of product: (Max 43)"));
        } while (ValidaterLogic.ValidateLocationProduct(location) == false);
        int quantity;
        while (true)
        {
            quantity = AnsiConsole.Prompt(new TextPrompt<int>("New quantity of product:"));
            if (quantity < 0)
                AnsiConsole.MarkupLine("[red]Quantity cannot be negative.[/]");
            else
                break;
        }

        var visible = AnsiConsole.Prompt(new TextPrompt<int>("New visibility of product (1 = visible, 0 = hidden):").DefaultValue(1));

        if (ProductLogic.AddProduct(name, price, nutritionDetails, description, category, location, quantity, visible))
        {
            ProductDetailsUI.ShowProductDetails(new ProductModel(name, price, nutritionDetails, description, category, location, quantity, visible: 1));
            AnsiConsole.MarkupLine($"[green]Succesfully added [red]{name}[/], press enter to continue.[/]");
            Console.ReadKey();
        }
        else
            AnsiConsole.MarkupLine("[red]Product already exists.[/]");
        Console.ReadKey();
    }
    public static void DeleteProduct()
    {
        ProductModel EditProduct = SearchUI.SearchProductByNameOrCategory();
        if (EditProduct == null)
        {
            return;
        }
        Console.Clear();
        Utils.PrintTitle("Delete Product");     

        AnsiConsole.MarkupLine($"Are you sure you want to delete [red]{EditProduct.Name}[/]? This action cannot be undone.");
        var confirm = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(ColorUI.Hover))
                .AddChoices(new[] { "Confirm", "Cancel" }));
        
        switch (confirm)
        {
            case "Confirm":
                Console.Clear();
                Utils.PrintTitle("Delete Product"); 
                ProductLogic.DeleteProductByID(EditProduct.ID);
                AnsiConsole.MarkupLine($"[green]Successfully deleted [red]{EditProduct.Name}[/].[/]");
                AnsiConsole.MarkupLine("Press [green]Any KEY[/] to continue.");
                Console.ReadKey();
                break;
            case "Cancel":
                return;
        }
    }
    public static void DiscountSpecificDate()
    {
        Console.Clear();
        Utils.PrintTitle("Add Discount on Specific Date");

        ProductModel Product = SearchUI.SearchProductByNameOrCategory();
        if (Product == null)
        {
            return;
        }

        double discount;
        do
        {
            discount = AnsiConsole.Prompt(new TextPrompt<double>("How much discount percentage do you want to give? (0-100)").DefaultValue(10));
        } while (discount < 0 || discount > 100);
        AnsiConsole.MarkupLine($"Enter the week number (1-53) for which you want to apply the discount for [blue]{Product.Name}[/]:");

        string week = "";
        int thisweek = ISOWeek.GetWeekOfYear(DateTime.Now);
        int intweek = 0;
        Console.Clear();
        Utils.PrintTitle("Choose week number");
        
        AnsiConsole.MarkupLine($"[blue]How many weeks from now, do you want the discount to be active?: [/]");
        AnsiConsole.MarkupLine($"Currently week: {thisweek}");
        AnsiConsole.MarkupLine($"[blue]Weeks from now: {week}[/]");

        while (true)
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Escape)
                break;
            if (char.IsDigit(key.KeyChar))
                week += key.KeyChar;
            if (key.Key == ConsoleKey.Backspace && week.Length > 0)
                week = week.Remove(week.Length - 1);

            Console.Clear();

            Utils.PrintTitle("Choose week number");

            int year = DateTime.Now.Year;
            if (week.Length != 0 && week.Length < 4)
            {
                intweek = Convert.ToInt32(week);
                intweek += thisweek; //Voor testdoeleinden, zodat je altijd een geldige week hebt.
                if (intweek > 53)
                {
                    intweek -= 53;
                    year += 1;
                }
            }

            if (week.Length == 0)
            {
                AnsiConsole.MarkupLine($"[blue]How many weeks from now, do you want the discount to be active?: [/]");
                AnsiConsole.MarkupLine($"Currently week: {thisweek}");
                AnsiConsole.MarkupLine($"[blue]Weeks from now: {week}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[blue]How many weeks from now, do you want the discount to be active?: [/]");
                AnsiConsole.MarkupLine($"Currently week: {thisweek}");
                AnsiConsole.MarkupLine($"[blue]Weeks from now: {week}[/]");
            }

            if (intweek < 1 || intweek > 53)
                continue;
            else if (week == "")
            {
                continue;
            }
            else
            {
                DateTime start = ISOWeek.ToDateTime(year, intweek, DayOfWeek.Monday);
                DateTime end = ISOWeek.ToDateTime(year, intweek, DayOfWeek.Sunday);

                Console.WriteLine();
                Console.WriteLine($"Week {intweek} of {year}");
                Console.WriteLine($"Starts: {start:dd-MM-yyyy}");
                Console.WriteLine($"Ends:   {end:dd-MM-yyyy}");
                if (key.Key == ConsoleKey.Enter)
                {
                    List<DiscountsModel> ExistingDiscounts = DiscountsLogic.GetAllWeeklyDiscounts();
                    DiscountsModel DiscountProduct = new DiscountsModel(Product.ID, discount, "Weekly", start, end);
                    foreach (DiscountsModel existing in ExistingDiscounts)
                    {
                        if (existing.ProductID == DiscountProduct.ProductID && existing.StartDate == DiscountProduct.StartDate && existing.EndDate == DiscountProduct.EndDate)
                        {
                            AnsiConsole.MarkupLine("[red]Error: A discount for this product already exists for the selected week.[/]");
                            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                            Console.ReadKey();
                            return;
                        }
                    }
                    DiscountsLogic.AddDiscount(DiscountProduct);
                    System.Console.WriteLine();
                    AnsiConsole.MarkupLine($"[green]Succesfully added a discount of [blue]{discount}%[/] to [blue]{Product.Name}[/] in year: [blue]{year}[/] for week [blue]{intweek}[/]. Press any key to continue.[/]");
                    Console.ReadKey();
                    return;
                }
            }
        }
    }

    public static void DeleteDiscountSpecificDate()
    {
        Console.Clear();
        Utils.PrintTitle("Delete Discount on Specific Date");
        
        List<DiscountsModel> AllDiscounts = DiscountsLogic.GetAllWeeklyDiscounts();
        if (AllDiscounts.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no weekly discounts to delete.[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
            return;
        }
        List<string> DiscountedProductsList = [];

        foreach (DiscountsModel discount in AllDiscounts)
        {
            ProductModel product = ProductLogic.GetProductById(discount.ProductID);
            DiscountedProductsList.Add($"{discount.ID} / {discount.StartDate:dd-MM-yyyy} to {discount.EndDate:dd-MM-yyyy} / {product.Name} / {discount.DiscountPercentage}%");
        }

        var weeksToDelete = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("[bold white]Select the weeks you want to delete:[/]")
                .NotRequired()
                .PageSize(20)
                .AddChoiceGroup("Select all", DiscountedProductsList)
        );

        if (weeksToDelete.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No weeks selected.[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
            return;
        }
        else
        {
            foreach (var week in weeksToDelete)
            {
                var weekID = week.Replace(" ", "").Split("/");
                DiscountsLogic.RemoveDiscountByID(Convert.ToInt32(weekID[0]));
            }
            AnsiConsole.MarkupLine("[green]Succesfully deleted selected discounts.[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
        }
    }

    public static void DeleteReviews()
    {
        while (true)
        {
            Console.Clear();
            Utils.PrintTitle("Delete Reviews");

            var reviewLogic = new ShopReviewLogic();
            var allReviews = reviewLogic.GetAllReviews();

            if (allReviews.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]There are no reviews to delete.[/]");
                AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                Console.ReadKey();
                return;
            }
            List<string> ReviewList = [];

            foreach (var review in allReviews)
            {
                UserModel user = UserSettingsLogic.GetUserByID(review.UserId)!;
                ReviewList.Add($"ReviewID: {review.Id} User: [yellow]{user.Name}[/] Stars: [green]{review.Stars}[/] Text: [blue]{review.Text}[/]");
            }

            var prompt = AnsiConsole.Prompt(new MultiSelectionPrompt<string>()
                .PageSize(10)
                .Title("[bold white]Select reviews to delete:[/]")
                .NotRequired()
                .AddChoices(ReviewList));
            
            if (prompt.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No reviews selected.[/]");
                AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                Console.ReadKey();
                return;
            }

            foreach (var review in prompt)
            {
                var reviewID = review.Split(" ");
                ShopReviewLogic.DeleteReviewByID(Convert.ToInt32(reviewID[1]));
            }

            AnsiConsole.MarkupLine("[green]Selected reviews have been deleted successfully.[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
            break;
        }
    }
}