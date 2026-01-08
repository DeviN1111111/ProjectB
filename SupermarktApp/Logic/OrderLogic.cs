using System.Data.Common;
using Spectre.Console;

public class OrderLogic
{
    public static int userId = SessionManager.CurrentUser?.ID ?? 0;
    public static void AddToCartProduct(ProductModel product, int quantity, double discount = 0, double RewardPrice = 0)
    {
        // Christmas box restrictions
        if (product.Category == "ChristmasBox") 
        {   
            var box = product as ChristmasBoxModel ?? ChristmasBoxLogic.CreateBox(product);

            if (box.Products == null || box.Products.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]This Christmas box is not available yet.[/]");
                return;
            }
            product = box;

            // check if bought already
            if (OrderItemAccess.HasUserPurchasedProduct(SessionManager.CurrentUser!.ID, product.ID))
            {
                AnsiConsole.MarkupLine("[yellow]You already purchased this Christmas box size before.[/]");
                return;
            }
            // cant add to cart
            var cartItems = CartProductAccess.GetAllUserProducts(SessionManager.CurrentUser!.ID);

            if (cartItems.Any(cp => cp.ProductId == product.ID))
            {
                AnsiConsole.MarkupLine(
                    "[yellow]You can only buy one Christmas box per size.[/]"
                );

                return;
            }
            quantity = 1;
        }
        // Console.WriteLine($"DEBUG add to cart: {product.Name}, ID = {product.ID}, Qty = {quantity}"); //// DEBUGGUGUGGU

