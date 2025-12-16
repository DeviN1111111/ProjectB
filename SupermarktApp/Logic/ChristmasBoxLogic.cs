using System.Collections.Generic;

public static class ChristmasBoxLogic
{
    // Returns all available Christmas boxes (UI will use this later)
    public static List<ChristmasBoxModel> GetAvailableBoxes()
    {
        // TODO: Generate or fetch Christmas boxes
        return new List<ChristmasBoxModel>();
    }

    // Creates a single Christmas box for a given price
    public static ChristmasBoxModel CreateBox(double boxPrice)
    {
        // TODO: Select products and build a valid Christmas box
        return new ChristmasBoxModel();
    }

    // Validates if a Christmas box meets the business rules
    public static bool IsValidBox(ChristmasBoxModel box)
    {
        // TODO: Check minimum products and price rules
        return false;
    }
}