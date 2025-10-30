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
            if (newQuantity > 99)
            {
                newQuantity = 99; // max stock limit
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


    // add to order history after checkout
    public static void AddToOrderHistory(List<CartModel> cartProducts)
    {
        foreach (var cartProduct in cartProducts)
        {
            //OrderHistoryAccess.AddToOrderHistory(SessionManager.CurrentUser.ID, cartProduct.ProductId, cartProduct.Quantity, DateTime.Now);
        }
    }

    // Get all order history for current user
    public static List<OrderHistoryModel> GetOrderHistory()
    {
        //return OrderHistoryAccess.GetOrderHistoryByUserID(SessionManager.CurrentUser.ID);
        return new List<OrderHistoryModel>();
    }

    // after checkout add all cart items to itemOrders
    public static void AddToItemOrders(List<CartModel> cartProducts, List<ProductModel> allProducts)
    {
        foreach (var cartProduct in cartProducts)
        {
            var product = allProducts.FirstOrDefault(p => p.ID == cartProduct.ProductId);
            if (product != null)
            {
                OrderItemsAccess.AddToOrderItems(SessionManager.CurrentUser.ID, product.ID, cartProduct.Quantity, product.Price);
            }
        }
    }

    // turn order itmes into list of order item models
    public static List<OrderItemModel> GetOrderItemsForCurrentUser()
    {
        return OrderItemsAccess.GetOrderItemsByUserId(SessionManager.CurrentUser.ID);
    }

}
