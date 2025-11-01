public class OrderHistoryModel
{
    public int UserId { get; set; }
    public List<OrderItemModel> Products { get; set; }
    public DateTime OrderDate { get; set; }

    public OrderHistoryModel(int userId, List<OrderItemModel> products, DateTime orderDate)
    {
        UserId = userId;
        Products = products;
        OrderDate = orderDate;
    }
}
    