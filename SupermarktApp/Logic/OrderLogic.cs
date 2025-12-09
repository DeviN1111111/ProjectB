using Spectre.Console;

public class OrderLogic
{
    public static void AddToCart(ProductModel product, int quantity, double discount = 0, double RewardPrice = 0)
    {
        // check if product already in cart
        List<CartModel> allUserProducts = CartAccess.GetAllUserProducts(SessionManager.CurrentUser!.ID);
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
            discount = CartItem.Discount + discount;
            CartAccess.AddToCart(SessionManager.CurrentUser.ID, product.ID, newQuantity, RewardPrice);
            return;
        }
        // add new item to cart
        CartAccess.AddToCart(SessionManager.CurrentUser.ID, product.ID, quantity, RewardPrice);
    }
       public static string AddBirthdayGiftToCart(UserModel user)
        {
            if (user == null || SessionManager.CurrentUser == null)
            {
                return "Error: No logged-in user found.";
            }

            // Find the birthday gift (case-insensitive)
            var giftProduct = ProductAccess.GetAllProducts()
                .FirstOrDefault(p => p.Name.Equals("Birthday Present", StringComparison.OrdinalIgnoreCase));

            if (giftProduct == null)
            {
                return "No 'Birthday Present' product found in the database.";
            }

            // Ensure we're checking the current user's cart
            var userCart = CartAccess.GetAllUserProducts(user.ID);
            var existingItem = userCart.FirstOrDefault(p => p.ProductId == giftProduct.ID);

            if (existingItem != null)
            {
                return "Birthday present already in your cart.";
            }

        // Add the gift to the cart
        AddToCart(giftProduct, 1, 0, 0);
            return $" Happy Birthday, {user.Name}! A free '{giftProduct.Name}' has been added to your cart!";
        }


    public static List<CartModel> AllUserProducts()
    {
        List<CartModel> allUserProducts = CartAccess.GetAllUserProducts(SessionManager.CurrentUser!.ID);
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
                    if (Product.Quantity - cartProduct.Quantity < 0)
                    {
                        AnsiConsole.MarkupLine($"[red]Error: Not enough stock for product '{Product.Name}'.[/]");
                        Console.ReadKey();
                        continue;
                    }
                    int newStock = Product.Quantity - cartProduct.Quantity;
                    ProductAccess.UpdateProductStock(Product.ID, newStock);
                }
            }
        }
    }
    public static void ChangeQuantity(int productId, int newQuantity)
    {
        CartAccess.UpdateProductQuantity(SessionManager.CurrentUser!.ID, productId, newQuantity);
    }

    // remove a product from cart by product id
    public static void RemoveFromCart(int productId)
    {
        double rewardPrice = CartAccess.GetUserProductByProductId(SessionManager.CurrentUser!.ID, productId)!.RewardPrice;
        string rewardItemName = ProductAccess.GetProductByID(productId)!.Name;
        if(rewardPrice > 0)
        {
            SessionManager.CurrentUser.AccountPoints += (int)rewardPrice;
            RewardLogic.ChangeRewardPoints(SessionManager.CurrentUser!.ID, SessionManager.CurrentUser.AccountPoints);
            AnsiConsole.MarkupLine($"[white]You have been refunded [/][green]{rewardPrice}[/] reward points for [yellow1]{rewardItemName}[/]!");
        }
        CartAccess.RemoveFromCart(SessionManager.CurrentUser!.ID, productId);
    }
    public static double CalculateTotalDiscount()
    {
        if (SessionManager.CurrentUser == null) return 0;
        var allUserProducts = CartAccess.GetAllUserProducts(SessionManager.CurrentUser.ID);
        double totalDiscount = 0;
        foreach (var item in allUserProducts)
        {
            totalDiscount += item.Discount;
        }
        return Math.Round(totalDiscount, 2);
    }
    //create a new order for the current user.
    public static void AddOrderWithItems(List<OrdersModel> cartProducts, List<ProductModel> allProducts)
    {
        // Create a new order and get its ID
        int orderId = OrderHistoryAccess.AddToOrderHistory(SessionManager.CurrentUser!.ID);
        foreach (var cartProduct in cartProducts)
        {
            // Find the matching product details
            var matchingProduct = allProducts.FirstOrDefault(matchingProduct => matchingProduct.ID == cartProduct.ProductID);
            var RewardItem = RewardItemsAccess.GetRewardItemByProductId(matchingProduct!.ID);
            if (matchingProduct != null)
            {
                // Add each item to OrderItems with the new orderId
                if (RewardItem != null)
                {
                    OrderAccess.AddToOrders(SessionManager.CurrentUser.ID, orderId, matchingProduct.ID, 0.0);
                }
                else
                    OrderAccess.AddToOrders(SessionManager.CurrentUser.ID, orderId, matchingProduct.ID, matchingProduct.Price);
            }
        }
    }
    public static List<OrdersModel> GetOrderssByOrderId(int orderId)
    {
        return OrderAccess.GetOrderssByOrderId(orderId);
    }
    public static OrderHistoryModel GetOrderByUserId(int userId) => OrderHistoryAccess.GetOrderByUserId(userId);
    public static void ProcessPay(List<CartModel> cartProducts, List<ProductModel> allProducts, int? selectedCouponId)
    {
        List<OrdersModel> allOrderItems = new List<OrdersModel>();

        foreach (var item in cartProducts)
        {
            var product = allProducts.FirstOrDefault(p => p.ID == item.ProductId);
            if (product == null)
                continue;

            // Create OrdersModel per quantity
            for (int i = 0; i < item.Quantity; i++)
            {
                allOrderItems.Add(new OrdersModel
                {
                    UserID = SessionManager.CurrentUser!.ID,
                    ProductID = product.ID,
                    Price = product.Price
                });
            }
        }

        // Save the order to the database
        if (allOrderItems.Count > 0)
        {
            OrderLogic.AddOrderWithItems(allOrderItems, allProducts);
        }

        // Apply coupon if selected
        if (selectedCouponId.HasValue)
        {
            CouponLogic.UseCoupon(selectedCouponId.Value);
            CouponLogic.ResetCouponSelection();
        }

        //  clean up
        OrderLogic.UpdateStock();
        OrderLogic.ClearCart();
    }
