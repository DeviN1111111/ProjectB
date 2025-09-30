using System.Reflection.Emit;

public static class MapLogic
{
    private const string Reset = "\u001b[0m";
    private const string Green = "\u001b[32m";

public static Tuple<int, int> GetCategoryCoordinates(string label)
{
    label = label.ToLower().Trim();

    if (label == "bakery") return Tuple.Create(0, 0);
    else if (label == "fruits") return Tuple.Create(0, 1);
    else if (label == "vegetables") return Tuple.Create(0, 2);
    else if (label == "dairy") return Tuple.Create(0, 3);
    else if (label == "meat") return Tuple.Create(0, 4);
    else if (label == "seafood") return Tuple.Create(1, 0);
    else if (label == "frozen foods") return Tuple.Create(1, 1);
    else if (label == "beverages") return Tuple.Create(1, 2);
    else if (label == "snacks") return Tuple.Create(1, 3);
    else if (label == "confectionery") return Tuple.Create(1, 4);
    else if (label == "canned goods") return Tuple.Create(2, 0);
    else if (label == "pasta & rice") return Tuple.Create(2, 1);
    else if (label == "cereals") return Tuple.Create(2, 2);
    else if (label == "condiments & sauces") return Tuple.Create(2, 3);
    else if (label == "spices & herbs") return Tuple.Create(2, 4);
    else if (label == "household supplies") return Tuple.Create(3, 0);
    else if (label == "cleaning products") return Tuple.Create(3, 1);
    else if (label == "personal care") return Tuple.Create(3, 2);
    else if (label == "pet supplies") return Tuple.Create(3, 3);
    else if (label == "alcohol") return Tuple.Create(3, 4);
    else return Tuple.Create(-1, -1);
}



    public static string ReturnMapString(Tuple<int, int> CatogoryCoords, int mapWidth, int mapHeight)
    {
        string mapString = "";

        if (CatogoryCoords.Item1 == -1 && CatogoryCoords.Item2 == -1)
        {
            return "this map doesnt exist g";
        }

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                if (i == CatogoryCoords.Item1 && j == CatogoryCoords.Item2)
                {
                    mapString += Green + " ████ " + Reset;
                }
                else
                { mapString += " ████ "; }
            }
            mapString += "\n";
            mapString += "\n";
        }

        for (int j = 0; j < mapWidth; j++)
        {
            mapString += " ████ ";
        }
        return mapString;
    }
    
}