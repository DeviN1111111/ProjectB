using Spectre.Console;
using System;
using System.Linq;

public static class ChristmasBoxUI
{
    public static void Show()
    {
        Console.Clear();
        Utils.PrintTitle("Christmas Boxes");

        var boxes = ChristmasBoxLogic.GetAvailableBoxes();
        
        if (boxes.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No Christmas boxes available at the moment.[/]");
            AnsiConsole.MarkupLine("\n[grey]Press [green]ENTER[/] to go back[/]");
            Console.ReadKey();
            return;
        }
        
        var table = Utils.CreateTable(new[]
        {
            "[white]#[/]",
            "[white]Box[/]",
            "[white]Price[/]",
            "[white]Contents[/]"
        });
        
        for (int i = 0; i < boxes.Count; i++)
        {
            var box = boxes[i];
            var contents = box.Products.Any()
                ? string.Join(", ", box.Products.Select(p => p.Name))
                : "[grey]No items yet[/]";
        
            table.AddRow(
                (i + 1).ToString(),
                box.Name,
                $"€{box.Price}",
                contents
            );
        }
        
        AnsiConsole.Write(table);


        AnsiConsole.MarkupLine("\n[grey]Select a Christmas box to add to cart[/]");
        AnsiConsole.MarkupLine("[grey]Or press [green]ENTER[/] to go back[/]");
        
        //  "go back" option
        var backOption = new ChristmasBoxModel
        {
            ID = -1,
            Name = "Go back",
            Price = 0
        };

        var selectableBoxes = boxes
            .Append(backOption)
            .ToList();

        var selectedBox = Utils.CreateSelectionPrompt(
            selectableBoxes,
            title: "[white]Choose a Christmas box[/]",
            format: box =>
                box.ID == -1
                    ? "[grey]Go back[/]"
                    : $"{box.Name} — €{box.Price}"
        );

        // escape
        if (selectedBox.ID == -1)
        {
            return;
        }
        
        // addtocart
        OrderLogic.AddToCartProduct(selectedBox, 1);

        AnsiConsole.MarkupLine(
            $"[green]{selectedBox.Name} added to cart![/]"
        );
        
        AnsiConsole.MarkupLine("\n[grey]Press [green]ENTER[/] to go back[/]");
        Console.ReadKey();
        
    }
}