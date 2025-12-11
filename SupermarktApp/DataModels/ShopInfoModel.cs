public class ShopInfoModel
{
    public int Id { get; set; }
    public string Day { get; set; }
    public string OpeningHour { get; set; }
    public string ClosingHour { get; set; }

    public ShopInfoModel(string day, string openingHour, string closingHour)
    {
        Day = day;
        OpeningHour = openingHour;
        ClosingHour = closingHour;
    }
    public ShopInfoModel() { }
}