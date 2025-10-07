using Spectre.Console;

public class ProductLogic
{
    public static ProductModel SearchProductByName()
    {
        string input = "";
        while (true)
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Escape)
                break;

            if (!char.IsControl(key.KeyChar))
            {
                input += key.KeyChar;
            }
            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input.Remove(input.Length - 1);
            }

            Console.Clear();
            if (input.Length == 0)
            {
                Console.WriteLine("Search: ");
            }
            else
            {
                Console.WriteLine($"Search: {input}");
            }

            // var basket = AnsiConsole.Prompt(
            //     new SelectionPrompt<string>()
            //         .Title("Basket")
            //         .PageSize(10)
            //         .MoreChoicesText("[grey](Move up and down to select)[/]")
            //         .AddChoices(new[] { $"" }));

            if (input.Length != 0)
            {
                List<ProductModel> productList = ProductAccess.SearchProductByName(input);
                if (productList.Count == 0)
                {
                    Console.WriteLine("No products found.");
                    continue;
                }

                foreach (ProductModel product in productList)
                {
                    Console.WriteLine(product.Name);
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    return productList[0];
                }
            }
        }
        return null!;
    }

    public static List<ProductModel> SearchProductByCategory()
    {
        string input = "";
        while (true)
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Escape)
                break;

            if (!char.IsControl(key.KeyChar))
            {
                input += key.KeyChar;
            }
            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input.Remove(input.Length - 1);
            }

            Console.Clear();
            if (input.Length == 0)
            {
                Console.WriteLine("Search: ");
            }
            else
            {
                Console.WriteLine($"Search: {input}");
            }

            if (input.Length != 0)
            {
                List<ProductModel> productList = ProductAccess.SearchProductByCategory(input);
                if (productList.Count == 0)
                {
                    Console.WriteLine("No categories found.");
                    continue;
                }

                foreach (ProductModel product in productList.DistinctBy(productmodel => productmodel.Category))
                {
                    Console.WriteLine(product.Category);
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    foreach (ProductModel product in productList)
                    {
                        Console.WriteLine(product.Name);
                    }
                    return productList;
                }
            }
        }
        return null!;
    }
}