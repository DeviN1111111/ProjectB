using System.Threading.Tasks;
using Spectre.Console;

public static class PayLaterLogic
{
    private static readonly string PaymentTemplatePath = "EmailTemplates/PaymentCodeTemplate.html";
    private static readonly string PaymentTemplate = File.ReadAllText(PaymentTemplatePath);
    public const int FineDays = 30;
    public static async Task Activate(int orderId)
    {
        var order = OrderHistoryAccess.GetOrderById(orderId);

        DateTime fineDate = order.Date.Date.AddDays(FineDays);
        Random random = new Random();
        int paymentCode = random.Next(100000, 1000000);
        OrderHistoryAccess.UpdateIsPaidStatus(orderId, fineDate, isPaid: false, paymentCode);

        await SendEmail(paymentCode);
        
    }
    public static double ApplyFine(OrderHistoryModel order)
    {
        var orderItems = OrderLogic.GetOrderssByOrderId(order.Id);
        double totalOrderPrice = 0;
        double totalFine = 50;
        var productCounts = new Dictionary<int, int>();

        foreach (var item in orderItems)
        {
            if (productCounts.ContainsKey(item.ProductID))
                productCounts[item.ProductID]++;
            else
                productCounts[item.ProductID] = 1;
        }
        foreach (var keyValuePair in productCounts)
        {
            int productId = keyValuePair.Key;
            int quantity = keyValuePair.Value;

            var product = ProductAccess.GetProductByID(productId);
            if (product != null)
            {
                double price = product.Price;
                DiscountsModel? discount = DiscountsLogic.GetWeeklyDiscountByProductID(product.ID);
                if (discount.DiscountType == "Weekly" ||
                    (discount.DiscountType == "Personal" && DiscountsLogic.CheckUserIDForPersonalDiscount(product.ID)))
                {
                    price = Math.Round(product.Price * (1 - discount.DiscountPercentage / 100), 2);
                }

                double itemTotal = quantity * price;
                totalOrderPrice += itemTotal;
                totalFine += totalOrderPrice;
            }
        }
        return totalFine;       
    }
    public static double Track(UserModel user)
    {
        List<OrderHistoryModel> orders = OrderHistoryAccess.GetAllUserOrders(user.ID);

        if (orders == null || orders.Count == 0) return 0;

        double totalFine = 0;

        foreach (var order in orders)
        {
            if (!order.IsPaid && order.FineDate.HasValue)
            {
                if (DateTime.Now > order.FineDate)
                {
                    totalFine += ApplyFine(order);
                }
            }         
        }
        return totalFine;
    }
    public static async Task SendEmail(int paymentCode)
    {
        string email = UserAccess.GetUserEmail(SessionManager.CurrentUser!.ID)!;
        string emailBody = PaymentTemplate.Replace("{{PAYMENT_CODE}}", paymentCode.ToString());

        await EmailLogic.SendEmailAsync(
            to: email,
            subject: "Your Payment Code",
            body: emailBody,
            isHtml: true
        );    
    }
    public static bool Pay(int orderId, int code)
    {
        var order = OrderHistoryAccess.GetOrderById(orderId);

        if (code == order.PaymentCode)
        {
            OrderHistoryAccess.UpdateIsPaidStatus(order.Id, null, true, null);
            return true;
        }
        return false;
    }
    public static void Pay()
    {
        UserModel user = UserAccess.GetUserByID(SessionManager.CurrentUser!.ID)!;
        List<OrderHistoryModel> orders = OrderHistoryAccess.GetAllUserOrders(user.ID);

        foreach (var order in orders)
        {
            if (!order.IsPaid && order.FineDate.HasValue)
            {
                if (DateTime.Now > order.FineDate)
                {
                    OrderHistoryAccess.UpdateIsPaidStatus(order.Id, null, true, null);
                }
            }
        }
    }
}
