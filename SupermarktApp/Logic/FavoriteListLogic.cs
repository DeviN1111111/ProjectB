static class FavoriteListLogic
{
    public static List<FavoriteListModel> GetAllListsByUserId(int userId)
    {
        var lists = FavoriteListAccess.GetAllFavoriteListsByUserId(userId);

        foreach (var list in lists)
        {
            var products = FavoriteListAccess.GetProductsByListId(list.Id);

            foreach (var p in products)
            {
                p.Product = ProductLogic.GetProductById(p.ProductId);
            }

            list.Products = products;
        }

        return lists;
    }
    public static int GetProductQuantity(
        List<FavoriteListProductModel> products,
        int productId)
    {
        return products
            .FirstOrDefault(p => p.ProductId == productId)?
            .Quantity
            ?? 0;
    }
    public static void AddProductToList(ProductModel product, int quantity, int listId)
    {
        FavoriteListAccess.AddProductToList(listId, product.ID, quantity);
    }

    public static void RemoveProductFromList(ProductModel product, int listId)
    {
        FavoriteListAccess.RemoveProductFromList(listId, product.ID);
    }

    public static void EditProductQuantity(
        FavoriteListModel list,
        List<FavoriteListProductModel> updatedProducts)
    {
        FavoriteListAccess.EditProductQuantity(updatedProducts, list.Id);
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
    public static void RemoveList(int listId)
    {
        FavoriteListAccess.RemoveList(listId);
    }
}