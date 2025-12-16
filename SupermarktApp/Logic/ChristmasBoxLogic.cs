using System.Collections.Generic;
using System.Linq;

public static class ChristmasBoxLogic
{
    private static readonly Dictionary<int, double> BoxConfigurations = new()
    {
        { 2, 15 },
        { 4, 25 },
        { 6, 35 },
        { 8, 45 }
    };



    public static List<ChristmasBoxModel> GetAvailableBoxes()
    {
        var boxes = new List<ChristmasBoxModel>();

        foreach (var config in BoxConfigurations)
        {
            int persons = config.Key;
            double price = config.Value;

            var box = CreateBox(persons, price);
            if (IsValidBox(box))
                boxes.Add(box);
        }


        return boxes;
    }

    public static ChristmasBoxModel CreateBox(int persons, double boxPrice)
    {
        var eligibleProducts = ProductLogic.GetAllProducts()
            .Where(p => p.Category == "ChristmasBoxItem" && p.Visible == 1)
            .ToList();

        var selectedProducts = new List<ProductModel>();
        double totalValue = 0;

        foreach (var product in eligibleProducts)
        {
            selectedProducts.Add(product);
            totalValue += product.Price;

            if (selectedProducts.Count >= ChristmasBoxModel.MinimumProductsRequired
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
