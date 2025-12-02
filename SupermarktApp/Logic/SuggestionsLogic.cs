public static class SuggestionsLogic
{
    public static List<ProductModel> GetSuggestedItems(int userId)
    {
        var suggestedProducts = OrderAccess.GetMostBoughtProducts(userId, 9);

        return suggestedProducts;
    }

}
