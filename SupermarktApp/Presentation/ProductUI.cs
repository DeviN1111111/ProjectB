using Spectre.Console;

public class ProductUI
{
    public static void SearchProduct()
    {
        var product = ProductLogic.SearchProductByNameOrCategory();

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