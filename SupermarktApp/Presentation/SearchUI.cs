using Spectre.Console;

public static class SearchUI
{
    public static ProductModel SearchProductByNameOrCategory()
    {
        Console.Clear();
        Utils.PrintTitle("Product Search");
        AnsiConsole.MarkupLine("[blue]Search:[/]");
        AnsiConsole.MarkupLine("[blue]You can find products by name or category.[/]");
        string input = "";
        while (true)
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Escape)
                break;
            if (char.IsLetter(key.KeyChar) || key.Key == ConsoleKey.Spacebar || char.IsDigit(key.KeyChar))
                input += key.KeyChar;
            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                input = input.Remove(input.Length - 1);

            Console.Clear();

            Utils.PrintTitle("Product Search");

            if (input.Length == 0)
            {
                AnsiConsole.MarkupLine("[blue]Search: [/]");
                AnsiConsole.MarkupLine("[blue]You can find products by name or category.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[blue]Search: {input}[/]");
            }
            // Tot hier gelezen
            Table table = Utils.CreateTable(new [] { "Name", "Price" });

            if (input.Length != 0)
            {
                // List<ProductModel> productList = ProductLogic.SearchProductByName(input);
                List<ProductModel> productList = ProductLogic
                    .GetAllProducts()
                    .Where(p =>
                        p.Name.Contains(input, StringComparison.OrdinalIgnoreCase) ||
                        p.Category.Contains(input, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();

                AnsiConsole.MarkupLine("[blue]You can find products by name or category.[/]");
                if (productList.Count == 0)
                {
                    Console.WriteLine("No products found.");
                    continue;
                }

                //Vul de table met Namen en prijzen.
                foreach (ProductModel product in productList)
                {
                    ProductDiscountDTO productDiscount = DiscountsLogic.CheckDiscountByProduct(product)!;
                    if(productDiscount != null)
                    {
                        string newPrice = Utils.CalculateDiscountedPriceString(product.Price, productDiscount.Discount!.DiscountPercentage);
                        table.AddRow(product.Name, newPrice);            
                    }
                    else if (RewardLogic.GetRewardItemByProductId(product.ID) != null)
                    {
                        continue;
                    }
                    else
                    {
                        string yellowPrice = $"{Utils.ChangePriceFormat(product.Price, "yellow")}";
                        table.AddRow(product.Name, yellowPrice);
                    }
                }

                AnsiConsole.Write(table);

                //List gemaakt om de namen te krijgen uit de productList anders krijg je ProductModels
                List<string> productNames = [];
                foreach (ProductModel product in productList)
                {

                    productNames.Add(product.Name);
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    if (productList.Count == 1)
                        return productList[0];
                    if (productList.Count == 0)
                        return null!;

                    var product = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Select a product")
                            .PageSize(10)
                            .AddChoices(productNames));

                    return ProductLogic.GetProductByName(product);
                }
            }
        }
        return null!;
    }
}