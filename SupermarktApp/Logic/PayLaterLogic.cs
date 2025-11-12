using System.Threading.Tasks;
using Spectre.Console;

public static class PayLaterLogic
{
    public const int FineDays = 30;
    public static async Task Activate(int orderId)
    {
        var order = OrderHistoryAccess.GetOrderById(orderId);

        DateTime fineDate = order.Date.AddDays(FineDays);
        Random random = new Random();
        int paymentCode = random.Next(100000, 1000000);
        OrderHistoryAccess.UpdateIsPaidStatus(orderId, fineDate, isPaid: false, paymentCode);

        await SendEmail(paymentCode);
        
    }
    public static double ApplyFine() => 50;
    public static double Track(UserModel user)
    {
        List<OrderHistoryModel> orders = OrderHistoryAccess.GetAllUserOrders(user.ID);
        if (orders == null || orders.Count == 0) return 0;

        double totalFine = 0;

        foreach (var order in orders)
        {
            if (!order.IsPaid && order.FineDate.HasValue)
            {
                if (DateTime.Today > order.FineDate.Value.Date)
                {
                    totalFine += ApplyFine();
                }
            }         
        }
        return totalFine;
    }
    public static async Task SendEmail(int paymentCode)
    {
        string email = UserAccess.GetUserEmail(SessionManager.CurrentUser!.ID)!;
        await PaymentEmailLogic.SendEmailAsync(email!, paymentCode);
        
    }
    public static bool Pay(int orderId, int code)
    {
        var order = OrderHistoryAccess.GetOrderById(orderId);

        if (code == order.PaymentCode)
        {
            OrderHistoryAccess.UpdateIsPaidStatus(order.Id, null, true, null);
            order = OrderHistoryAccess.GetOrderById(orderId);
        }
        return order.IsPaid;
    }
}