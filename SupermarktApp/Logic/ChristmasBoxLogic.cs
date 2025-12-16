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
            if (IsValidBox(box))
                boxes.Add(box); // add to list of boxes
        }

        return boxes; // return the list of boxes
    }

    public static ChristmasBoxModel CreateBox(int persons, double boxPrice)
    {
        // var eligibleProducts = ProductLogic.GetAllProducts() // get aal products admin selected 
        //     .Where(p => p.Category == "ChristmasBoxItem" && p.Visible == 1)
        //     .ToList();

        var eligibleProducts = ProductLogic.GetAllProducts() //TEST//
            .Where(p => p.Visible == 1)
            .ToList();


        var selectedProducts = new List<ProductModel>(); // list of product for the box
        double totalValue = 0; // tracks selected product prices

        foreach (var product in eligibleProducts)
        {
            selectedProducts.Add(product); // add product to boxxx
            totalValue += product.Price;   // add price to total valvue 

            if (selectedProducts.Count >= ChristmasBoxModel.MinimumProductsRequired // check if valid
                && totalValue >= boxPrice)
                break;
        }

        return new ChristmasBoxModel
        {
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

        if (box.TotalProductsValue < (decimal)box.Price)
            return false;

        return true;
    }
}
