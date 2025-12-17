using System.Collections.Generic;
using System.Linq;

public static class ChristmasBoxLogic
{
    private static readonly Dictionary<int, double> BoxConfigurations = new()
    {   // box size (pp), en de prijs van box
        { 2, 15 },
        { 4, 25 },
        { 6, 35 },
        { 8, 45 }
    };

    public static List<ChristmasBoxModel> GetAvailableBoxes() // create all available boxes
    {   
        var boxes = new List<ChristmasBoxModel>(); // creates the list of boxes

        foreach (var config in BoxConfigurations)
        {
            int persons = config.Key;
            double price = config.Value;

            var box = CreateBox(persons, price); // create each box per size
            // Console.WriteLine($"Created box for {persons} persons with {box.Products.Count} products, total value {box.TotalProductsValue}"); //test

            if (IsValidBox(box))
                boxes.Add(box); // add to list of boxes
        }

        return boxes; // return the list of boxes
        // Console.WriteLine($"Created {boxes.Count} Christmas boxes");

    }

    public static ChristmasBoxModel CreateBox(int persons, double boxPrice)
    {
        var eligibleProducts = ProductAccess.GetAllProducts(includeHidden: true) // get aal products admin selected 
            .Where(p =>
                p.Visible == 1 &&
                p.Category != null &&
                p.Category.Equals("ChristmasBoxItem", StringComparison.OrdinalIgnoreCase)
            )
            .ToList();

        var selectedProducts = new List<ProductModel>(); // list of product for the box
        var random = new Random();

        foreach (var product in eligibleProducts.OrderBy(_ => random.Next())) // create boxes random items
        {
            selectedProducts.Add(product); // add product to boxxx

            // add 1 items to minimun with bigger boxes
            int requiredItems = Math.Max(
                ChristmasBoxModel.MinimumProductsRequired,
                persons + 1
            );

            if (selectedProducts.Count >= requiredItems)
                break;

        }

        return new ChristmasBoxModel
        {
            ID = -persons, // negative IDs = virtual products
            Name = $"Christmas Box for {persons} persons",
            Price = boxPrice,
            Category = "ChristmasBox",
            Visible = 1,
            Products = selectedProducts
        };

    }

    public static bool IsValidBox(ChristmasBoxModel box)
    {
        if (box.Products.Count < ChristmasBoxModel.MinimumProductsRequired)
            return false;

        // if (box.TotalProductsValue < (decimal)box.Price)
        //     return false;

        return true;
    }
}
