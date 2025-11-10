public class DiscountsModel
{
    public int ID { get; set; }
    public int ProductID { get; set; }
    public int? UserID { get; set; } 
    public double DiscountPercentage { get; set; }
    public string DiscountType { get; set; } = "None";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public DiscountsModel(int productId, double discountPercentage, string discountType, DateTime startDate, DateTime endDate, int? userId = null)
    {
        ProductID = productId;
        DiscountPercentage = discountPercentage;
        DiscountType = discountType;
        StartDate = startDate;
        EndDate = endDate;
        UserID = userId;
    }
}
