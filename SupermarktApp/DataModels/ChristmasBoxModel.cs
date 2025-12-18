public class ChristmasBoxModel : ProductModel
{   // inherit from ProductModel
    public const int MinimumProductsRequired = 3; // box must contain 3

    public List<ProductModel> Products { get; set; } = new(); // list of products in xmas-box

    public decimal TotalProductsValue =>
        // calculate total price content box
        Products.Sum(p => (decimal)p.Price);  

}
