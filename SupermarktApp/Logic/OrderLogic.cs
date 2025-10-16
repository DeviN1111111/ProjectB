public class OrderLogic
{
    public static void AddToCart(ProductModel product, int quantity)
    {
        // check if product already in cart
        
        List<CartModel> allUserProducts = CartAccess.GetAllUserProducts(SessionManager.CurrentUser.ID);
        var CartItem = allUserProducts.FirstOrDefault(item => item.ProductId == product.ID);
        if (CartItem != null)
        {
            int newQuantity = CartItem.Quantity + quantity;
            if (newQuantity > 20)
            {
                newQuantity = 20; // max stock limit
            }
            CartAccess.RemoveFromCart(SessionManager.CurrentUser.ID, product.ID);
            CartAccess.AddToCart(SessionManager.CurrentUser.ID, product.ID, newQuantity);
            return;
        }
        // add new item to cart
        CartAccess.AddToCart(SessionManager.CurrentUser.ID, product.ID, quantity);
    }

    public static List<CartModel> AllUserProducts()
    {
        List<CartModel> allUserProducts = CartAccess.GetAllUserProducts(SessionManager.CurrentUser.ID);
        return allUserProducts;
    }

    public static void ClearCart()
    {
        CartAccess.ClearCart();
    }
    public static double DeliveryFee(double totalAmount)
    {
        double deliveryFee = 0;
        if (totalAmount >= 25)
        {
            deliveryFee = 0;
            return deliveryFee;
        }
        else if (totalAmount < 25 && totalAmount > 0)
        {
            deliveryFee = 5;
            return deliveryFee;
        }
        else
        {
            return 0;
        }
    }

    public static void UpdateStock()
    {
        List<CartModel> allUserProducts = OrderLogic.AllUserProducts();  // List of user Products in cart
        List<ProductModel> allProducts = ProductAccess.GetAllProducts();
        foreach (var cartProduct in allUserProducts)
        {
            // Get Product id and find match in all products
            foreach (ProductModel Product in allProducts)
            {
                if (cartProduct.ProductId == Product.ID)
                {
                    int newStock = Product.Quantity - cartProduct.Quantity;
                    ProductAccess.UpdateProductStock(Product.ID, newStock);
                }
            }
        }
    }
    
    // remove a product from cart by product id
    public static void RemoveFromCart(int productId)
    {
        CartAccess.RemoveFromCart(SessionManager.CurrentUser.ID, productId);
    }
}