static class FavoriteListLogic
{
    public static List<FavoriteListModel> GetAllListsByUserId(int userId)
    {
        return FavoriteListAccess.GetAllFavoriteListsByUserId(userId);
    }
    public static int GetProductQuantity(List<ProductModel> products, int id)
    {
        return products.Count(p => p.ID == id);
    }
}