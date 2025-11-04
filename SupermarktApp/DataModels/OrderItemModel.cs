public class OrderItemModel
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }

    public OrderItemModel(int productId, int quantity, double price)
    {
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }
    public OrderItemModel()
    {
    }
}