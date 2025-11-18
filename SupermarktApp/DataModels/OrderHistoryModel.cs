public class OrderHistoryModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? FineDate { get; set; }
    public int PaymentCode { get; set; }

    public OrderHistoryModel(int userId, bool isPaid = true)
    {
        UserId = userId;
        Date = DateTime.Now;
        IsPaid = isPaid;
    }
    public OrderHistoryModel() { }
}
    