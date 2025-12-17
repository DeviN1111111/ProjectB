using Spectre.Console;
using System.Text.RegularExpressions;
public static class ShopInfoLogic
{
    public static void UpdateDescription(string description)
    {
        ShopInfoAccess.UpdateShopInfoDescription(description);
    }

    public static List<ShopInfoModel> GetDescriptionAndAllDays()
    {
        return ShopInfoAccess.GetDescriptionAndAllDays();
    }

    public static void UpdateOpeningHours(string newOpeningHour, string newClosingHour, string day)
    {
        ShopInfoAccess.UpdateOpeningHours(newOpeningHour, newClosingHour, day);
    }
}


