class ShopDetailsModel
{
    public int ID { get; set; }
    public string? Description { get; set; }
    public string? OpeningHour { get; set; }
    public string? ClosingHour { get; set; }
    public string? OpeningHourSunday { get; set; }
    public string? ClosingHourSunday { get; set; }

    public ShopDetailsModel(int id, string? description, string? openingHour, string? closingHour, string? openingHourSunday, string? closingHourSunday)
    {
        ID = id;
        Description = description;
        OpeningHour = openingHour;
        ClosingHour = closingHour;
        OpeningHourSunday = openingHourSunday;
        ClosingHourSunday = closingHourSunday;
    }
    public ShopDetailsModel(){}

}