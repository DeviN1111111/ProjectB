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
                    Console.WriteLine("Enter quantity to add to cart (max 99): ");
                    if (!int.TryParse(Console.ReadLine(), out int quantity))
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                        Console.ReadKey();
                        continue;

                    }
                    else
                    {
                        if (quantity > 99 || quantity > product.Quantity)
                        {
                            if (product.Quantity < 99)
                            {
                                Console.WriteLine($"Only {product.Quantity} items in stock. Please enter a valid quantity.");
                                AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                                Console.ReadKey();
                                continue;
                            }
                            else
                            {
                                Console.WriteLine($"Max 99 items Please enter a valid quantity.");
                                AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                                Console.ReadKey();
                                continue;
                            }
                        }
                        else if (quantity <= 0)
                        {
                            Console.WriteLine("Quantity must be at least 1. Please enter a valid quantity.");
                            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                            Console.ReadKey();
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