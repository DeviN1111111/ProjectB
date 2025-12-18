using Spectre.Console;

public class ProductUI
{
    private static int AskForQuantity(ProductModel product)
    {
        while(true)
        {
            AnsiConsole.Markup($"[white]Enter quantity (1–{Math.Min(99, product.Quantity)}): [/]");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int quantity))
            {
                AnsiConsole.MarkupLine("[red]Invalid input![/] Please enter a number.");
                continue;
            }

            // user can not add more than 1 to the cart 
            if (product.Category == "ChristmasBox" && quantity != 1)
            {
                AnsiConsole.MarkupLine(
                    "[yellow]You can only buy one Christmas box per size.[/]"
                );
                continue;
            }

            if (quantity <= 0)
            {
                AnsiConsole.MarkupLine("[red]Quantity must be at least 1.[/]");
                continue;
            }

            if (quantity > Math.Min(99, product.Quantity))
            {
                AnsiConsole.MarkupLine($"[red]Max allowed: {Math.Min(99, product.Quantity)}[/]");
                continue;
            }

            return quantity;
        }
    }
    public static void SearchProduct(string mode = "default")
    {
        var product = SearchUI.SearchProductByNameOrCategory();

        if (product == null)
        {
            return;
        }
        ProductDetailsUI.ShowProductDetails(product);

        if (mode == "checklist")
        {
            int quantity = AskForQuantity(product);
            ChecklistLogic.AddToChecklist(product, quantity);
            AnsiConsole.MarkupLine($"[green] {quantity}x {product.Name} added to your checklist![/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue.");
            Console.ReadKey();
            return;
        }

        var options = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .AddChoices(new[]{
                "Add to Cart",
                "Add to checklist",
                "Show on map",
                "Go back"
            })
        );

        switch (options)
        {
            case "Add to Cart":
                {
                    int quantity = AskForQuantity(product);
                    OrderLogic.AddToCartProduct(product, quantity);
                    AnsiConsole.MarkupLine($"[green]{quantity}x {product.Name} added to CartProduct![/]");
                    break;
                }

            case "Add to checklist":
                {
                    int quantity = AskForQuantity(product);
                    ChecklistLogic.AddToChecklist(product, quantity);
                    AnsiConsole.MarkupLine($"[green]{quantity}x {product.Name} added to checklist![/]");
                    break;
                }
            case "Show on map":
                MapUI.DisplayMap(product.Location);
                break;
            case "Go back":
                break;

        }
    }
}