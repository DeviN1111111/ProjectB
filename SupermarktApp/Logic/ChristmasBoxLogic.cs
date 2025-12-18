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

        var baseBoxProducts = ProductAccess.GetAllProducts(includeHidden: true)
            .Where(p => p.Category == "ChristmasBox" && p.Visible == 1)
            .ToList();

        // debug
        // Console.WriteLine($"DEBUG: baseBoxProducts count = {baseBoxProducts.Count}");

        foreach (var baseProduct in baseBoxProducts)
        {
            boxes.Add(CreateBox(baseProduct)); // add products to box
        }
        return boxes;
    }

    public static ChristmasBoxModel CreateBox(ProductModel baseProduct)
    {
        int persons = int.Parse( // change the number in the string to int
            new string(
                baseProduct.Name
                .Where(char.IsDigit)
                .ToArray()
            )
        );

        // so you can add items bases on persons
        int itemCount = persons + 1; //add one more item than amount of persons
        var random = new Random();

        var products = ProductAccess.GetAllProducts(includeHidden: true)
            .Where(p => p.Visible == 1 && p.IsChristmasBoxItem)
            .OrderBy(_ => random.Next())   // show them random
            .Take(itemCount)
            .ToList();

        return new ChristmasBoxModel
        {
            ID = baseProduct.ID,  
            Name = baseProduct.Name,
            Price = baseProduct.Price,
            Category = baseProduct.Category,
            Visible = baseProduct.Visible,
            Products = products
        };
    }


    public static bool IsValidBox(ChristmasBoxModel box)
    {
        if (box.Products.Count < ChristmasBoxModel.MinimumProductsRequired)
            return false;

        return true;
    }
}
