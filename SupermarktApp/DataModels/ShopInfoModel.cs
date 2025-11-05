public class ShopInfoModel
{
    public int Id { get; set; }
    public string Description { get; set; }

    public string OpeningHourMonday { get; set; }
    public string ClosingHourMonday { get; set; }

    public string OpeningHourTuesday { get; set; }
    public string ClosingHourTuesday { get; set; }

    public string OpeningHourWednesday { get; set; }
    public string ClosingHourWednesday { get; set; }

    public string OpeningHourThursday { get; set; }
    public string ClosingHourThursday { get; set; }

    public string OpeningHourFriday { get; set; }
    public string ClosingHourFriday { get; set; }

    public string OpeningHourSaturday { get; set; }
    public string ClosingHourSaturday { get; set; }

    public string OpeningHourSunday { get; set; }
    public string ClosingHourSunday { get; set; }

    public ShopInfoModel(string description, string openingHourMonday, string closingHourMonday,
        string openingHourTuesday, string closingHourTuesday,
        string openingHourWednesday, string closingHourWednesday,
        string openingHourThursday, string closingHourThursday,
        string openingHourFriday, string closingHourFriday,
        string openingHourSaturday, string closingHourSaturday,
        string openingHourSunday, string closingHourSunday)
    {
        Description = description;
        OpeningHourMonday = openingHourMonday; ClosingHourMonday = closingHourMonday;
        OpeningHourTuesday = openingHourTuesday; ClosingHourTuesday = closingHourTuesday;
        OpeningHourWednesday = openingHourWednesday; ClosingHourWednesday = closingHourWednesday;
        OpeningHourThursday = openingHourThursday; ClosingHourThursday = closingHourThursday;
        OpeningHourFriday = openingHourFriday; ClosingHourFriday = closingHourFriday;
        OpeningHourSaturday = openingHourSaturday; ClosingHourSaturday = closingHourSaturday;
        OpeningHourSunday = openingHourSunday; ClosingHourSunday = closingHourSunday;
    }
    public ShopInfoModel() { }
}