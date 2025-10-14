using Spectre.Console;

public class ProductLogic
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

            var table = new Table();
            table.AddColumn("Name");
            table.AddColumn("Price");

            if (input.Length != 0)
            {
                List<ProductModel> productList = ProductAccess.SearchProductByName(input);
                AnsiConsole.MarkupLine("[blue]You can find products by name or category.[/]");
                if (productList.Count == 0)
                {
                    Console.WriteLine("No products found.");
                    continue;
                }

                //Vul de table met Namen en prijzen.
                foreach (ProductModel product in productList)
                {
                    table.AddRow(product.Name, product.Price.ToString());
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

                    return ProductAccess.GetProductByName(product);
                }
            }
        }
        return null!;
    }

    public static void ChangeProductDetails(int id, string name, double price, string nutritionDetails, string description, string category, int location, int quantity)
    {
        ProductModel NewProduct = new ProductModel(id, name, price, nutritionDetails, description, category, location, quantity);
        ProductAccess.ChangeProductDetails(NewProduct);
        return;
    }

    public static void DeleteProductByID(int id)
    {
        ProductAccess.DeleteProductByID(id);
    }

    public static bool AddProduct(string name, double price, string nutritionDetails, string description, string category, int location, int quantity)
    {
        ProductModel NewProduct = new ProductModel(name, price, nutritionDetails, description, category, location, quantity);
        ProductModel? ExistingProductCheck = ProductAccess.GetProductByName(NewProduct.Name);
        if (ExistingProductCheck == null)
        {
            ProductAccess.AddProduct(NewProduct);
            return true;
        }
        else
            return false;
    }
}