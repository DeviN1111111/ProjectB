public class RewardProductDTO
{
    public ProductModel Product { get; set; }
    public int PriceInPoints { get; set; }

    public RewardProductDTO() { }

    public RewardProductDTO(ProductModel product, int priceInPoints)
    {
        Product = product;
        PriceInPoints = priceInPoints;
    }
}