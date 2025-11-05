using System.Reflection;

public class WeeklyPromotionsModel
{
    public int ID { get; set; }
    public int ProductID { get; set; }
    public double Discount { get; set; }

    public WeeklyPromotionsModel(int productID, double discount)
    {
        ProductID = productID;
        Discount = discount;
    }

    public WeeklyPromotionsModel() { }
}