using Spectre.Console;

public class ProductLogic
{
    public static void ChangeProductDetails(int id, string name, double price, string nutritionDetails, string description, string category, int location, int quantity, int visible)
    {
        ProductModel NewProduct = new ProductModel(id, name, price, nutritionDetails, description, category, location, quantity, visible);
        ProductAccess.ChangeProductDetails(NewProduct);
        return;
    }

    public static void DeleteProductByID(int id)
    {
        ProductAccess.DeleteProductByID(id);
    }

    public static bool AddProduct(string name, double price, string nutritionDetails, string description, string category, int location, int quantity, int visible)
    {
        ProductModel NewProduct = new ProductModel(name, price, nutritionDetails, description, category, location, quantity, visible);
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
        string? user = SessionManager.CurrentUser?.AccountStatus;
        if (user == "Admin" || user == "SuperAdmin")
        {
            return ProductAccess.SearchProductByName(name, true);
        }
        return ProductAccess.SearchProductByName(name);
    }

    public static ProductModel GetProductByName(string name)
    {
        return ProductAccess.GetProductByName(name)!;
    }

    public static ProductModel GetProductById(int id)
    {
        return ProductAccess.GetProductByID(id)!;
    }
    public static List<ProductModel> GetAllProducts()
    {
        return ProductAccess.GetAllProducts();
    }

    public static ProductModel GetProductByID(int productID)
    {
        return ProductAccess.GetProductByID(productID)!;
    }
}


