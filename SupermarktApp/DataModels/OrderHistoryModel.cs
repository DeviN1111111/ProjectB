public class OrderHistoryModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }

    public OrderHistoryModel(int userId)
    {
        UserId = userId;
        Date = DateTime.Now;
    }
    public OrderHistoryModel() { }
}
    