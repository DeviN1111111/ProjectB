public static class MapUI
{
    public static void DisplayMap(string label)
    {
        string mapString = MapLogic.ReturnMapString(MapLogic.GetCategoryCoordinates(label), 10, 40);
        Console.WriteLine(mapString);
    }
}