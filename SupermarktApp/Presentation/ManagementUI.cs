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
                .AddChoices(new[] { "Edit product details", "Add new product", "Delete product", "Edit Shop Description", "Edit Opening Hours", "Add discount on a specific date", "Delete discount on a specific date", "Go back" }));

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

            default:
                AnsiConsole.MarkupLine("[red]Invalid selection[/]");
                break;
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
        double discount;
        do
        {
            discount = AnsiConsole.Prompt(new TextPrompt<double>("How much discount percentage do you want to give? (0-100)").DefaultValue(10));
        } while (discount < 0 || discount > 100);
        AnsiConsole.MarkupLine($"Enter the week number (1-53) for which you want to apply the discount for [blue]{Product.Name}[/]:");

        string week = "";
        int intweek = 0;
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Choose week number")
                .Centered()
                .Color(AsciiPrimary));
        AnsiConsole.MarkupLine($"[yellow]Enter a number(1-53)[/] for which you want to apply the discount for [blue]{Product.Name}[/]:");
        AnsiConsole.MarkupLine($"[blue]Search: [/]");

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

            if (week.Length == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]Enter a number(1-53)[/] for which you want to apply the discount for [blue]{Product.Name}[/]:");
                AnsiConsole.MarkupLine($"[blue]Search: [/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[yellow]Enter a number(1-53)[/] for which you want to apply the discount for [blue]{Product.Name}[/]:");
                AnsiConsole.MarkupLine($"[blue]Search: {week}[/]");
            }

            int year = DateTime.Now.Year;
            if (week.Length != 0 && week.Length < 4)
            {
                intweek = Convert.ToInt32(week);
                intweek += 45; //Voor testdoeleinden, zodat je altijd een geldige week hebt.
                if (intweek > 53)
                {
                    intweek -= 53;
                    year += 1;
                }
            }

            if (intweek < 1 || intweek > 53)
                // System.Console.WriteLine("Please enter a valid week number (1-53).");
                continue;
            else if (week == "")
            {
                continue;
            }
            else
            {
                DateTime start = ISOWeek.ToDateTime(year, intweek, DayOfWeek.Monday);
                DateTime end = ISOWeek.ToDateTime(year, intweek, DayOfWeek.Sunday);

                System.Console.WriteLine();
                Console.WriteLine($"Week {week} of {year}");
                Console.WriteLine($"Starts: {start:dd-MM-yyyy}");
                Console.WriteLine($"Ends:   {end:dd-MM-yyyy}");
                if (key.Key == ConsoleKey.Enter)
                {
                    DiscountsModel DiscountProduct = new DiscountsModel(Product.ID, discount, "Weekly", start, end);
                    DiscountsLogic.AddDiscount(DiscountProduct);
                    break;
                }
            }
        }
        AnsiConsole.MarkupLine($"[green]Succesfully added a discount of [red]{discount}%[/] to [blue]{Product.Name}[/] for week [red]{intweek}[/]. Press any key to continue.[/]");
        Console.ReadKey();
    }

    public static void DeleteDiscountSpecificDate()
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Delete discount on a specific date")
                .Centered()
                .Color(AsciiPrimary));

        ProductModel Product = SearchUI.SearchProductByNameOrCategory();
        if (Product.DiscountType == "None")
        {
            AnsiConsole.MarkupLine($"There is no discount to delete for [blue]{Product.Name}[/].");
            AnsiConsole.MarkupLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }
        else if (Product.DiscountType == "Personal")
        {
            AnsiConsole.MarkupLine($"The discount for [blue]{Product.Name}[/] is a personal discount. Personal discounts can not be removed.");
            AnsiConsole.MarkupLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }
        else
        {
            DiscountsLogic.RemoveDiscountByProductID(Product.ID);
            AnsiConsole.MarkupLine($"[green]Succesfully removed the discount from [blue]{Product.Name}[/]. Press any key to continue.[/]");
            Console.ReadKey();
        }
    }
}