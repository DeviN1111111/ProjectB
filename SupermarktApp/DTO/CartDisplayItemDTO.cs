public class CartDisplayItemDTO
{
    public int ProductId { get; set; }

    public string Name { get; set; } = "";
    public int Quantity { get; set; }

    // display-only strings
    public string PriceText { get; set; } = "";
    public string TotalText { get; set; } = "";

    // numeric values for summary calculation
    public double TotalValue { get; set; }
    public double DiscountValue { get; set; }

    // only for Christmas boxes
    public List<string>? Contents { get; set; }
}
