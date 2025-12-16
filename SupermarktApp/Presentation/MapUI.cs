using System;
using System.Text;
using Spectre.Console;

public static class MapUI
{
    public static void DisplayMap(int box)
    {
        Console.Clear();
        Utils.PrintTitle("Store Map");
        Console.WriteLine(MapLogic.MapBuilder(box));
        Console.ReadKey();
        AnsiConsole.MarkupLine("[green]Press ENTER to return to the main menu.[/]");
    }
}