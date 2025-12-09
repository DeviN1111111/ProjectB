using Spectre.Console;

public class NotificationUI
{
    public static void DisplayMenu()
    {
        while (true)
        {
            Console.Clear();
            Utils.PrintTitle("Notifications");

            var prompt1 = new SelectionPrompt<string>()
                .AddChoices(new[] { "Fill specific low stock product", "Go back" });
            string selectedItem = AnsiConsole.Prompt(prompt1);

            switch (selectedItem)
            {
                case "Go back":
                    return;
                case "Fill specific low stock product":
                    FillSpecificLowStockProduct();
                    return;
            }
        }
    }

    public static double FillSpecificLowStockProduct()
    {
        Console.Clear();
        Utils.PrintTitle("Fill Products");

        int QuantityThreshold = AnsiConsole.Prompt(new TextPrompt<int>("Enter the quantity threshold for low stock notifications:").DefaultValue(50));
        List<ProductModel> AllLowQuantityProducts = NotificationLogic.GetAllLowQuantityProducts(QuantityThreshold);

        if (AllLowQuantityProducts.Count == 0)
        {
            AnsiConsole.MarkupLine("[green]No products found with low stock.[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
            return 0;
        }

        List<string> LowQuantityProductNames = [];

        foreach (var product in AllLowQuantityProducts)
        {
            LowQuantityProductNames.Add($"ProductID: {product.ID } Name: [yellow]{product.Name}[/] Quantity: [red]{product.Quantity}[/]");
        }

        var itemsToFill = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("[bold white]Select items to fill the quantity:[/]")
                .NotRequired()
                .PageSize(20)
                // .AddChoices(LowQuantityProductNames)
                .AddChoiceGroup("Select all", LowQuantityProductNames)
        );

        if (itemsToFill.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No items selected.[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
            return 0;
        }
        Console.Clear();
        Utils.PrintTitle("Fill Products");

        double price = 0;
        int QuantityFill;
        do
        {
            QuantityFill = AnsiConsole.Prompt(new TextPrompt<int>("Enter the quantity you want to add to all the selected products:").DefaultValue(100));
        } while (ValidaterLogic.ValidateQuantityProduct(QuantityFill) == false);

        foreach (var product in itemsToFill)
        {
            var productID = int.Parse(product.Split(" ")[1]);
            var cost = NotificationLogic.FillProductQuantity(productID, QuantityFill);

            price += cost;
            NotificationLogic.AddRestockEntry(new RestockHistoryModel(productID, QuantityFill, DateTime.Now, cost / QuantityFill));

        }
        AnsiConsole.MarkupLine($"[green]Successfully filled the selected products. Total cost: [yellow]€{Math.Round(price, 2)}[/].[/]");
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
        Console.ReadKey();
        return price;
    }
}