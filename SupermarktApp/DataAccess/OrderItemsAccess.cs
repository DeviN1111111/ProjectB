using Dapper;
using Microsoft.Data.Sqlite;
public static class OrderItemsAccess
{
    private const string ConnectionString = "Data Source=database.db";
    public static void AddToOrderItems(int orderId, int productId, int quantity, double price)
    {
    using var db = new SqliteConnection(ConnectionString);

    db.Execute(@"
        INSERT INTO OrderItem (OrderId, ProductId, Quantity, Price)
        VALUES (@OrderId, @ProductId, @Quantity, @Price)
        ON CONFLICT(OrderId, ProductId)
        DO UPDATE SET
            Quantity = excluded.Quantity,
            Price = excluded.Price;",
        new { OrderId = orderId, ProductId = productId, Quantity = quantity, Price = price }
    );
    }

    public static List<OrderItemModel> GetOrderItemsByOrderId(int orderId)
    {
        using var connection = new SqliteConnection(ConnectionString);
        var query = @"
            SELECT * FROM OrderItem
            WHERE OrderId = @OrderId;
        ";
        return connection.Query<OrderItemModel>(query, new { OrderId = orderId }).AsList();
    }
    public static List<ProductModel> GetTop5MostBoughtProducts(int userId)
    {
        using var db = new SqliteConnection(ConnectionString);

        var productCounts = db.Query<(int ProductId, int Count)>(@"
            SELECT ProductID, COUNT(ProductID) AS Count
            FROM Orders
            WHERE UserID = @UserId
            GROUP BY ProductID
            ORDER BY Count DESC;",
            new { UserId = userId }).AsList();

        var topProducts = new List<ProductModel>();

        foreach (var item in productCounts)
        {
            var discount = db.QueryFirstOrDefault<DiscountsModel>(
                "SELECT * FROM Discounts WHERE ProductId = @ProductId",
                new { ProductId = item.ProductId }
            );
            var checkReward = RewardItemsAccess.GetRewardItemByProductId(item.ProductId);

            if(discount != null && discount.DiscountType != "Weekly") // so if its NOT already a discount item unless its Personal (You have have multiple personal discount items for multiple users)
            {
                var product = db.QueryFirstOrDefault<ProductModel>(
                "SELECT * FROM Products WHERE Id = @Id",
                new { Id = item.ProductId });
                if (product != null)
                {
                    topProducts.Add(product);
                }
                if (topProducts.Count == 5)
                {
                    return topProducts;
                }
            }
            else if (discount == null && checkReward == null) // so if its NOT already a discount or reward item
            {
                var product = db.QueryFirstOrDefault<ProductModel>(
                "SELECT * FROM Products WHERE Id = @Id",
                new { Id = item.ProductId });
                if (product != null)
                {
                    topProducts.Add(product);
                }
                if (topProducts.Count == 5)
                {
                    return topProducts;
                }
            }
        }

        return topProducts;
    }


    
}