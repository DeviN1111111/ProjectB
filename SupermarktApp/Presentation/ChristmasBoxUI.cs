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
            var contents = string.Join(", ", box.Products.Select(p => p.Name));
        
            table.AddRow(
                (i + 1).ToString(),
                box.Name,
                $"€{box.Price}",
                contents
            );
        }
        
        AnsiConsole.Write(table);
        
        AnsiConsole.MarkupLine("\n[grey]Press [green]ENTER[/] to go back[/]");
        Console.ReadKey();
        
    }
}