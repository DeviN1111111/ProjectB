using Spectre.Console;

public class ProductUI
{
    public static void SearchProduct()
    {
        var product = SearchUI.SearchProductByNameOrCategory();

        if (product == null)
        {
            return;
        }
        ProductDetailsUI.ShowProductDetails(product);

        var options = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .AddChoices(new[]{
                "Add to basket",
                "Show on map",
                "Go back"
            })
        );

        switch (options)
        {
            case "Add to basket":
            Console.Clear();
                int quantity = AnsiConsole.Prompt(new TextPrompt<int>("How many:"));
                if (quantity > 0)
                {
                    OrderLogic.AddToCart(product, quantity);
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Can't have negative quantity.[/]");
                    Console.ReadKey();
                }
                break;
            case "Show on map":
                MapUI.DisplayMap(product.Location);
                break;
            case "Go back":
                break;

        }
    }
}