        // check if product already in CartProduct
        List<CartProductModel> items = CartProductAccess.GetAllUserProducts(userId);
        var existing = items.FirstOrDefault(item => item.ProductId == product.ID);
        if (existing != null)
        {
            int finalQuantity = Math.Min(existing.Quantity + quantity, 99);

            CartProductAccess.RemoveFromCartProduct(userId, product.ID);
            RewardPrice = existing.RewardPrice + RewardPrice;
            discount = existing.Discount + discount;
            CartProductAccess.AddToCartProduct(userId, product.ID, finalQuantity, RewardPrice);
            return;
        }
        // add new item to CartProduct
        CartProductAccess.AddToCartProduct(userId, product.ID, quantity, RewardPrice);
    }

    public static List<CartDisplayItemDTO> GetCartDisplayItems()
    {
        var cartItems = CartProductAccess.GetAllUserProducts(userId);
        var products = ProductLogic.GetAllProducts();

        var result = new List<CartDisplayItemDTO>();

        foreach (var cartItem in cartItems)
        {
            var product = products.First(p => p.ID == cartItem.ProductId);

            // Xmas box
            if (product is ChristmasBoxModel box)
            {
                double totalBoxValue = box.Price * cartItem.Quantity;

                result.Add(new CartDisplayItemDTO
                {
                    ProductId = box.ID,
                    Name = box.Name,
                    Quantity = cartItem.Quantity,
                    PriceText = $"€{box.Price:0.00}",
                    TotalText = $"€{totalBoxValue:0.00}",

                    TotalValue = totalBoxValue,
                    DiscountValue = 0,
                    Contents = box.Products.Select(p => p.Name).ToList()
                });

                continue;
            }

            // Normal product
            double totalValue = product.Price * cartItem.Quantity;            

            result.Add(new CartDisplayItemDTO
            {
                ProductId = product.ID,
                Name = product.Name,
                Quantity = cartItem.Quantity,
                PriceText = $"€{product.Price:0.00}",
                TotalText = $"€{totalValue:0.00}",          

                TotalValue = totalValue,
                DiscountValue = 0
            });
        }

        return result;
    }

       public static string AddBirthdayGiftToCartProduct(UserModel user)
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

            // Ensure we're checking the current user's CartProduct
            var userCartProduct = CartProductAccess.GetAllUserProducts(user.ID);
            var existingItem = userCartProduct.FirstOrDefault(p => p.ProductId == giftProduct.ID);

            if (existingItem != null)
            {
                return "Birthday present already in your CartProduct.";
            }

        // Add the gift to the CartProduct
        AddToCartProduct(giftProduct, 1, 0, 0);
            return $" Happy Birthday, {user.Name}! A free '{giftProduct.Name}' has been added to your Cart!";
        }


    public static List<CartProductModel> AllUserProducts()
    {
        List<CartProductModel> allUserProducts = CartProductAccess.GetAllUserProducts(userId);
        return allUserProducts;
    }

    public static void ClearCartProduct()
    {
        CartProductAccess.ClearCartProduct();
    }
    public static double DeliveryFee(double total)
    {
        if (total < 0 || total >= 25) return 0; 
        return 5.0; // standard delivery fee
    }

    public static void UpdateStock()
    {
        var cartItems = OrderLogic.AllUserProducts(); // current user cart items

        foreach (var cartItem in cartItems)
        {
            var product = ProductAccess.GetProductByID(cartItem.ProductId); 

            if(product == null)
                continue;
            
            if (product.Category == "ChristmasBox") // check if the item is a christmas box
            {
                var box = ChristmasBoxLogic.CreateBox(product);  // box contents

                foreach (var contentItem in box.Products)
                {
                    int available = ProductAccess.GetProductQuantityByID(contentItem.ID); // old stock
                    int newStock = available - cartItem.Quantity; // new stock

                    if (newStock < 0)
                    {
                        AnsiConsole.MarkupLine($"[red]Error: Not enough stock for product '{contentItem.Name}' (Christmas box item).[/]");
                        Console.ReadKey();
                        continue;
                    }
                    ProductAccess.UpdateProductStock(contentItem.ID, newStock); // update
                }
                int boxStock = ProductAccess.GetProductQuantityByID(product.ID);
                int newBoxStock = boxStock - cartItem.Quantity;
                
                if (newBoxStock < 0)
                {
                    AnsiConsole.MarkupLine($"[red]Error: Not enough stock for '{product.Name}'.[/]");
                    Console.ReadKey(true);
                    continue;
                }
                
                ProductAccess.UpdateProductStock(product.ID, newBoxStock);
                continue;
            }


            int stock = ProductAccess.GetProductQuantityByID(product.ID);
            int updated = stock - cartItem.Quantity;

            if (updated < 0)
            {
                AnsiConsole.MarkupLine($"[red]Error: Not enough stock for product '{product.Name}'.[/]");
                Console.ReadKey();
                continue;
            }

            ProductAccess.UpdateProductStock(product.ID, updated);

        }
    }
    public static void ChangeQuantity(int productId, int newQuantity)
    {
        var product = ProductAccess.GetProductByID(productId);
        if (product == null) return;
    
        // Christmas boxes cannot change quantity
        if (product.Category == "ChristmasBox")
        {
            newQuantity = 1;
        }
    
        // Normal product limits
        if (newQuantity < 1) newQuantity = 1;
        if (newQuantity > 99) newQuantity = 99;
    
        CartProductAccess.UpdateProductQuantity(SessionManager.CurrentUser!.ID, productId, newQuantity);
    }
    // remove a product from CartProduct by product id
    public static void RemoveFromCartProduct(int productId)
    {
        double rewardPrice = CartProductAccess.GetUserProductByProductId(userId, productId)!.RewardPrice;
        string rewardItemName = ProductAccess.GetProductByID(productId)!.Name;
        if(rewardPrice > 0)
        {
            SessionManager.CurrentUser.AccountPoints += (int)rewardPrice;
            RewardLogic.ChangeRewardPoints(userId, SessionManager.CurrentUser.AccountPoints);
            AnsiConsole.MarkupLine($"[white]You have been refunded [/][green]{rewardPrice}[/] reward points for [yellow1]{rewardItemName}[/]!");
        }
        CartProductAccess.RemoveFromCartProduct(userId, productId);
    }
    public static double CalculateTotalDiscount()
    {
        if (SessionManager.CurrentUser == null) return 0;
        var allUserProducts = CartProductAccess.GetAllUserProducts(SessionManager.CurrentUser.ID);
        double totalDiscount = 0;
        foreach (var item in allUserProducts)
        {
            totalDiscount += item.Discount;
        }
        return Math.Round(totalDiscount, 2);
    }
    //create a new order for the current user.
    public static void AddOrderWithItems(List<OrderItemsModel> CartProductProducts, List<ProductModel> allProducts)
    {
        // Create a new order and get its ID
        int orderId = OrderHistoryAccess.AddToOrderHistory(userId);
        foreach (var CartProductProduct in CartProductProducts)
        {
            // Find the matching product details
            var matchingProduct = allProducts.FirstOrDefault(matchingProduct => matchingProduct.ID == CartProductProduct.ProductID);
            var RewardItem = RewardItemsAccess.GetRewardItemByProductId(matchingProduct!.ID);
            if (matchingProduct != null)
            {
                // Add each item to OrderItems with the new orderId
                if (RewardItem != null)
                {
                    OrderItemAccess.AddToOrders(SessionManager.CurrentUser.ID, orderId, matchingProduct.ID, 0.0);
                }
                else
                    OrderItemAccess.AddToOrders(SessionManager.CurrentUser.ID, orderId, matchingProduct.ID, matchingProduct.Price);
            }
        }
    }
    public static List<OrderItemsModel> GetOrderItemsByOrderId(int orderId)
    {
        return OrderItemAccess.GetOrderItemsByOrderId(orderId);
    }
    public static OrderHistoryModel GetOrderByUserId(int userId) => OrderHistoryAccess.GetOrderByUserId(userId);
    public static void ProcessPay(List<CartProductModel> CartProductProducts, List<ProductModel> allProducts, int? selectedCouponId)
    {
        List<OrderItemsModel> allOrderItems = new List<OrderItemsModel>();

        foreach (var item in CartProductProducts)
        {
            var product = allProducts.FirstOrDefault(p => p.ID == item.ProductId);
            if (product == null)
                continue;

            // Create OrderItemsModel per quantity
            for (int i = 0; i < item.Quantity; i++)
            {
                allOrderItems.Add(new OrderItemsModel
                {
                    UserID = userId,
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

        //  clean up
        OrderLogic.UpdateStock();
        OrderLogic.ClearCartProduct();
    }
public static (List<string> OutOfStock, List<string> Unavailable) ReorderPastOrder(int orderHistoryId)
{
    var pastOrderItems = OrderItemAccess.GetOrderItemsByOrderId(orderHistoryId);

    var outOfStockProducts = new List<string>();
    var unavailableProducts = new List<string>();

    // group items by product ID
    var grouped = pastOrderItems
        .GroupBy(i => i.ProductID)
        .Select(g => new { ProductID = g.Key, QuantityNeeded = g.Count() });

    foreach (var group in grouped)
    {
        var product = ProductAccess.GetProductByID(group.ProductID);
        if (product == null)
            continue;

        // unavailable product
        if (product.Visible == 0)
        {
            unavailableProducts.Add(product.Name);
            continue;
        }

        // Check stock
        int stock = ProductAccess.GetProductQuantityByID(product.ID);
        int needed = group.QuantityNeeded;

        if (stock <= 0)
        {
            outOfStockProducts.Add($"{product.Name} (0 in stock)");
            continue;
        }

        if (stock < needed)
        {
            AddToCartProduct(product, stock);

            outOfStockProducts.Add(
                $"{product.Name} — needed {needed}, only {stock} added"
            );

            continue;
        }

        // // blockif it is a christmas box that has already been purchased
        if (product.Category == "ChristmasBox" && OrderItemAccess.HasUserPurchasedProduct(SessionManager.CurrentUser!.ID, product.ID))
        {
            unavailableProducts.Add($"{product.Name} (Already purchased before)");
            continue;        
        }

        // Full stock available
        AddToCartProduct(product, needed);
    }

    return (outOfStockProducts, unavailableProducts);
}
    public static List<OrderHistoryModel> GetAllUserOrders(int userId)
    {
        List<OrderHistoryModel> allOrders = OrderHistoryAccess.GetAllUserOrders(userId);
        return allOrders;
    }
// CheckStockBeforeCheckout using tuple
    public static List<string> CheckStockBeforeCheckout(List<CartProductModel> CartProductProducts, List<ProductModel> allProducts)
    {
        var outOfStockProducts = new List<string>();

        foreach (var cartProductItem in CartProductProducts)
        {
            var product = ProductAccess.GetProductByID(cartProductItem.ProductId);
            if (product == null)
                continue;

            if (product.Category == "ChristmasBox")
            {
                var box = ChristmasBoxLogic.CreateBox(product);

                foreach (var contentItem in box.Products)
                {
                    int available = ProductAccess.GetProductQuantityByID(contentItem.ID);

                    if(available < cartProductItem.Quantity)
                    {
                        outOfStockProducts.Add($"{contentItem.Name} (Christmas box item)");
                    }
                }
                continue;
            }
            int availableStock = ProductAccess.GetProductQuantityByID(product.ID);
            
            if (availableStock < cartProductItem.Quantity)
            {
                outOfStockProducts.Add(product.Name);
            }
        }
        return outOfStockProducts;
    }
    public static void RemoveAllProductsFromOrder(int orderId)
    {
        OrderItemAccess.RemoveAllProductsFromOrder(orderId);
    } 
    public static void DeleteOrderHistory(int orderId)
    {
        OrderHistoryAccess.DeleteOrderHistory(orderId);
    }
}
