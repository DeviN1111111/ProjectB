using Spectre.Console;
using System.Text.RegularExpressions;
public class UpdateShopInfo
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

}