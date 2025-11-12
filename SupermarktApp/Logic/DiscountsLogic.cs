using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using Spectre.Console;

public class DiscountsLogic
{
    public static void AddDiscount(DiscountsModel Discount)
    {
        DiscountsAccess.AddDiscount(Discount);
    }

    public static List<ProductModel> GetWeeklyDiscounts() // this returns all ACTIVE weekly discounts
    {
        List<ProductModel> weeklyProducts = DiscountsAccess.GetWeeklyDiscounts().ToList();
        List<ProductModel> validWeeklyProducts = new List<ProductModel>();

        foreach (var product in weeklyProducts)
        {
            var discount = DiscountsAccess.GetDiscountsByProductID(product.ID);

            if (product != null && DateTime.Now >= discount.StartDate && DateTime.Now <= discount.EndDate)
            {
                validWeeklyProducts.Add(product);
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
        List<ProductModel> top5Products = OrderAccess.GetTop5MostBoughtProducts(userID);

        if (top5Products.Count < 5)
            return;

        Random rand = new Random();

        foreach (ProductModel product in top5Products)
        {
            double discountPercentage = rand.Next(5, 41); 
            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate.AddDays(7);

            DiscountsModel discount = new DiscountsModel(
                productId: product.ID,
                discountPercentage: discountPercentage,
                discountType: "Personal",
                startDate: startDate,
                endDate: endDate,
                userId: userID
            );

            AddDiscount(discount);
        }
    }
}