public static (List<string> OutOfStock, List<string> Unavailable) ReorderPastOrder(int orderHistoryId)
{
    var pastOrderItems = OrderAccess.GetOrderssByOrderId(orderHistoryId);

    var outOfStockProducts = new List<string>();
    var unavailableProducts = new List<string>();

    // GROUP items by product ID
    var grouped = pastOrderItems
        .GroupBy(i => i.ProductID)
        .Select(g => new { ProductID = g.Key, QuantityNeeded = g.Count() });

    foreach (var group in grouped)
    {
        var product = ProductAccess.GetProductByID(group.ProductID);
        if (product == null)
            continue;

        // Hidden/unavailable product
        if (product.Visible == 0)
        {
            unavailableProducts.Add(product.Name);
            continue;
        }

        // Check stock
        int stock = ProductAccess.GetProductQuantityByID(product.ID);
        int needed = group.QuantityNeeded;

        // Nothing in stock
        if (stock <= 0)
        {
            outOfStockProducts.Add($"{product.Name} (0 in stock)");
            continue;
        }

        // Partial stock situation
        if (stock < needed)
        {
            AddToCart(product, stock);

            outOfStockProducts.Add(
                $"{product.Name} — needed {needed}, only {stock} added"
            );

            continue;
        }

        // Full stock available
        AddToCart(product, needed);
    }

    return (outOfStockProducts, unavailableProducts);
}

    public static List<OrderHistoryModel> GetAllUserOrders(int userId)
    {
        List<OrderHistoryModel> allOrders = OrderHistoryAccess.GetAllUserOrders(userId);
        return allOrders;
    }

// CheckStockBeforeCheckout using tuple

    public static List<string> CheckStockBeforeCheckout(List<CartModel> cartProducts, List<ProductModel> allProducts)
    {
        List<string> outOfStockProducts = new List<string>();
        foreach (var cartItem in cartProducts)
        {
            var product = ProductAccess.GetProductByID(cartItem.ProductId);
            if (product != null)
            {
                int availableStock = ProductAccess.GetProductQuantityByID(product.ID);
                
                if (availableStock < cartItem.Quantity)
                {
                    outOfStockProducts.Add(product.Name);
                }
            }
        }
        return outOfStockProducts;
    }

    public static void RemoveAllProductsFromOrder(int orderId)
    {
        OrderAccess.RemoveAllProductsFromOrder(orderId);
    } 
    public static void DeleteOrderHistory(int orderId)
    {
        OrderHistoryAccess.DeleteOrderHistory(orderId);
    }
}
