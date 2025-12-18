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
        string? user = SessionManager.CurrentUser?.AccountStatus;
        bool includeHidden = user == "Admin" || user == "SuperAdmin";
    
        var products = ProductAccess.GetAllProducts(includeHidden); // get all products
        // if (!products.Any(p => p is ChristmasBoxModel))
        // {
        //     var christmasBoxes = ChristmasBoxLogic.GetAvailableBoxes(); // get xmas boxes
        //     products.AddRange(christmasBoxes); // add boxes to products
        // }
        // return products; 
        var christmasBoxes = ChristmasBoxLogic.GetAvailableBoxes(); // get xmas boxes
        products.AddRange(christmasBoxes); // add boxes to products
        
        return products;
    }
    public static void UpdateStock(int productId, int incomingQuantity)
    {
        var currentQuantity = ProductAccess.GetProductQuantityByID(productId);
        var newQuantity = currentQuantity + incomingQuantity;

        if (newQuantity < 0)
            newQuantity = 0;

        ProductAccess.UpdateProductStock(productId, newQuantity);
    }

    public static (ProductModel?, double) GetProductWithCompetitorPrice(int id)
    {
        var product = ProductAccess.GetProductByID(id);
        if (product == null)
        {
            return (null, 0);
        }

        var competitorPrice = ProductAccess.GetCompetitorPriceByID(id);
        return (product, competitorPrice);
    }

    public static List<ProductModel> GetOverpricedProducts(List<ProductModel> products)
    {
        return products.Where(p => p.Price > p.CompetitorPrice).ToList();
    }

    public static void LowerPriceForOverpricedProduct(ProductModel product, double newPrice)
    {
        product.Price = newPrice;
        ProductAccess.ChangeProductDetails(product);
    }
    // the product logic to make the admin make product ellible for the christmas box 
    public static void UpdateProduct(ProductModel product)
    {
       ProductAccess.ChangeProductDetails(product);
    }

}