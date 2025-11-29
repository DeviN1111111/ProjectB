using System.Text.Json;
static class FavoriteListLogic
{
    public static List<FavoriteListModel> GetAllListsByUserId(int userId)
    {
        return FavoriteListAccess.GetAllFavoriteListsByUserId(userId);
    }
    public static int GetProductQuantity(List<ProductModel> products, int productId)
    {
        return products.Count(p => p.ID == productId);
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
    public static void EditProductQuantity(
        Dictionary<ProductModel, int> originalProducts,
        Dictionary<ProductModel, int> updatedProducts,
        int listId)
    {
        var merged = new Dictionary<ProductModel, int>(originalProducts);

        foreach (var kvp in updatedProducts)
        {
            merged[kvp.Key] = kvp.Value; 
        }

        FavoriteListAccess.EditProductQuantity(merged, listId);
    }
    public static void ChangeListName(int listId, string newName)
    {
        FavoriteListAccess.ChangeListName(listId, newName);
    }
    public static void CreateList(FavoriteListModel list)
    {
        var id = FavoriteListAccess.CreateList(list.Name, list.UserId);
        list.Id = id;
    }
}