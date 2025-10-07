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
                    var product = AnsiConsole.Prompt(
                        new SelectionPrompt<ProductModel>()
                            .Title("Select a product")
                            .PageSize(10)
                            .MoreChoicesText("[grey](Move up and down to select)[/]")
                            .AddChoices(productList));
                    return product;
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