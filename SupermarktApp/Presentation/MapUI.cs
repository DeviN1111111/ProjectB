public static class MapUI
{
    public static void DisplayMap(string label)
    {
        string mapString = MapLogic.ReturnMapString(MapLogic.GetCategoryCoordinates(label), 5, 4);
        Console.WriteLine(mapString);
    }
}