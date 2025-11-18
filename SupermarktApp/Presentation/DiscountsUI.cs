using Spectre.Console;

public class DiscountsUI
{
    public static readonly Color Text = Color.FromHex("#E8F1F2");
    public static readonly Color Hover = Color.FromHex("#006494");
    public static readonly Color Confirm = Color.FromHex("#13293D");
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static readonly Color AsciiSecondary = Color.FromHex("#1B98E0");
    public static void DisplayMenu()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("Discounts")
                    .Centered()
                    .Color(AsciiPrimary));

            var Choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .HighlightStyle(new Style(Hover))
                    .AddChoices(new[] { "Weekly Discounts", "Personal Discounts", "Go back" }));

            switch (Choice)
            {
                case "Go back":
                    return;

                case "Weekly Discounts":
                    DisplayWeeklyDiscounts();
                    break;

                case "Personal Discounts":
                    DisplayPersonalDiscounts();
                    break;

                default:
                    AnsiConsole.MarkupLine("[red]Invalid selection[/]");
                    break;
            }
        }
    }

    public static void DisplayWeeklyDiscounts()
    {
        Console.Clear();
        AnsiConsole.Write(
        new FigletText("Weekly Discounted Products")
            .Centered()
            .Color(AsciiPrimary));

        var products = DiscountsLogic.GetWeeklyDiscounts();
        if (products.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red] NO WEEKLY DISCOUNTS[/]");
            AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
            Console.ReadKey();
        }
        else
        {
            var table = new Table();
            table.AddColumn("[blue]Name[/]");
            table.AddColumn("[italic yellow]Discount Percentage[/]");
            table.AddColumn("[red]Original Price[/]");
            table.AddColumn("[green]Discounted Price[/]");
            foreach (DiscountsModel discountModel in products)
            {
                ProductModel productModel = ProductLogic.GetProductByID(discountModel.ProductID);
                table.AddRow($"[blue]{productModel.Name}[/]", $"[italic yellow]{discountModel.DiscountPercentage}% OFF[/]", $"[strike][red]€{productModel.Price}[/][/]", $"[green]€{Math.Round(productModel.Price * (1 - (discountModel.DiscountPercentage / 100)), 2)}[/]");
            }
            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
            Console.ReadKey();
        }
    }

    public static void DisplayPersonalDiscounts()
    {
        Console.Clear();
        AnsiConsole.Write(
        new FigletText("Personal Discounted Products")
            .Centered()
            .Color(AsciiPrimary));

        DiscountsLogic.SeedPersonalDiscounts(SessionManager.CurrentUser!.ID); // clear previous discounts and seed again based on order history
        var products = DiscountsLogic.GetPersonalDiscountsProducts(SessionManager.CurrentUser!.ID); // retrieve the (new) personal discounts

        if (products.Count < 5)
        {
            AnsiConsole.MarkupLine("[bold red] NO PERSONAL DISCOUNTS [italic yellow](Place some orders to get personal discounts!)[/][/]");
            AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
            Console.ReadKey();
        }
        else
        {
            var table = new Table();
            table.AddColumn("[blue]Name[/]");
            table.AddColumn("[italic yellow]Discount Percentage[/]");
            table.AddColumn("[red]Original Price[/]");
            table.AddColumn("[green]Discounted Price[/]");
            foreach (ProductModel product in products)
            {
                table.AddRow($"[blue]{product.Name}[/]", $"[italic yellow]{product.DiscountPercentage}% OFF[/]", $"[strike][red]€{product.Price}[/][/]", $"[green]€{Math.Round(product.Price * (1 - (product.DiscountPercentage / 100)), 2)}[/]");
            }
            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
            Console.ReadKey();
            products.Clear();
        }
    }
}