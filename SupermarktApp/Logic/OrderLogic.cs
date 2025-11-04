using Spectre.Console;

public class OrderLogic
{
    public static void AddToCart(ProductModel product, int quantity, double discount = 0, double RewardPrice = 0)
    {
        // check if product already in cart
        WeeklyPromotionsModel WeeklyDiscountProduct = ProductLogic.GetProductByIDinWeeklyPromotions(product.ID);
        if(WeeklyDiscountProduct != null)
        {
            discount = WeeklyDiscountProduct.Discount;
        }

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
            RewardPrice = CartItem.RewardPrice + RewardPrice;
            discount = CartItem.Discount;
            CartAccess.AddToCart(SessionManager.CurrentUser.ID, product.ID, newQuantity, discount, RewardPrice);
            return;
        }
        // add new item to cart
        CartAccess.AddToCart(SessionManager.CurrentUser.ID, product.ID, quantity, discount, RewardPrice);
    }

    public static List<CartModel> AllUserProducts()
    {
        List<CartModel> allUserProducts = CartAccess.GetAllUserProducts(SessionManager.CurrentUser.ID);
        return allUserProducts;
    }

    public static void ClearCart()
    {
        foreach (var item in AllUserProducts())
        {
            if(item.RewardPrice > 0)
            {
                RewardLogic.ChangeRewardPoints(SessionManager.CurrentUser.ID, SessionManager.CurrentUser.AccountPoints + (int)(item.RewardPrice * item.Quantity));
                SessionManager.CurrentUser.AccountPoints += (int)(item.RewardPrice * item.Quantity);
            }
        }
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
        double rewardPrice = CartAccess.GetUserProductByProductId(SessionManager.CurrentUser.ID, productId).RewardPrice;
        string rewardItemName = ProductAccess.GetProductByID(productId).Name;
        if(rewardPrice > 0)
        {
            SessionManager.CurrentUser.AccountPoints += (int)rewardPrice;
            RewardLogic.ChangeRewardPoints(SessionManager.CurrentUser.ID, SessionManager.CurrentUser.AccountPoints);
            AnsiConsole.MarkupLine($"[white]You have been refunded [/][green]{rewardPrice}[/] reward points for [yellow1]{rewardItemName}[/]!");
        }
        CartAccess.RemoveFromCart(SessionManager.CurrentUser.ID, productId);
    }
    public static double CalculateTotalDiscount()
    {
        if (SessionManager.CurrentUser == null) return 0;
        var allUserProducts = CartAccess.GetAllUserProducts(SessionManager.CurrentUser.ID);
        double totalDiscount = 0;
        foreach (var item in allUserProducts)
        {
            if (item.RewardPrice == 0)
            {
                totalDiscount += item.Discount * item.Quantity;
            }
        }
        return Math.Round(totalDiscount, 2);
    }
    
    //create a new order for the current user.
    public static void AddOrderWithItems(List<OrderItemModel> cartProducts, List<ProductModel> allProducts)
    {
        // Create a new order and get its ID
        int orderId = OrderAccess.AddToOrderHistory(SessionManager.CurrentUser.ID);
        foreach (var cartProduct in cartProducts)
        {
            // Find the matching product details
            var matchingProduct = allProducts.FirstOrDefault(matchingProduct => matchingProduct.ID == cartProduct.ProductId);
            if (matchingProduct != null)
            {
                // Add each item to OrderItems with the new orderId
                OrderItemsAccess.AddToOrderItems(orderId, matchingProduct.ID, cartProduct.Quantity, matchingProduct.Price);
            }
        }
    }
}



