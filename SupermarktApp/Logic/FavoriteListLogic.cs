static class FavoriteListLogic
{
    public static List<FavoriteListModel> GetAllListsByUserId(int userId)
    {
        return FavoriteListAccess.GetAllFavoriteListsByUserId(userId);
    }
}