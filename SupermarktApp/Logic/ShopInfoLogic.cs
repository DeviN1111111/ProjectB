using Spectre.Console;
using System.Text.RegularExpressions;
public static class ShopInfoLogic
{
    public static void UpdateDescription(string description)
    {
        ShopInfoModel shopInfo = ShopInfoAccess.GetShopInfo() ?? new ShopInfoModel();
        shopInfo.Description = description;
        ShopInfoAccess.UpdateShopInfo(shopInfo);
    }
    public static void UpdateOpeningHours(ShopInfoModel updatedHours)
    {
        ShopInfoAccess.UpdateShopInfo(updatedHours);
    }

    public static ShopInfoModel GetShopInfo()
    {
        return ShopInfoAccess.GetShopInfo() ?? new ShopInfoModel();
    }
    public static List<string[]> getDayDate()
    {
        DateTime today = DateTime.Today;
        // Calculate how many days have passed since Monday
        // zorg ervoor dat het altijd met maandag begint
        int daysSinceMonday = today.DayOfWeek == DayOfWeek.Monday 
            ? 6 
            : (int)today.DayOfWeek - (int)DayOfWeek.Monday;

        DateTime monday = today.AddDays(-daysSinceMonday);
        List<string[]> dayDate = new();

        for (int i = 0; i <= 7; i++)
        {
            DateTime date = monday.AddDays(i);
            string day = date.ToString("dddd");   // e.g. "Monday"
            string formattedDate = date.ToString("d MMMM yyyy"); // e.g. "4 November 2025"
            dayDate.Add(new[] { day, formattedDate });
        }

        return dayDate;
    }
}


