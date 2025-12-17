public static class ReturnItemLogic
{
    public static List<OrderHistoryModel> CheckReturnableOrders(
        List<OrderHistoryModel> orders,
        DateTime today)
    {
        return orders
            .Where(o => (today - o.Date.Date).TotalDays <= 10 && o.IsPaid)
            .ToList();
    }
    public static List<ProductModel> CheckReturnableProducts(OrderHistoryModel orderHistory)
    {
        return OrderLogic
            .GetOrderItemsByOrderId(orderHistory.Id)
            .Select(o => ProductLogic.GetProductById(o.ProductID))
            .ToList();        
    }
    public static List<(ProductModel Product, int Quantity, double UnitPrice)> GetReturnableProductsWithQuantity(List<OrderItemsModel> orderLines)
    {
        return orderLines
            .GroupBy(o => o.ProductID)
            .Select(g =>
            {
                var product = ProductLogic.GetProductById(g.Key); // g.Key is the product id
                double basePrice = product.Price;
                var productDiscount = DiscountsLogic.CheckDiscountByProduct(product);
                double unitPrice = basePrice;

                if (productDiscount != null &&
                    productDiscount.Discount.DiscountPercentage > 0)
                {
                    var discount = productDiscount.Discount.DiscountPercentage;
                    unitPrice = Math.Round(basePrice * (1 - discount / 100.0), 2);
                }

                return (
                    Product:   product,
                    Quantity:  g.Count(),
                    UnitPrice: unitPrice
                );
            })
            .ToList();
    }
    public static void RemoveProductQuantityFromOrder(int orderId, int productId, int quantity)
    {
        OrderItemAccess.RemoveProductQuantityFromOrder(orderId, productId, quantity);
    }
}