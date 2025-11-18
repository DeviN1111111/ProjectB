using Spectre.Console;
using System;
using System.Globalization;
public static class ManagementUI
{
    public static readonly Color Text = Color.FromHex("#E8F1F2");
    public static readonly Color Hover = Color.FromHex("#006494");
    public static readonly Color Confirm = Color.FromHex("#13293D");
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static readonly Color AsciiSecondary = Color.FromHex("#1B98E0");
    public static void DisplayMenu()
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Product Management")
                .Centered()
                .Color(AsciiPrimary));

        var period = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { "Edit product details", "Add new product", "Delete product", "Edit Shop Description", "Edit Opening Hours","Create Coupon", "Edit Coupons", "Add discount on a specific date", "Delete discount on a specific date", "Go back" }));

        switch (period)
        {
            case "Go back":
                return;

            case "Add new product":
                AddProduct();
                break;

            case "Delete product":
                DeleteProduct();
                break;

            case "Edit product details":
                ChangeProductDetails();
                break;

            case "Edit Shop Description":
                ShopDetailsUI.PromptDescription();
                break;

            case "Edit Opening Hours":
                ShopDetailsUI.PromptOpeningHours();
                break;
            case "Add discount on a specific date":
                DiscountSpecificDate();
                break;
            case "Delete discount on a specific date":
                DeleteDiscountSpecificDate();
                break;

            
            case "Create Coupon":
                CreateCouponForUser();
                break;

            case "Edit Coupons":
                EditCoupons();
                break;
                
            default:
                AnsiConsole.MarkupLine("[red]Invalid selection[/]");
                break;
        }
    }
    public static void CreateCouponForUser()
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Create Coupon")
                .Centered()
                .Color(AsciiPrimary));

        var email = AnsiConsole.Prompt(new TextPrompt<string>("Enter user email:"));
        var user = LoginLogic.GetUserByEmail(email);
        if (user == null)
        {
            AnsiConsole.MarkupLine("[red]User not found.[/]");
            AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
            Console.ReadKey();
            return;
        }

        var credit = AnsiConsole.Prompt(new TextPrompt<double>("Enter coupon credit:"));
        CouponLogic.CreateCoupon(user.ID, credit);
        AnsiConsole.MarkupLine("[green]Coupon created successfully.[/]");
        AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
        Console.ReadKey();
    }
    public static void EditCoupons()
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Edit Coupon")
                .Centered()
                .Color(AsciiPrimary));
        var id = AnsiConsole.Prompt(new TextPrompt<int>("Enter coupon id:"));
        var coupon = CouponAccess.GetCouponById(id);
        if (coupon == null)
        {
            AnsiConsole.MarkupLine("[red]Coupon not found.[/]");
            AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
            Console.ReadKey();
            return;
        }
        var newCredit = AnsiConsole.Prompt(new TextPrompt<double>("New credit:").DefaultValue(coupon.Credit));
        var validityChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Set validity:")
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { "Valid", "Invalid" }));
        var newIsValid = validityChoice == "Valid";
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Confirm Changes")
                .Centered()
                .Color(AsciiPrimary));
        AnsiConsole.MarkupLine($"Id: [yellow]{coupon.Id}[/]");
        AnsiConsole.MarkupLine($"UserId: [blue]{coupon.UserId}[/]");
        AnsiConsole.MarkupLine($"Credit: [red]{coupon.Credit}[/] -> [green]{newCredit}[/]");
        AnsiConsole.MarkupLine($"IsValid: [red]{coupon.IsValid}[/] -> [green]{newIsValid}[/]");
        var confirm = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { "Confirm", "Cancel" }));
        if (confirm == "Confirm")
        {
            coupon.Credit = newCredit;
            coupon.IsValid = newIsValid;
            CouponLogic.EditCoupon(coupon);
            AnsiConsole.MarkupLine("[green]Coupon updated successfully.[/]");
            AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
            Console.ReadKey();
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
        AnsiConsole.Write(
            new FigletText("Change Product Details")
                .Centered()
                .Color(AsciiPrimary));

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
        var quantity = AnsiConsole.Prompt(new TextPrompt<int>("New quantity of product:").DefaultValue(EditProduct.Quantity));
        var visible = AnsiConsole.Prompt(new TextPrompt<int>("New visibility of product (1 = visible, 0 = hidden):").DefaultValue(EditProduct.Visible));

        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Compare Product Changes")
                .Centered()
                .Color(AsciiPrimary));

        ProductDetailsUI.CompareTwoProducts(EditProduct, new ProductModel(name, price, nutritionDetails, description, category, location, quantity, visible));
        AnsiConsole.MarkupLine($"Are you sure you want to save changes to [red]{EditProduct.Name}[/]?");

        var confirm = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { "Confirm", "Cancel" }));

        switch (confirm)
        {
            case "Confirm":
                AnsiConsole.Write(
                new FigletText("Edit Product")
                    .Centered()
                    .Color(AsciiPrimary));
                ProductLogic.ChangeProductDetails(EditProduct.ID, name, price, nutritionDetails, description, category, location, quantity, visible);
                break;
            case "Cancel":
                return;
        }
    }
    public static void AddProduct()
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Add Product")
                .Centered()
                .Color(AsciiPrimary));

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
        var quantity = AnsiConsole.Prompt(new TextPrompt<int>("New quantity of product:"));
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
        AnsiConsole.Write(
            new FigletText("Delete Product")
                .Centered()
                .Color(AsciiPrimary));

        AnsiConsole.MarkupLine($"Are you sure you want to delete [red]{EditProduct.Name}[/]? This action cannot be undone.");
        var confirm = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { "Confirm", "Cancel" }));
        switch (confirm)
        {
            case "Confirm":
                ProductLogic.DeleteProductByID(EditProduct.ID);
                Console.Clear();
                break;
            case "Cancel":
                return;
        }
    }
    public static void DiscountSpecificDate()
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Add discount on a specific date")
                .Centered()
                .Color(AsciiPrimary));

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
        AnsiConsole.Write(
            new FigletText("Choose week number")
                .Centered()
                .Color(AsciiPrimary));
        
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

            AnsiConsole.Write(
            new FigletText("Choose week number")
                .Centered()
                .Color(AsciiPrimary));

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
        AnsiConsole.Write(
            new FigletText("Delete discount on a specific date")
                .Centered()
                .Color(AsciiPrimary));
        
        List<DiscountsModel> AllDiscounts = DiscountsLogic.GetAllWeeklyDiscounts();
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
}
