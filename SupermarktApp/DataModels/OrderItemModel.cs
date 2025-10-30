public class OrderItemModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public OrderItemModel(int id, int productId, int quantity, decimal price)
    {
        Id = id;
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }
}