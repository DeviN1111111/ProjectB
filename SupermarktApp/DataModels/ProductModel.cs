public class ProductModel
{
    public int ID { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string NutritionDetails { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string Location { get; set; }
    public int Quantity { get; set; }

    public ProductModel(string name, double price, string nutritionDetails, string description, string category, string location, int quantity)
    {
        Name = name;
        Price = price;
        NutritionDetails = nutritionDetails;
        Description = description;
        Category = category;
        Location = location;
        Quantity = quantity;
    }

    public ProductModel() { }
}