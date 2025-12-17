public class OrderItemsModel
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public int ProductID { get; set; }
    public int OrderId { get; set; }
    public double Price { get; set; }
    public DateTime Date { get; set; }


    public OrderItemsModel(int userID)
    {
        UserID = userID;
        Date = DateTime.Now;
    }
    public OrderItemsModel(int userID, int productID, double price)
    {
        UserID = userID;
        ProductID = productID;
        Price = price;
        Date = DateTime.Now;
    }
    public OrderItemsModel() { }
}