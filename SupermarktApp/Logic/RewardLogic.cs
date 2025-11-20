public static class RewardLogic
{
    public static bool AddRewardPointsToUser(int points)
    {
        if (SessionManager.CurrentUser == null)
        {
            return false;
        }

        SessionManager.CurrentUser.AccountPoints += points;

        if (SessionManager.CurrentUser.AccountPoints < 0)
        {
            SessionManager.CurrentUser.AccountPoints = 0;
        }

        UserAccess.UpdateUserPoints(SessionManager.CurrentUser.ID, SessionManager.CurrentUser.AccountPoints);
        return true;
    }

    public static int CalculateRewardPoints(double amountSpent)
    {
        if (amountSpent <= 0) return 0;
        return (int)Math.Floor(amountSpent / 10.0);
    }

    public static List<RewardProductDTO> GetAllRewardItems()
    {
        var rewardItems = RewardItemsAccess.GetAllRewardItems();
        var products = new List<RewardProductDTO>();

        foreach (var item in rewardItems)
        {
            var product = ProductLogic.GetProductById(item.ProductId);
            if (product != null)
            {
                products.Add(new RewardProductDTO(product, item.PriceInPoints));
            }
        }

        return products;
    }

    public static void ChangeRewardPoints(int userId, int newPoints)
    {
        UserAccess.UpdateUserPoints(userId, newPoints);
    }
    
    public static RewardProductDTO? GetRewardItemByProductId(int productId)
    {
        return RewardItemsAccess.GetRewardItemByProductId(productId);
    }
}