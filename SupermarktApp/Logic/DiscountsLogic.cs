using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using NUnit.Framework.Internal.Execution;
using Spectre.Console;

public class DiscountsLogic
{
    private static bool EmailSent = false;
    private static string DiscountTemplatePath = "EmailTemplates/DiscountTemplate.html";
    private static string DiscountTemplate = File.ReadAllText(DiscountTemplatePath);
    public static void AddDiscount(DiscountsModel Discount)
    {
        DiscountsAccess.AddDiscount(Discount);
    }

    public static List<DiscountsModel> GetWeeklyDiscounts() // this returns all ACTIVE weekly discounts
    {
        List<DiscountsModel> weeklyProducts = DiscountsAccess.GetWeeklyDiscounts().ToList();
        List<DiscountsModel> validWeeklyProducts = new List<DiscountsModel>();

        foreach(var discount in weeklyProducts)
        {
            if (discount != null && DateTime.Now >= discount.StartDate && DateTime.Now <= discount.EndDate)
            {
                validWeeklyProducts.Add(discount);
            }
        }
        return validWeeklyProducts;
    }

    public static List<DiscountsModel> GetValidPersonalDiscounts(int userID) // this returns all valid personal discounts for UserID
    {
        List<DiscountsModel> personalDiscounts = DiscountsAccess.GetPersonalDiscounts(userID);
        List<DiscountsModel> validDiscounts = new List<DiscountsModel>();
        foreach (DiscountsModel discount in personalDiscounts)
        {
            if (DateTime.Now >= discount.StartDate && DateTime.Now <= discount.EndDate)
            {
                validDiscounts.Add(discount);
            }
        }
        return validDiscounts;
    }
    public static List<ProductModel> GetPersonalDiscountsProducts(int userID) // returns a List with products that have a personal discount for the User
    {
        var personalDiscountsList = GetValidPersonalDiscounts(userID);
        List<ProductModel> personalDiscountsProducts = [];
        foreach (DiscountsModel personalDiscounts in personalDiscountsList)
        {
            if(personalDiscounts != null && DateTime.Now >= personalDiscounts.StartDate && DateTime.Now <= personalDiscounts.EndDate)
            {
                personalDiscountsProducts.Add(ProductAccess.GetProductByID(personalDiscounts.ProductID));
            }
        }
        return personalDiscountsProducts;
    }

    public static bool CheckUserIDForPersonalDiscount(int productID) // this checks if a product is in the personal discounts for currentUser
    {
        var personalDiscountList = GetValidPersonalDiscounts(SessionManager.CurrentUser.ID);
        foreach (DiscountsModel personalDiscount in personalDiscountList)
        {
            if (personalDiscount.ProductID == productID)
            {
                return true;
            }
        }
        return false;
    }
    public static void SeedPersonalDiscounts(int userID)
    {
        if (UserAccess.GetUserByID(userID) == null)
        {
            return;
        }

        RemoveAllPersonalDiscountsByUserID(userID); 
        List<ProductModel> top5Products = OrderItemsAccess.GetTop5MostBoughtProducts(userID);
        
        if (top5Products.Count < 5)
            return;

        Random rand = new Random();

        foreach (ProductModel product in top5Products)
        {
            double discountPercentage = rand.Next(1, 5);
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MaxValue;

            DiscountsModel discount = new DiscountsModel(
                productId: product.ID,
                discountPercentage: discountPercentage,
                discountType: "Personal",
                startDate: startDate,
                endDate: endDate,
                userId: userID
            );
            product.DiscountType = "Personal";
            product.DiscountPercentage = discountPercentage;
            AddDiscount(discount);
        }
        if (!EmailSent && top5Products.Count >= 5)
        {
            SentDiscountEmail(top5Products);
        }
        
    }

    public static async Task SentDiscountEmail(List<ProductModel> Top5List)
    {
        EmailSent = true;
        for (int i = 0; i < Top5List.Count; i++)
        {
            DiscountTemplate = DiscountTemplate.Replace($"--DISCOUNT.PRODUCT{i}--", Top5List[i].Name);
            DiscountTemplate = DiscountTemplate.Replace($"--DISCOUNT.PERCENTAGE{i}--", Top5List[i].DiscountPercentage.ToString());
            DiscountTemplate = DiscountTemplate.Replace($"--DISCOUNT.BEFORE{i}--", Top5List[i].Price.ToString());
            DiscountTemplate = DiscountTemplate.Replace($"--DISCOUNT.AFTER{i}--", Math.Round(Top5List[i].Price * (1 - Top5List[i].DiscountPercentage / 100.0), 2).ToString());
        }

        DiscountTemplate = DiscountTemplate.Replace("{{DISCOUNT.TYPE}}", Top5List[0].DiscountType);

        await EmailLogic.SendEmailAsync(
            to: UserAccess.GetUserEmail(SessionManager.CurrentUser!.ID)!,
            subject: "Your Discounts!",
            body: DiscountTemplate,
            isHtml: true
        );
    }

    public static bool IsDiscountActive(DiscountsModel discount)
    {
        if (discount == null)
            return false;

        DateTime now = DateTime.Now;
        return now >= discount.StartDate && now <= discount.EndDate;
    }

    public static DiscountsModel GetDiscountsByProductID(int productID)
    {
        return DiscountsAccess.GetDiscountByProductID(productID);
    }

    public static void RemoveDiscountByProductID(int productID)
    {
        DiscountsAccess.RemoveDiscountByProductID(productID);
    }

    public static void RemoveAllPersonalDiscountsByUserID(int userID)
    {
        DiscountsAccess.RemoveAllPersonalDiscountsByUserID(userID);
    }

    public static List<DiscountsModel> GetAllWeeklyDiscounts()
    {
        return DiscountsAccess.GetAllWeeklyDiscounts();
    }
    
    public static void RemoveDiscountByID(int discountID)
    {
        DiscountsAccess.RemoveDiscountByID(discountID);
    }
}