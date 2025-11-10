using System.Reflection.Metadata.Ecma335;
using Spectre.Console;

public class DiscountsLogic
{
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static void AddDiscount(DiscountsModel Discount)
    {
        DiscountsAccess.AddDiscount(Discount);
    }
}