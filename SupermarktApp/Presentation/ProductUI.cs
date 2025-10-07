using Spectre.Console;

public class ProductUI
{
    public static void SearchProduct()
    {
        var product = ProductLogic.SearchProductByNameOrCategory();
        var options = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .AddChoices(new[]{
                "Add to basket",
                "Show on map",
                "Show product details",
                "Go back"
            })
        );

        switch (options)
        {
            case "Add to basket":
                break;
            case "Show on map":
                MapUI.DisplayMap(product.Location);
                break;
            case "Show product details":
                ProductDetailsUI.ShowProductDetails(product);
                break;
            case "Go back":
                break;
        }
    }
}