using System.Collections.Generic;
using System.Linq;

public static class ChristmasBoxLogic
{
    private static readonly Dictionary<int, double> BoxConfigurations = new()
    {   // box size (pp), en de prijs van box
        { 1, 15 },
        { 2, 25 },
        { 4, 35 },
        { 6, 45 },
        { 8, 55 }
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

        // so you can add items bases on price
        double targetPrice = persons switch
        {   // grap per persons the price
            2 => 15,
            4 => 25,
            6 => 35,
            8 => 45,
            _ => baseProduct.Price 
        };

        var random = new Random();

        var eligibleProducts = ProductAccess.GetAllProducts(includeHidden: true)
            .Where(p => p.Visible == 1 && p.IsChristmasBoxItem && p.Price > 0)
            .OrderBy(_ => random.Next())   // show them random
            .ToList();
        
            // fill box up until price is reached
            var selectedProducts = new List<ProductModel>();
            double currentTotal = 0;

            foreach (var product in eligibleProducts)
            {
                if (currentTotal + product.Price > targetPrice)
                    continue;

                selectedProducts.Add(product);
                currentTotal += product.Price;

                if (currentTotal >= targetPrice)
                    break;
            }

        return new ChristmasBoxModel
        {
            ID = baseProduct.ID,  
            Name = baseProduct.Name,
            Price = baseProduct.Price,
            Category = baseProduct.Category,
            Visible = baseProduct.Visible,
            Products = selectedProducts
        };
    }
}