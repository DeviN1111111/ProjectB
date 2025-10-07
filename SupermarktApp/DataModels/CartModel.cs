public class CartModel
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }


    public CartModel(int userId, int productId, int quantity)
    {
        UserId = userId;
        ProductId = productId;
        Quantity = quantity;
    }
    public CartModel(){}
}