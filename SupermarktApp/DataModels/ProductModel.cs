public class ProductModel
{
    public int ID { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string NutritionDetails { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public int Location { get; set; }
    public int Quantity { get; set; }
    public int Visible { get; set; } = 1;
    public double CompetitorPrice { get; set; }
    public double DiscountPercentage { get; set; }
    public string DiscountType{ get; set;}
    public DateTime ExpiryDate { get; set; }
    public bool IsChristmasBoxItem { get; set; } // can it be USED in a box

    public ProductModel(string name, double price, string nutritionDetails, string description, string category, int location, int quantity, int visible)
    {
        Name = name;
        Price = price;
        NutritionDetails = nutritionDetails;
        Description = description;
        Category = category;
        Location = location;
        Quantity = quantity;
        Visible = visible;
    }

    public ProductModel(int id, string name, double price, string nutritionDetails, string description, string category, int location, int quantity, int visible)
    {
        ID = id;
        Name = name;
        Price = price;
        NutritionDetails = nutritionDetails;
        Description = description;
        Category = category;
        Location = location;
        Quantity = quantity;
        Visible = visible;
    }
    public ProductModel(int id, string name, double price)
    {
        ID = id;
        Name = name;
        Price = price;
    }

    public ProductModel() { }
}