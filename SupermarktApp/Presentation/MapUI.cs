using System;
using System.Text;
using Spectre.Console;

public static class MapUI
{
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static void DisplayMap(int box)
    {
        Console.Clear();
            AnsiConsole.Write(
            new FigletText("Map")
            .Centered()
            .Color(AsciiPrimary));
        Console.WriteLine(MapLogic.MapBuilder(box));
        Console.ReadKey();
    }
}