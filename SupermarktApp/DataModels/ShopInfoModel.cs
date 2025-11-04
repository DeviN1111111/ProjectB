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
    public ShopInfoModel()
    {
        Description = @"
        Welcome to our supermarket — where freshness comes first.
        Our bakery opens early with warm, freshly baked bread, and all our vegetables are kept perfectly cooled throughout the day.
        Most restocking takes place in the evening, so the shelves are full and ready for you every morning.";
        OpeningHourMonday = "07:00";
        ClosingHourMonday = "22:00";
        OpeningHourTuesday = "07:00";
        ClosingHourTuesday = "22:00";
        OpeningHourWednesday = "07:00";
        ClosingHourWednesday = "22:00";
        OpeningHourThursday = "07:00";
        ClosingHourThursday = "22:00";
        OpeningHourFriday = "07:00";
        ClosingHourFriday = "22:00";
        OpeningHourSaturday = "08:00";
        ClosingHourSaturday = "20:00";
        OpeningHourSunday = "08:00";
        ClosingHourSunday = "20:00";
    }
    // public ShopInfoModel() { }
}