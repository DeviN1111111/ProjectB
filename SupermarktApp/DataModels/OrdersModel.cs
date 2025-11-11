public class OrdersModel
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public int ProductID { get; set; }
     public int OrderId { get; set; }
    public double Price { get; set; }
    public DateTime Date { get; set; }

    public OrdersModel(int userID)
    {
        UserID = userID;
        Date = DateTime.Now;
    }
    public OrdersModel(int userID, int productID, double price)
    {
        UserID = userID;
        ProductID = productID;
        Price = price;
        Date = DateTime.Now;
    }
    public OrdersModel() { }
}