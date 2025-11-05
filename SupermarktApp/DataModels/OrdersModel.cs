public class OrdersModel
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public int ProductID { get; set; }
    public DateTime Date { get; set; }

    public OrdersModel(int userID)
    {
        UserID = userID;
        Date = DateTime.Now;
    }
    public OrdersModel(int userID, int productID)
    {
        UserID = userID;
        ProductID = productID;
        Date = DateTime.Now;
    }

    public OrdersModel() { }
}