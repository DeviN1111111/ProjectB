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
        if (box == null)
        {
            return false;
        }

        if (!box.HasMinimumProducts())
        {
            return false;
        }

        return box.TotalProductsValue >= (decimal)box.Price && box.Price > 0;
    }
}
