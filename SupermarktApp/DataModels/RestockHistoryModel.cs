using System;
public class RestockHistoryModel
{
    public int ID { get; set; }
    public int ProductID { get; set; }
    public int QuantityAdded { get; set; }
    public DateTime RestockDate { get; set; }
    public double CostPerUnit { get; set; }


    public RestockHistoryModel(int productId, int quantityAdded, DateTime restockDate, double costPerUnit)
    {
        ProductID = productId;
        QuantityAdded = quantityAdded;
        RestockDate = restockDate;
        CostPerUnit = costPerUnit;
    }
    public RestockHistoryModel() { }
}

