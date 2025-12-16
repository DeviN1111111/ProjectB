using System.Collections.Generic;
using System.Linq;

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
        var box = new ChristmasBoxModel
        {
            Name = $"Christmas Box €{Math.Round(boxPrice, 2)}",
            Price = boxPrice
        };

        if (boxPrice <= 0)
        {
            return box;
        }

        var eligibleProducts = ProductAccess.GetChristmasBoxEligibleProducts()
            .OrderByDescending(p => p.Price)
            .ToList();

        decimal runningTotal = 0;

        foreach (var product in eligibleProducts)
        {
            box.Products.Add(product);
            runningTotal += (decimal)product.Price;

            if (box.Products.Count >= ChristmasBoxModel.MinimumProductsRequired && runningTotal >= (decimal)boxPrice)
            {
                return box;
            }
        }

        return box;
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
