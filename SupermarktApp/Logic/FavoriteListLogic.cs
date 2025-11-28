using System.Text.Json;
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
    public static void AddProductToList(ProductModel product, int quantity, int listId)
    {
        // Load JSON
        string? json = FavoriteListAccess.GetProductsJson(listId);

        // Convert to dictionary
        var products = string.IsNullOrEmpty(json) ?
            new Dictionary<int, int>():
            JsonSerializer.Deserialize<Dictionary<int, int>>(json)!;

        // Add or update
        products[product.ID] = quantity;

        // Serialize
        string updatedJson = JsonSerializer.Serialize(products);

        // Pass FINAL JSON to DAL
        FavoriteListAccess.AddProductToList(listId, updatedJson);
    }
    public static void RemoveProductFromList(ProductModel product, int listId)
    {
        string? json = FavoriteListAccess.GetProductsJson(listId);

        if (string.IsNullOrEmpty(json))
            return;

        var products = JsonSerializer.Deserialize<Dictionary<int, int>>(json)!;

        products.Remove(product.ID);

        string updatedJson = JsonSerializer.Serialize(products);

        FavoriteListAccess.RemoveProductFromList(listId, updatedJson);
    }
}