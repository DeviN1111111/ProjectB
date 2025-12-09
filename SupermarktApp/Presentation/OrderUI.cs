using System.Security.Cryptography.X509Certificates;
using Spectre.Console;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
public class Order
{
    private static string Safe(string text) => Markup.Escape(text);
    public static double CouponCredit = 0;
    public static int? SelectedCouponId = null;
    public static double TotalPrice = 0;
    public static async Task ShowCart()
    {
        Console.Clear();
        double totalAmount = 0;
        double totalDiscount = 0;
        List<CartModel> allUserProducts = OrderLogic.AllUserProducts();  // List of user Products in cart
        List<ProductModel> allProducts = ProductLogic.GetAllProducts();  // List of all products dit moet via logic

        // Title
        Utils.PrintTitle("Cart");

        // Cart table
        var cartTable = new Table()
            .BorderColor(ColorUI.AsciiPrimary)
            .AddColumn("[white]Product[/]")
            .AddColumn("[white]Quantity[/]")
            .AddColumn("[white]Price[/]")
            .AddColumn("[white]Total[/]");


        // Products in cart
        foreach (var cartProduct in allUserProducts)
        {
            // Get Product id and find match in all products
            foreach (ProductModel Product in allProducts)
            {
                ProductDiscountDTO productDiscount = DiscountsLogic.CheckDiscountByProduct(Product);
        
                if (cartProduct.ProductId == Product.ID)
                {
                    if(RewardLogic.GetRewardItemByProductId(Product.ID) != null) // if the product is a reward item print FREE
                    {
                        cartTable.AddRow(Product.Name, cartProduct.Quantity.ToString(), $"[green]FREE![/]", $"[green]FREE![/]");
                    }
                    else if(productDiscount != null)
                    {
                        double priceAfterDiscount = Math.Round((Product.Price * (1 - productDiscount.Discount.DiscountPercentage / 100)), 2);
                        double differenceBetweenPriceAndDiscountPrice = Product.Price - priceAfterDiscount;

                        totalDiscount += differenceBetweenPriceAndDiscountPrice * cartProduct.Quantity;

                        cartTable.AddRow(Product.Name, cartProduct.Quantity.ToString(), $"[strike red]€{Product.Price}[/][green] €{priceAfterDiscount}[/]", $"€{Math.Round(priceAfterDiscount * cartProduct.Quantity, 2)}");
                        totalAmount = totalAmount + (priceAfterDiscount * cartProduct.Quantity);
                    }
                    else
                    {
                        cartTable.AddRow(Product.Name, cartProduct.Quantity.ToString(), $"€{Product.Price}", $"€{Math.Round(Product.Price * cartProduct.Quantity, 2)}");
                        totalAmount = totalAmount + (Product.Price * cartProduct.Quantity);
                    }
                }
            }
        }

        double deliveryFee = allUserProducts.Count == 0 ? 0 : OrderLogic.DeliveryFee(totalAmount);
        var currentUser = SessionManager.CurrentUser!;
        double UnpaidFine = PayLaterLogic.Track(currentUser);
        double unpaidFineAmount = 0;
        double unpaidOrdersTotal = 0;
        int unpaidOrdersCount = 0;
        var userOrders = OrderLogic.GetAllUserOrders(currentUser.ID);

        if (userOrders != null)
        {
            foreach (var order in userOrders)
            {
                if (!order.IsPaid)
                {
                    unpaidOrdersCount++;
                }

                if (UnpaidFine > 0 && !order.IsPaid && order.FineDate.HasValue && DateTime.Now > order.FineDate.Value)
                {
                    unpaidFineAmount += 50;
                }
            }
        }

        if (UnpaidFine > unpaidFineAmount)
        {
            unpaidOrdersTotal = UnpaidFine - unpaidFineAmount;
        }
        var TotalAmount = Math.Round(totalAmount + deliveryFee + UnpaidFine - CouponCredit, 2);
        if (TotalAmount < 0)
        {
            TotalAmount = 0;
        }
        var headerText = unpaidOrdersCount > 0
            ? $"[bold white]You have {unpaidOrdersCount} unpaid orders[/]"
            : "[bold white]Summary[/]";
        
        var leftSide = new Rows(
            cartTable,
            new Panel(
                new Markup(
                    $"[bold white]Discount:[/] [red]-€{Math.Round(totalDiscount, 2)}[/]\n" +
                    $"[bold white]Delivery Fee:[/] [yellow]€{Math.Round(deliveryFee, 2)}[/]\n" +
                    $"[bold white]Unpaid Fine:[/] [yellow]€{Math.Round(unpaidFineAmount, 2)}[/]\n" +
                    $"[bold white]Unpaid Order:[/] [yellow]€{Math.Round(unpaidOrdersTotal, 2)}[/]\n" +
                    $"[bold white]Coupon Credit:[/] [green]€{Math.Round(CouponCredit, 2)}[/]\n" +
                    $"[bold white]Total price:[/] [bold green]€{TotalAmount}[/]"
                )
            )
            .Header(headerText, Justify.Left)
            .Border(BoxBorder.Double)
            .BorderColor(ColorUI.AsciiPrimary)
        );

        var rightSide =SuggestionsUI.GetSuggestionsPanel(SessionManager.CurrentUser!.ID);
        AnsiConsole.Write(
            new Columns(leftSide, rightSide)
                .Collapse()
                .PadRight(2)
        );

        AnsiConsole.WriteLine();

        double finalAmount = totalAmount + deliveryFee - totalDiscount + UnpaidFine - CouponCredit;
        TotalPrice = totalAmount + deliveryFee + UnpaidFine;

        await Checkout(allUserProducts, allProducts, finalAmount, UnpaidFine);
    }

