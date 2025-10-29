public class RewardItemsModel
{
    public int ID { get; set; }
    public int ProductId { get; set; }
    public int PriceInPoints { get; set; }

    public RewardItemsModel(int productId, int priceInPoints)
    {
        ProductId = productId;
        PriceInPoints = priceInPoints;
    }

    public RewardItemsModel() { }
}