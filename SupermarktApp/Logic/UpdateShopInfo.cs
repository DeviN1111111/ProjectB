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

}