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
            case "Add to basket":
            Console.Clear();
                Console.WriteLine("How many? ");
                int quantity = int.Parse(Console.ReadLine()!);
                OrderLogic.AddToCart(product, quantity);
                break;
            case "Show on map":
                MapUI.DisplayMap(product.Location);
                break;
            case "Go back":
                break;

        }
    }
}