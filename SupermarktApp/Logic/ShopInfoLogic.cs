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
        List<string[]> dayDate = new();

        for (int i = 0; i <= 6; i++)
        {
            DateTime date = today.AddDays(i);
            string day = date.ToString("dddd");
            string formattedDate = date.ToString("d MMMM yyyy");
            dayDate.Add(new[] { day, formattedDate });
        }

        return dayDate;
    }
}


