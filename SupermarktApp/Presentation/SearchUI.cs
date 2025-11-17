using Spectre.Console;

public static class SearchUI
{
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static ProductModel SearchProductByNameOrCategory()
    {
        Console.Clear();
        AnsiConsole.Write(
        new FigletText("Product Search")
            .Centered()
            .Color(AsciiPrimary));
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

            AnsiConsole.Write(
            new FigletText("Product Search")
                .Centered()
                .Color(AsciiPrimary));

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
            var table = new Table();
            table.AddColumn("Name");
            table.AddColumn("Price");

            if (input.Length != 0)
            {
                List<ProductModel> productList = ProductLogic.SearchProductByName(input);
                AnsiConsole.MarkupLine("[blue]You can find products by name or category.[/]");
                if (productList.Count == 0)
                {
                    Console.WriteLine("No products found.");
                    continue;
                }

                //Vul de table met Namen en prijzen.
                foreach (ProductModel product in productList)
                {
                    if(product == null)
                    {
                        break;
                    }
                    var PersonalDiscount = DiscountsLogic.GetPeronsalDiscountByProductAndUserID(product.ID, SessionManager.CurrentUser!.ID);
                    var WeeklyDiscount = DiscountsLogic.GetWeeklyDiscountByProductID(product.ID);
                    
                    if(PersonalDiscount.DiscountType == "Personal" && DiscountsLogic.CheckUserIDForPersonalDiscount(product.ID))
                    {
                        string text = product.Price.ToString();
                        var struckPrice = $"[strike][red]€{text}[/][/]";

                        string newPrice = $"{struckPrice} [green]€{Math.Round(product.Price * (1 - (PersonalDiscount.DiscountPercentage / 100)), 2)}[/]";
                        table.AddRow(product.Name, newPrice);            
                    }
                    else if(WeeklyDiscount.DiscountType == "Weekly")
                    {
                        string text = product.Price.ToString();
                        var struckPrice = $"[strike][red]€{text}[/][/]";

                        string newPrice = $"{struckPrice} [green]€{Math.Round(product.Price * (1 - (WeeklyDiscount.DiscountPercentage / 100)), 2)}[/]";
                        table.AddRow(product.Name, newPrice);
                    }
                    else
                    {
                        string yellowPrice = $"[yellow]€{product.Price.ToString()}[/]";
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