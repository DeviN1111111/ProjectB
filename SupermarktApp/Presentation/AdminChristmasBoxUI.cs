using Spectre.Console;
using System.Linq;

public static class AdminChristmasBoxUI
{
    public static void Show()
    {
        Console.Clear();
        Utils.PrintTitle("Christmas Box Admin");

        var products = ProductLogic.GetAllProducts();

        // Multi-select products
        var selectedProducts = Utils.CreateMultiSelectionPrompt(
            products,
            "[white]Select products for Christmas boxes[/]",
            p => $"{p.ID} - {p.Name} ({p.Category})"
        );

        // Add items to christmas category
        foreach (var product in selectedProducts)
        {
            product.Category = product.Category == "ChristmasBoxItem"
                ? "Regular"
                : "ChristmasBoxItem";

            ProductLogic.UpdateProduct(product);
        }

        AnsiConsole.MarkupLine("[green]Product updated successfully![/]");
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
        Console.ReadKey();
    }
}
