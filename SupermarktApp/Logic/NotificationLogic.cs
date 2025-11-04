using System.Reflection.Metadata.Ecma335;

public class NotificationLogic
{
    public static List<ProductModel> GetAllLowQuantityProducts(int QuantityThreshold = 50)
    {
        List<ProductModel> AllProducts = ProductAccess.GetAllProducts();

        List<ProductModel> LowQuantityProducts = [];

        foreach (ProductModel product in AllProducts)
        {
            if (product.Quantity < QuantityThreshold)
            {
                LowQuantityProducts.Add(product);
            }
        }

        return LowQuantityProducts;
    }

    public static double FillProductQuantity(int productId, int quantityToAdd)
    {
        ProductModel? product = ProductAccess.GetProductByID(productId);
        int newQuantity = product.Quantity + quantityToAdd;
        ProductAccess.UpdateProductStock(productId, newQuantity);
        return product.Price * quantityToAdd;
    }
}