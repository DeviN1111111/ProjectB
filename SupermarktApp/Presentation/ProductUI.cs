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
                while(true)
                {
                    Console.Clear();
                    Console.WriteLine("Enter quantity to add to cart (max 20): ");
                    if (!int.TryParse(Console.ReadLine(), out int quantity))
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                        Thread.Sleep(2000);
                        continue;

                    }
                    else
                    {
                        if (quantity > 20 && quantity > product.Quantity)
                        {
                            if (product.Quantity < 20)
                            {
                                Console.WriteLine($"Only {product.Quantity} items in stock. Please enter a valid quantity.");
                                Thread.Sleep(2000);
                                continue;
                            }
                            else
                            {
                                Console.WriteLine($"Only {20} items in stock. Please enter a valid quantity.");
                                Thread.Sleep(2000);
                                continue;
                            }
                        }
                        else if (quantity <= 0)
                        {
                            Console.WriteLine("Quantity must be at least 1. Please enter a valid quantity.");
                            Thread.Sleep(2000);
                            continue;
                        } 
                    }
                    OrderLogic.AddToCart(product, quantity);
                    break;
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