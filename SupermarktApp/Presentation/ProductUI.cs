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
                "Show on map",
                "Go back"
            })
        );

        switch (options)
        {
            case "Show on map":
                MapUI.DisplayMap(product.Location);
                break;
            case "Go back":
                break;

        }
    }
}