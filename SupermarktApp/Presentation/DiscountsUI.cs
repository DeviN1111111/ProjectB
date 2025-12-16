using Spectre.Console;

public class DiscountsUI
{
    public static void DisplayMenu()
    {
        while (true)
        {
            Console.Clear();
            Utils.PrintTitle("Discounts");

            var Choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .HighlightStyle(new Style(ColorUI.Hover))
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
            Table table = Utils.CreateTable(new [] {
                "[blue]Name[/]", 
                "[italic yellow]Discount Percentage[/]", 
                "[red]Original Price[/]", 
                "[green]Discounted Price[/]"});
                
            foreach (DiscountsModel discount in discounts)
            {
                var product = ProductLogic.GetProductById(discount.ProductID);
                table.AddRow($"[blue]{product.Name}[/]", $"[italic yellow]{discount.DiscountPercentage}% OFF[/]", $"[strike][red]€{product.Price}[/][/]", $"[green]{Utils.CalculateDiscountedPrice(product.Price, discount.DiscountPercentage)}[/]");
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
            Table table = Utils.CreateTable(new [] {
                "[blue]Name[/]", 
                "[italic yellow]Discount Percentage[/]", 
                "[red]Original Price[/]", 
                "[green]Discounted Price[/]"});

            foreach (var discount in discounts)
            {
                var product = ProductLogic.GetProductById(discount.ProductID);
                table.AddRow($"[blue]{product.Name}[/]", $"[italic yellow]{discount.DiscountPercentage}% OFF[/]", $"[strike][red]€{product.Price}[/][/]", $"[green]{Utils.CalculateDiscountedPrice(product.Price, discount.DiscountPercentage)}[/]");
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
            Table table = Utils.CreateTable([
                "[blue]Name[/]", 
                "[italic yellow]Discount Percentage[/]", 
                "[red]Original Price[/]", 
                "[green]Discounted Price[/]"]);

            foreach (DiscountsModel discount in discounts)
            {
                var product = ProductLogic.GetProductById(discount.ProductID);
                ProductDiscountDTO discountedProduct = DiscountsLogic.CheckDiscountByProduct(product)!;
                if(discountedProduct != null && discountedProduct.Discount!.DiscountType == "Expiry") // Maybe make it page system so page doesnt flood with too many discounts
                {
                    table.AddRow($"[blue]{product.Name}[/]", $"[italic yellow]{discount.DiscountPercentage}% OFF[/]", $"[strike][red]€{product.Price}[/][/]", $"[green]{Utils.CalculateDiscountedPrice(product.Price, discount.DiscountPercentage)}[/]");
                }      
            }
            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("Press [green]any key[/] to continue.");
            Console.ReadKey();
        }
    }
}