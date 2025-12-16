using Spectre.Console;
using System;

public static class ChristmasBoxUI
{
    public static void Show()
    {
        Console.Clear();
        Utils.PrintTitle("Christmas Boxes");

        //// remove later ////
        var table = Utils.CreateTable(new[]
        {
            "[white]Box[/]",
            "[white]Price[/]",
            "[white]Contents[/]"
        });

        table.AddRow(
            "Christmas Box €10",
            "€10",
            "Chocolate, Cookies, Tea"
        );

        table.AddRow(
            "Christmas Box €20",
            "€20",
            "Wine, Cheese, Chocolate"
        );

        table.AddRow(
            "Christmas Box €50",
            "€50",
            "Wine, Cheese, Snacks, Chocolate"
        );

        AnsiConsole.Write(table);

        AnsiConsole.MarkupLine("\n[grey]Press [green]ENTER[/] to go back[/]");
        Console.ReadKey();
        //// remove later ////
    }
}