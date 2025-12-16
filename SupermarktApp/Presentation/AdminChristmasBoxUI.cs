using Spectre.Console;
using System.Linq;

public static class AdminChristmasBoxUI
{
    public static void Show()
    {
        Console.Clear();
        Utils.PrintTitle("Christmas Box Admin");

        var products = ProductLogic.GetAllProducts();

        var choices = products
            .Select(p =>
                $"{p.ID} - {Markup.Escape(p.Name)} {Markup.Escape(p.Category)}" // markup escape to prevent rendering errors [ ] 
            )
            .ToList();

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[white]Select a product to toggle Christmas box eligibility[/]")
                .AddChoices(choices)
        );

        int productId = int.Parse(selected.Split('-')[0].Trim());
        var product = products.First(p => p.ID == productId);

        if (product.Category == "ChristmasBoxItem")
        {
            product.Category = "Regular";
        }
        else
        {
            product.Category = "ChristmasBoxItem";
        }

        ProductLogic.UpdateProduct(product);

        AnsiConsole.MarkupLine("[green]Product updated successfully![/]");
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
        Console.ReadKey();
    }
}
