public static class ReturnItemLogic
{
    public static List<OrderHistoryModel> CheckReturnableOrders(
        List<OrderHistoryModel> orders,
        DateTime today)
    {
        return orders
            .Where(o => (today - o.Date.Date).TotalDays <= 3 && o.IsPaid)
            .ToList();
    }
}