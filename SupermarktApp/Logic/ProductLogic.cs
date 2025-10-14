using Spectre.Console;

public class ProductLogic
{
    public static void ChangeProductDetails(int id, string name, double price, string nutritionDetails, string description, string category, int location, int quantity)
    {
        ProductModel NewProduct = new ProductModel(id, name, price, nutritionDetails, description, category, location, quantity);
        ProductAccess.ChangeProductDetails(NewProduct);
        return;
    }

    public static void DeleteProductByID(int id)
    {
        ProductAccess.DeleteProductByID(id);
    }

    public static bool AddProduct(string name, double price, string nutritionDetails, string description, string category, int location, int quantity)
    {
        ProductModel NewProduct = new ProductModel(name, price, nutritionDetails, description, category, location, quantity);
        ProductModel? ExistingProductCheck = ProductAccess.GetProductByName(NewProduct.Name);
        if (ExistingProductCheck == null)
        {
            ProductAccess.AddProduct(NewProduct);
            return true;
        }
        else
            return false;
    }

    public static List<ProductModel> SearchProductByName(string name)
    {
        return ProductAccess.SearchProductByName(name);
    }

    public static ProductModel GetProductByName(string name)
    {
        return ProductAccess.GetProductByName(name);
    }
}