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
                int quantity = AnsiConsole.Prompt(new TextPrompt<int>("Enter amount:"));
                AnsiConsole.MarkupLine($"Added [blue]{quantity}[/] [green]{product.Name}[/] to cart, press [green]ENTER[/] to continue");
                Console.ReadKey();
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