public class ProductModel
{
    public int ID { get; set; }
    public string? Name { get; set; }
    public double Price { get; set; }
    public string NutritionDetails { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Category { get; set; } = null!;
    public int Quantity { get; set; } 
}