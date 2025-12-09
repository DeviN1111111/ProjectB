using Dapper;
using Microsoft.Data.Sqlite;

public static class RewardItemsAccess
{
    private static readonly IDatabaseFactory _sqlLiteConnection = new SqliteDatabaseFactory("Data Source=database.db");
    private static readonly SqliteConnection _connection = _sqlLiteConnection.GetConnection();

    public static void AddRewardItem(RewardItemsModel RewardItem)
    {
        _connection.Execute(@"INSERT INTO RewardItems 
            (ProductId, PriceInPoints)
            VALUES (@ProductId, @PriceInPoints)", RewardItem);
    }

    public static List<RewardItemsModel> GetAllRewardItems()
    {
        return _connection.Query<RewardItemsModel>("SELECT * FROM RewardItems").ToList();
    }

    public static RewardProductDTO? GetRewardItemByProductId(int productId)
    {
        var rewardItem = _connection.QuerySingleOrDefault<RewardItemsModel>(
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