    public static void ShowChecklist()
    {
        int selectedIndex = 0;
        var checkedItems = new HashSet<int>();

        while (true)
        {
            Console.Clear();

            var allUserProducts = ChecklistLogic.AllUserProducts();
            var allProducts = ProductLogic.GetAllProducts();
            
            Utils.PrintTitle("Checklist");

            if (allUserProducts.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[red]Your checklist is empty![/]");
                AnsiConsole.MarkupLine("[grey]──────────────────────────────[/]");
                AnsiConsole.MarkupLine("Press [green]ENTER[/] to go back.");
                Console.ReadKey(true);
                return;
            }

            AnsiConsole.MarkupLine("\n[grey]──────────────────────────────[/]");

            for (int i = 0; i < allUserProducts.Count; i++)
            {
                var checklistItem = allUserProducts[i];
                var product = allProducts.FirstOrDefault(p => p.ID == checklistItem.ProductId);
                if (product == null) continue;

                bool isSelected = (i == selectedIndex);
                bool isChecked = checkedItems.Contains(i);

                string checkbox = isChecked ? "[green][[X]][/]" : "[grey][[ ]][/]";
                string selector = isSelected ? "[cyan]>[/]" : " ";

                string safeName = Markup.Escape(product.Name);

                AnsiConsole.MarkupLine($"{selector} {checkbox} [white]{safeName}[/] (x{checklistItem.Quantity})");
            }

            AnsiConsole.MarkupLine("[grey]──────────────────────────────[/]");

            AnsiConsole.MarkupLine("[grey][[↑/↓]] Navigate  [[Space]] Toggle  [[Enter]] Confirm  [[Esc]] Back[/]");

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + allUserProducts.Count) % allUserProducts.Count;
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % allUserProducts.Count;
                    break;

                case ConsoleKey.Spacebar:
                    if (checkedItems.Contains(selectedIndex))
                        checkedItems.Remove(selectedIndex);
                    else
                        checkedItems.Add(selectedIndex);
                    break;

                case ConsoleKey.Enter:
                    if (checkedItems.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[yellow]No items selected.[/]");
                        continue;
                    }

                    var action = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[bold white]What would you like to do with the selected items?[/]")
                            .AddChoices("Clear list", "Remove from checklist", "Cancel")
                    );

