using Dapper;
using Microsoft.Data.Sqlite;

public static class RewardItemsAccess
{
    private const string ConnectionString = "Data Source=database.db";

    public static void AddRewardItem(RewardItemsModel RewardItem)
    {
        using var db = new SqliteConnection(ConnectionString);
        db.Execute(@"INSERT INTO RewardItems 
            (ProductId, PriceInPoints)
            VALUES (@ProductId, @PriceInPoints)", RewardItem);
    }

    public static List<RewardItemsModel> GetAllRewardItems()
    {
        using var db = new SqliteConnection(ConnectionString);
        return db.Query<RewardItemsModel>("SELECT * FROM RewardItems").ToList();
    }

    public static RewardProductDTO? GetRewardItemByProductId(int productId)
    {
        using var db = new SqliteConnection(ConnectionString);
        var rewardItem = db.QuerySingleOrDefault<RewardItemsModel>(
            "SELECT * FROM RewardItems WHERE ProductId = @ProductId",
            new { ProductId = productId });

        if (rewardItem == null)
        {
            return null;
        }

        var product = ProductAccess.GetProductByID(productId);
        if (product == null)
        {
            return null;
        }

        return new RewardProductDTO(product, rewardItem.PriceInPoints);
    }
}