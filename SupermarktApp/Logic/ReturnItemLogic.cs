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
    public static List<(ProductModel Product, int Quantity, double UnitPrice)> GetReturnableProductsWithQuantity(List<OrdersModel> orderLines)
    {
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
    public static void RemoveProductQuantityFromOrder(int orderId, int productId, int quantity)
    {
        OrderAccess.RemoveProductQuantityFromOrder(orderId, productId, quantity);
    }
}