                    foreach (var i in checkedItems.ToList())
                    {
                        var checklistItem = allUserProducts[i];
                        var product = allProducts.FirstOrDefault(p => p.ID == checklistItem.ProductId);
                        if (product == null) continue;

                        string safeName = Markup.Escape(product.Name);

                        switch (action)
                        {
                            case "Clear list":
                                ChecklistLogic.RemoveFromChecklist(product.ID);
                                AnsiConsole.MarkupLine($"[green]List has been cleared![/]");
                                break;

                            case "Remove from checklist":
                                ChecklistLogic.RemoveFromChecklist(product.ID);
                                AnsiConsole.MarkupLine($"[red]- {safeName} removed from checklist.[/]");
                                break;
                        }
                    }

                    if (action == "Cancel")
                        AnsiConsole.MarkupLine("[grey]No changes made.[/]");

                    checkedItems.Clear();
                    break;

                case ConsoleKey.Escape:
                    return;
            }
        }
    }



    public static async Task Checkout(
        List<CartModel> cartProducts, 
        List<ProductModel> allProducts, 
        double totalAmount, 
        double UnpaidFine)
    {

        var options = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .AddChoices(new[]{
            "Checkout",
            "Remove items",
            "Change quantity",
            "Suggested items", 
            "Add coupon",
            "Go back"
        }));

        switch (options)
        {
            case "Checkout":
                // check if all items are in stock
                var outOfStockProducts = OrderLogic.CheckStockBeforeCheckout(cartProducts, allProducts);
                if (outOfStockProducts.Count > 0)
                {
                    AnsiConsole.MarkupLine("[red]The following products are out of stock or exceed available quantity:[/]");
                    foreach (var productName in outOfStockProducts)
                    {
                        AnsiConsole.MarkupLine($"- {productName}");
                    }
                    AnsiConsole.MarkupLine("Please adjust your cart before proceeding to checkout.");
                    AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                    Console.ReadKey();
                    return;
                }
                // check if cart is empty
                if (cartProducts.Count == 0 && UnpaidFine <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Your cart is empty![/]");
                    AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                    Console.ReadKey();
                    return;
                }
                // Add reward points to user
                double rewardableAmount = Math.Max(0, TotalPrice);
                int rewardPoints = RewardLogic.CalculateRewardPoints(rewardableAmount);
                // pay now or pay on pickup
                Console.Clear();
                Utils.PrintTitle("Checkout");

                AnsiConsole.WriteLine("Choose payment method:");
                var option1 = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .AddChoices(new[]{
                            "Pay now",
                            "Pay on pickup",
                            "Pay Later"
                }));

                switch (option1)
                {
                    case "Pay now":
                        OrderLogic.ProcessPay(cartProducts, allProducts, SelectedCouponId);
                        AnsiConsole.WriteLine("Thank you purchase succesful!");
                        RewardLogic.AddRewardPointsToUser(rewardPoints);
                        AnsiConsole.MarkupLine($"[italic yellow]Added {rewardPoints} reward points to your account![/]");
                        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                        Console.ReadKey();
                        break;
                    case "Pay on pickup":
                        OrderLogic.ProcessPay(cartProducts, allProducts, SelectedCouponId);
                        AnsiConsole.WriteLine("Thank you purchase succesful!");
                        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                        Console.ReadKey();
                        break;
                    case "Pay Later":
                        List<OrdersModel> OrderedItems = new List<OrdersModel>();  // List to hold order items

                        foreach (var item in cartProducts)
                        {
                            var product = allProducts.FirstOrDefault(p => p.ID == item.ProductId);
                            if (product != null)
                            {
                                for (int i = 0; i < item.Quantity; i++)
                                {
                                    var newOrder = new OrdersModel
                                    {
                                        UserID = SessionManager.CurrentUser!.ID,
                                        ProductID = product.ID,
                                        Price = product.Price,
                                    };
                                    OrderedItems.Add(newOrder);
                                }
                            }
                        }
                        OrderLogic.AddOrderWithItems(OrderedItems, allProducts);  // Create order with items
                        AnsiConsole.WriteLine("Thank you purchase succesful!");
                        AnsiConsole.WriteLine($"You have till {DateTime.Today.AddDays(30)} to complete your payment. Unpaid orders will be fined. You will receive an email with payment instructions.");
                        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");

                        OrderHistoryModel order = OrderLogic.GetOrderByUserId(SessionManager.CurrentUser!.ID);
                        Console.ReadKey();
                        await PayLaterLogic.Activate(order.Id);
                        OrderLogic.UpdateStock();
                        OrderLogic.ClearCart();
                        break;
                }
                break;

            case "Remove items":
                RemoveFromCart(cartProducts, allProducts);
                break;
            case "Change quantity":
                ChangeCartQuantity(cartProducts, allProducts);
                break;
            
            case "Suggested items":
                await ShowSuggestedItems();
                await ShowCart();
                return;

            case "Add coupon":
                CouponUI.DisplayMenu();
                return;

            case "Go back":
                break;

        }
        return;
    }

    public static void RemoveFromCart(List<CartModel> cartProducts, List<ProductModel> allProducts)
    {
        // Build list of item names
        var cartChoices = new List<string>();
        foreach (var item in cartProducts)
        {
            var product = allProducts.FirstOrDefault(cartProduct => cartProduct.ID == item.ProductId);
            if (product != null)
                cartChoices.Add($"{product.Name} (x{item.Quantity})");
        }

        if (cartChoices.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Your cart is empty![/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
            return;
        }

        var itemsToRemove = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("[bold white]Select items to remove from your cart:[/]")
                .NotRequired()
                .PageSize(10)
                .MoreChoicesText("[grey](Use ↑/↓ to navigate, [blue]<space>[/] to select, [green]<enter>[/] to confirm)[/]")
                .AddChoices(cartChoices)
        );

        if (itemsToRemove.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No items selected.[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
            return;
        }

        foreach (var choice in itemsToRemove)
        {
            var productName = choice.Split(" (x")[0];
            var product = allProducts.FirstOrDefault(Product => Product.Name == productName);
            if (product != null)
                RemoveItemFromCart(product.ID);
        }

        AnsiConsole.MarkupLine("[green]Selected items have been removed![/]");
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
        Console.ReadKey();
        ShowCart();
    }
    private static void RemoveItemFromCart(int productId)
    {
        OrderLogic.RemoveFromCart(productId);
    }

    public static async Task DisplayOrderHistory()
    {
        while (true)
        {
            Console.Clear();

            var currentUser = SessionManager.CurrentUser!;
            var firstOrders = OrderLogic.GetAllUserOrders(currentUser.ID);
            if (firstOrders != null && firstOrders.Count == 1)
            {
                var userCoupons = CouponLogic.GetAllCoupons(currentUser.ID);
                if (userCoupons == null || userCoupons.Count == 0)
                {
                    CouponLogic.CreateCoupon(currentUser.ID, 5);
                }
            }
            Utils.PrintTitle("Order History");

            var userOrders = OrderLogic.GetAllUserOrders(SessionManager.CurrentUser!.ID);

            AnsiConsole.MarkupLine("[grey](Press [yellow]ESC[/] to go back or any key to continue)[/]\n");
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                return;

            if (userOrders == null || userOrders.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No order history found.[/]");
                AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                Console.ReadLine();
                return;
            }

            var orderChoices = userOrders
                .Select(order => order.IsPaid
                    ? $"Order #{order.Id} - {order.Date:dd-MM-yyyy HH:mm}"
                    : $"[red]Order #{order.Id} - {order.Date:dd-MM-yyyy HH:mm} (Unpaid)[/]")
                .ToList();
            
            string selectedOrderLabel = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Select an order to view details[/]")
                    .AddChoices(orderChoices)
            );
            var selectedOrderId = int.Parse(
                selectedOrderLabel
                    .Split(' ')[1]
                    .Replace("#", "")
            );

            var orderItems = OrderLogic.GetOrderssByOrderId(selectedOrderId);

            if (orderItems.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]This order has no items.[/]");
                Console.ReadKey();
                continue;
            }

            Console.Clear();
            Utils.PrintTitle($"Order #{selectedOrderId}");

            var orderTable = new Table()
                .BorderColor(ColorUI.AsciiPrimary)
                .AddColumn("[white]Product[/]")
                .AddColumn("[white]Quantity[/]")
                .AddColumn("[white]Price per Unit[/]")
                .AddColumn("[white]Total Price[/]");

            double totalOrderPrice = 0;

            // Dictionary to count how many times each product appears
            var productCounts = new Dictionary<int, int>();

            // First pass: count how many of each ProductID there are
            foreach (var item in orderItems)
            {
                if (productCounts.ContainsKey(item.ProductID))
                    productCounts[item.ProductID]++;
                else
                    productCounts[item.ProductID] = 1;
            }

            // Second pass: build the table using the counted quantities
            foreach (var keyValuePair in productCounts)
            {
                int productId = keyValuePair.Key;
                int quantity = keyValuePair.Value;

                var product = ProductLogic.GetProductById(productId);
                if (product != null)
                {
                    double price = product.Price;

                    ProductDiscountDTO productDiscount = DiscountsLogic.CheckDiscountByProduct(product);
                    
                    double DiscountPercentage = 0;

                    if(productDiscount != null)
                    {
                        DiscountPercentage = productDiscount.Discount.DiscountPercentage;
                    }
                    // Apply discounts if needed
                    if (DiscountPercentage > 0)
                    {
                        price = Math.Round(product.Price * (1 - DiscountPercentage / 100), 2);
                        // TODO: update the price in OrderHistory database if needed
                    }

                    double itemTotal = quantity * price;
                    totalOrderPrice += itemTotal;

                    orderTable.AddRow(
                        product?.Name ?? "[red]Unknown Product[/]",
                        quantity.ToString(),
                        $"${price:F2}",
                        $"${itemTotal:F2}"
                    );
                }
            }


            // Add total row
            orderTable.AddEmptyRow();
            orderTable.AddRow("Subtotal", "", "", $"${totalOrderPrice:F2}");
            double deliveryFee = OrderLogic.DeliveryFee(totalOrderPrice);
            if (deliveryFee > 0)
            {
                orderTable.AddEmptyRow();
                orderTable.AddRow("[yellow]Delivery Fee[/]", "", "", $"[bold red]${deliveryFee:F2}[/]");
            }
            double finalTotal = totalOrderPrice + deliveryFee;
            orderTable.AddEmptyRow();
            orderTable.AddRow("[yellow]Total[/]", "", "", $"[bold green]${finalTotal:F2}[/]");

            AnsiConsole.Write(orderTable);

           var selectedOrder = userOrders.First(o => o.Id == selectedOrderId);

            if (!selectedOrder.IsPaid)
            {
                HandleUnpaidOrder(selectedOrder, selectedOrderId, finalTotal);
            }
            else
            {
                HandlePaidOrder(selectedOrder);
            }

            AnsiConsole.MarkupLine("\nPress [green]ENTER[/] to return to your orders list");
            Console.ReadKey();
        }
    }

    public static async Task ShowSuggestedItems()
    {
        Console.Clear();

        var suggestions = SuggestionsLogic.GetSuggestedItems(SessionManager.CurrentUser!.ID);

        if (suggestions.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No suggestions available right now.[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to go back.");
            Console.ReadKey(true);
            return;
        }

        // dispay the suggestion list to the right
        var panel = SuggestionsUI.GetSuggestionsPanel(SessionManager.CurrentUser!.ID);
        AnsiConsole.Write(panel);

        AnsiConsole.MarkupLine("\n[grey]Press a number (1–9) to add an item.[/]");
        AnsiConsole.MarkupLine("[grey]Press ENTER to return.[/]");

        var key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.Enter)
            return;
        
        // convert key to index
        int indexPressed = (int)key - (int)ConsoleKey.D1;

        if(indexPressed >= 0 && indexPressed< suggestions.Count)
        {
            var product = suggestions[indexPressed];
            // fill 0 in for the discount and reward
            OrderLogic.AddToCart(product, 1, 0, 0);
            AnsiConsole.MarkupLine($"\n[green]Added [yellow]{Markup.Escape(product.Name)}[/] to cart![/]");
            Thread.Sleep(800);
        }
        return;
    }
    public static void ChangeCartQuantity(List<CartModel> cartProducts, List<ProductModel> allProducts)
    {
        while (true)
    {
        var productNames = new List<string>();
                foreach (var item in cartProducts)
                {
                    var product = allProducts.FirstOrDefault(cartProduct => cartProduct.ID == item.ProductId);
                    if (product != null)
                        productNames.Add($"{product.Name} (x{item.Quantity})");
                }
                if (productNames.Count == 0)
                {
                    AnsiConsole.MarkupLine("[red]Your cart is empty![/]");
                    AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                    Console.ReadKey();
                    return;
                }
                string selectProduct = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[white]Select a product to change its quantity:[/]")
                        .AddChoices(productNames)
                );

                var selectedProductName = selectProduct.Split(" (x")[0];
                var selectedProduct = allProducts.FirstOrDefault(Product => Product.Name == selectedProductName);
                if (selectedProduct != null)
                {
                    int newQuantity = AnsiConsole.Prompt(
                        new TextPrompt<int>($"Enter new quantity for [yellow]{selectedProduct.Name}[/]:")
                            .Validate(
                                quantity => { return quantity < 1 || quantity > 99 ? ValidationResult.Error("[red]Quantity must be at least 1 and less than 100.[/]") : ValidationResult.Success(); }
                                )
                    );

                    OrderLogic.ChangeQuantity(selectedProduct.ID, newQuantity);
                    AnsiConsole.MarkupLine($"[green]Quantity for [yellow]{selectedProduct.Name}[/] updated to [yellow]{newQuantity}[/].[/]");
                    AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                    Console.ReadKey();
                    break;
                }
    }
    }
    public static void HandleUnpaidOrder(OrderHistoryModel selectedOrder, int selectedOrderId, double finalTotal)
    {
        // Show fine date if exists
        if (selectedOrder.FineDate != null)
        {
            AnsiConsole.MarkupLine(
                $"[yellow]You have till [red]{selectedOrder.FineDate:dd-MM-yyyy HH:mm}[/] to pay.[/]\n"
            );
        }

        var payChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]This order is unpaid. What would you like to do?[/]")
                .AddChoices("Pay Now", "Go Back")
        );

        if (payChoice != "Pay Now")
            return;

        var paymentCode = AnsiConsole.Ask<string>("[yellow]Enter your 6-digit payment code:[/]");
        if (!paymentCode.All(char.IsDigit) || paymentCode.Length != 6)
        {
            AnsiConsole.MarkupLine("[red]Invalid code. It must be 6 digits and numeric.[/]");
            Console.ReadKey();
            return;
        }

        bool isPaid = PayLaterLogic.Pay(selectedOrderId, int.Parse(paymentCode));
        if (!isPaid)
        {
            AnsiConsole.MarkupLine("[red]Payment failed or declined.[/]");
            Console.ReadKey();
            return;
        }

        AnsiConsole.MarkupLine("[green]Payment successful.[/]");
        double rewardableAmount = Math.Max(0, finalTotal);
        int rewardPoints = RewardLogic.CalculateRewardPoints(rewardableAmount);
        RewardLogic.AddRewardPointsToUser(rewardPoints);
        AnsiConsole.MarkupLine($"[italic yellow]Added {rewardPoints} reward points to your account![/]");
        Console.ReadKey();
    }
    public static void HandlePaidOrder(OrderHistoryModel selectedOrder)
    {
        var reorderChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]This order is already paid. What would you like to do?[/]")
                .AddChoices("Reorder", "Go Back")
        );

        if (reorderChoice != "Reorder")
            return;

        var confirmReorder = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[red]Are you sure you want to reorder this past order?[/]")
                .AddChoices("Yes", "No")
        );

        if (confirmReorder != "Yes")
            return;

        int actualOrderId = selectedOrder.Id;
        var reorderResult = OrderLogic.ReorderPastOrder(actualOrderId);
        AnsiConsole.MarkupLine("[green]Items added to cart (where possible)![/]");

        if (reorderResult.Unavailable.Any())
        {
            AnsiConsole.MarkupLine("\n[red]The following products are no longer available:[/]");
            foreach (var name in reorderResult.Unavailable)
                AnsiConsole.MarkupLine($"- {name}");
        }

        if (reorderResult.OutOfStock.Any())
        {
            AnsiConsole.MarkupLine("\n[yellow]The following products had stock issues:[/]");
            foreach (var name in reorderResult.OutOfStock)
                AnsiConsole.MarkupLine($"- {name}");
        }
        Console.ReadKey();
    }
}
