using System;
using System.Text;

public static class MapUI
{
    public static void DisplayMap(int box)
    {
        Console.WriteLine(MapLogic.MapBuilder(box));
    }
}