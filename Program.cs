class Program
{
    static void Main()
    {
        ProductAccess.CreateTable();

        var products = new List<ProductModel>
        {
            new ProductModel { Name = "Apple", Price = 0.5, NutritionDetails = "52 kcal per 100g", Description = "Fresh red apple", Category = "Fruits" },
            new ProductModel { Name = "Banana", Price = 0.3, NutritionDetails = "89 kcal per 100g", Description = "Yellow ripe banana", Category = "Fruits" },
            new ProductModel { Name = "Carrot", Price = 0.2, NutritionDetails = "41 kcal per 100g", Description = "Organic carrot", Category = "Vegetables" },
            new ProductModel { Name = "Milk", Price = 1.2, NutritionDetails = "42 kcal per 100ml", Description = "Full cream milk", Category = "Dairy" },
            new ProductModel { Name = "Bread", Price = 1.0, NutritionDetails = "265 kcal per 100g", Description = "Whole wheat bread", Category = "Bakery" }
        };

        ProductAccess.InsertProducts(products);

        var allProducts = ProductAccess.GetAllProducts().ToList();
        foreach (var p in allProducts)
        {
            Console.WriteLine($"{p.Id}: {p.Name} ({p.Category}) - ${p.Price}");
        }

        Console.WriteLine("Select a product to view the location on the map (enter Id):");

        string userinput = Console.ReadLine();

        if (int.TryParse(userinput, out int productId))
        {
            Console.WriteLine($"You selected {allProducts[productId - 1].Name}. Opening map...");
            MapUI.DisplayMap(allProducts[productId - 1].Category);
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid product Id.");
        }
    }
}
