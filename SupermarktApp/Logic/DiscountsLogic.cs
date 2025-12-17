using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using Microsoft.VisualBasic;
using NUnit.Framework.Internal.Execution;
using Spectre.Console;

public class DiscountsLogic
{

    private static bool DiscountMailSent = false;
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
            var discountedProduct = CheckDiscountByProduct(ProductLogic.GetProductById(discount.ProductID));

            if(discountedProduct != null && discountedProduct.Discount.DiscountType == "Weekly")
            {
                if (discount != null && DateTime.Now >= discount.StartDate && DateTime.Now <= discount.EndDate)
                {
                    validWeeklyProducts.Add(discount);
                }
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
            var discountedProduct = CheckDiscountByProduct(ProductLogic.GetProductById(discount.ProductID));
            
            if(discountedProduct != null && discountedProduct.Discount.DiscountType == "Personal")
            {
                if (DateTime.Now >= discount.StartDate && DateTime.Now <= discount.EndDate)
                {
                validDiscounts.Add(discount);
                }   
            }
        }
        return validDiscounts;
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

    public static void AddExpiryDateDiscounts(int daysBeforeExpiry = 3, double discountPercentage = 30.0)
    {
        var allExpiryDiscounts = DiscountsAccess.GetAllExpiryDiscounts();
        
        foreach(var discount in allExpiryDiscounts)
        {
            RemoveDiscountByID(discount.ID);
        }

        var allProducts = ProductAccess.GetAllProducts();

        foreach(var product in allProducts)
        {
            if(product == null || product.ExpiryDate == null) continue;

            int daysUntilExpiry = (product.ExpiryDate.Date - DateTime.Today).Days;

            if (daysUntilExpiry >= 0 && daysUntilExpiry <= daysBeforeExpiry)
            {
                var discount = new DiscountsModel(
                    productId: product.ID,
                    discountPercentage: discountPercentage,
                    discountType: "Expiry",
                    startDate: DateTime.Now,
                    endDate: product.ExpiryDate,
                    userId: null
                    );
                AddDiscount(discount);
            }
        }
    }
    public static ProductDiscountDTO? CheckDiscountByProduct(ProductModel product)
    {
        List<DiscountsModel> discountModel = DiscountsAccess.GetAllDiscountByProductID(product.ID);

        ProductDiscountDTO? productDiscount = null;

        foreach (var discount in discountModel)
        {
            if(IsDiscountActive(discount) && discount.DiscountType == "Expiry")
            {
                return new ProductDiscountDTO(product, discount);
            }

            if(IsDiscountActive(discount) && discount.DiscountType == "Weekly")
            {
                if(productDiscount != null && productDiscount.Discount?.DiscountType == "Expiry") continue;
                productDiscount = new ProductDiscountDTO(product, discount);
            }

            if(IsDiscountActive(discount) && discount.DiscountType == "Personal" && discount.UserID == SessionManager.CurrentUser?.ID)
            {
                if(productDiscount != null && (productDiscount.Discount?.DiscountType == "Expiry" || productDiscount.Discount.DiscountType == "Weekly")) continue;
                productDiscount = new ProductDiscountDTO(product, discount);
            }
        }
        return productDiscount;
    }
    public static DiscountsModel GetWeeklyDiscountByProductID(int productID)
    {
        return DiscountsAccess.GetWeeklyDiscountByProductID(productID);
    }
    public static void SeedPersonalDiscounts(int userID)
    {
        if (UserAccess.GetUserByID(userID) == null)
        {
            return;
        }

        RemoveAllPersonalDiscountsByUserID(userID);
        List<ProductModel> top5Products = OrderItemAccess.GetTop5MostBoughtProducts(userID);

        if (top5Products.Count < 5)
            return;

        Random rand = new Random();

        foreach (ProductModel product in top5Products)
        {
            var discountedProduct = CheckDiscountByProduct(ProductLogic.GetProductById(product.ID));
            
            if(discountedProduct != null)
            {
                if(discountedProduct.Discount.DiscountType != "Personal")
                {
                    continue;
                }
            }

            double discountPercentage = rand.Next(5, 41); 
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
            AddDiscount(discount);
        }

        SentPersonalDiscountEmail(top5Products);
    }
    
    public static DiscountsModel GetPeronsalDiscountByProductAndUserID(int productID, int UserID)
    {
        return DiscountsAccess.GetPeronsalDiscountByProductAndUserID(productID, UserID);
    }
    public static async Task SentPersonalDiscountEmail(List<ProductModel> Top5List)
    {
        for (int i = 0; i < Top5List.Count; i++)
        {
            var discount = GetPeronsalDiscountByProductAndUserID(Top5List[i].ID, SessionManager.CurrentUser!.ID);

            DiscountTemplate = DiscountTemplate.Replace($"--DISCOUNT.PRODUCT{i}--", Top5List[i].Name);
            DiscountTemplate = DiscountTemplate.Replace($"--DISCOUNT.PERCENTAGE{i}--", discount.DiscountPercentage.ToString());
            DiscountTemplate = DiscountTemplate.Replace($"--DISCOUNT.BEFORE{i}--", Top5List[i].Price.ToString());
            DiscountTemplate = DiscountTemplate.Replace($"--DISCOUNT.AFTER{i}--", Math.Round(Top5List[i].Price * (1 - discount.DiscountPercentage / 100.0), 2).ToString());
        }

        DiscountTemplate = DiscountTemplate.Replace("{{DISCOUNT.TYPE}}", "Personal");
        if(!DiscountMailSent)
        {
            await EmailLogic.SendEmailAsync(
            to: UserAccess.GetUserEmail(SessionManager.CurrentUser!.ID)!,
            subject: "Your Discounts!",
            body: DiscountTemplate,
            isHtml: true
            );
            
            DiscountMailSent = true;
        }
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
    
    public static List<DiscountsModel> GetAllExpiryDiscounts()
    {
        return DiscountsAccess.GetAllExpiryDiscounts();
    }
    public static void RemoveDiscountByID(int discountID)
    {
        DiscountsAccess.RemoveDiscountByID(discountID);
    }
}