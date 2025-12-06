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
    public static List<ProductModel> CheckReturnableProducts(OrderHistoryModel orderHistory)
    {
        return OrderLogic
            .GetOrderssByOrderId(orderHistory.Id)
            .Select(o => ProductLogic.GetProductById(o.ProductID))
            .ToList();        
    }
    public static List<(ProductModel Product, int Quantity, double UnitPrice)> GetReturnableProductsWithQuantity(OrderHistoryModel orderHistory)
    {
        var orderLines = OrderAccess.GetOrderssByOrderId(orderHistory.Id);

        return orderLines
            .GroupBy(o => o.ProductID)
            .Select(g =>
            {
                var product = ProductLogic.GetProductById(g.Key); // g.Key is the product id

                return (
                    Product:   product,
                    Quantity:  g.Count(),
                    UnitPrice: g.First().Price
                );
            })
            .ToList();
    }
}