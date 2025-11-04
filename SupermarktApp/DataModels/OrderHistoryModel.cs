public class OrderHistoryModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public DateTime Date { get; set; }

    public OrderHistoryModel(int userId)
    {
        UserId = userId;
        Date = DateTime.Now;
    }
    public OrderHistoryModel(int userId, int productId)
    {
        UserId = userId;
        ProductId = productId;
        Date = DateTime.Now;
    }

    public OrderHistoryModel() { }
}
    