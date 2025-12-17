public class CartProductModel
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public double Discount { get; set; }
    public double RewardPrice { get; set; } = 0;

    public CartProductModel(int userId, int productId, int quantity)
    {
        UserId = userId;
        ProductId = productId;
        Quantity = quantity;
    }
    public CartProductModel(){}
}