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
            Utils.PrintTitle("Discounts");

            var Choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .HighlightStyle(new Style(Hover))
                    .AddChoices(new[] { "Weekly Discounts", "Personal Discounts", "Expiry Date Discounts", "Go back" }));

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
                case "Expiry Date Discounts":
                    DisplayExpiryDateDiscounts();
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
        Utils.PrintTitle("Weekly Discounted Products");

        var discounts = DiscountsLogic.GetWeeklyDiscounts();
        if (discounts.Count == 0)
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
            foreach (DiscountsModel discount in discounts)
            {
                var product = ProductLogic.GetProductById(discount.ProductID);
                table.AddRow($"[blue]{product.Name}[/]", $"[italic yellow]{discount.DiscountPercentage}% OFF[/]", $"[strike][red]€{product.Price}[/][/]", $"[green]€{Math.Round(product.Price * (1 - (discount.DiscountPercentage / 100)), 2)}[/]");
            }
            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
            Console.ReadKey();
        }
    }

    public static void DisplayPersonalDiscounts()
    {
        Console.Clear();
        Utils.PrintTitle("Personal Discounts");

        DiscountsLogic.SeedPersonalDiscounts(SessionManager.CurrentUser!.ID); // clear previous discounts and seed again based on order history
        var discounts = DiscountsLogic.GetValidPersonalDiscounts(SessionManager.CurrentUser!.ID); // retrieve the (new) personal discounts

        if (discounts.Count < 5)
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
            foreach (var discount in discounts)
            {
                var product = ProductLogic.GetProductById(discount.ProductID);
                table.AddRow($"[blue]{product.Name}[/]", $"[italic yellow]{discount.DiscountPercentage}% OFF[/]", $"[strike][red]€{product.Price}[/][/]", $"[green]€{Math.Round(product.Price * (1 - (discount.DiscountPercentage / 100)), 2)}[/]");
            }
            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
            Console.ReadKey();
            discounts.Clear();
        }
    }

    public static void DisplayExpiryDateDiscounts()
    {
        Console.Clear();
        Utils.PrintTitle("Expiry Date Discounts");

        var discounts = DiscountsLogic.GetAllExpiryDiscounts();
        if (discounts.Count == 0)
        {
            AnsiConsole.MarkupLine("[bold red] NO EXPIRY DATE DISCOUNTS[/]");
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
            foreach (DiscountsModel discount in discounts)
            {
                var product = ProductLogic.GetProductById(discount.ProductID);
                ProductDiscountDTO discountedProduct = DiscountsLogic.CheckDiscountByProduct(product);
                if(discountedProduct != null && discountedProduct.Discount.DiscountType == "Expiry") // Maybe make it page system so page doesnt flood with too many discounts
                {
                    table.AddRow($"[blue]{discountedProduct.Product.Name}[/]", $"[italic yellow]{discountedProduct.Discount.DiscountPercentage}% OFF[/]", $"[strike][red]€{discountedProduct.Product.Price}[/][/]", $"[green]€{Math.Round(discountedProduct.Product.Price * (1 - (discountedProduct.Discount.DiscountPercentage / 100)), 2)}[/]");
                }      
            }
            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
            Console.ReadKey();
        }
    }
}