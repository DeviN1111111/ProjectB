using System.Security.Cryptography.X509Certificates;
using Spectre.Console;

using System.Threading;
public class Order
{
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    private static string Safe(string text) => Markup.Escape(text);
    public static void ShowCart()
    {
        Console.Clear();
        double totalAmount = 0;
        List<CartModel> allUserProducts = OrderLogic.AllUserProducts();  // List of user Products in cart
        List<ProductModel> allProducts = ProductLogic.GetAllProducts();  // List of all products dit moet via logic

        // Title
        AnsiConsole.Write(
            new FigletText("Cart")
                .Centered()
                .Color(AsciiPrimary));


        // Cart table
        var cartTable = new Table()
            .BorderColor(AsciiPrimary)
            .AddColumn("[white]Product[/]")
            .AddColumn("[white]Quantity[/]")
            .AddColumn("[white]Price[/]")
            .AddColumn("[white]Total[/]");

        foreach (var cartProduct in allUserProducts)
        {
            // Get Product id and find match in all products
            foreach (ProductModel Product in allProducts)
            {
                WeeklyPromotionsModel WeeklyDiscountProduct = ProductLogic.GetProductByIDinWeeklyPromotions(Product.ID);
                if (cartProduct.ProductId == Product.ID)
                {
                    if(cartProduct.RewardPrice > 0)
                    {
                        cartTable.AddRow(Product.Name, cartProduct.Quantity.ToString(), $"[green]FREE![/]", $"[green]FREE![/]");
                        
                    }
                    else if(WeeklyDiscountProduct != null)
                    {
                        string text = Product.Price.ToString();
                        var struckPrice = $"[strike][red]{text}[/][/]";

                        double discountedPrice = Product.Price - WeeklyDiscountProduct.Discount;

                        cartTable.AddRow(Product.Name, cartProduct.Quantity.ToString(), $"€{struckPrice} [green]€{Math.Round(discountedPrice, 2)}[/]", $"€{Math.Round(discountedPrice * cartProduct.Quantity, 2)}");
                        totalAmount = totalAmount + (Product.Price * cartProduct.Quantity);
                    }
                    else
                    {
                        cartTable.AddRow(Product.Name, cartProduct.Quantity.ToString(), $"€{Product.Price}", $"€{Math.Round(Product.Price * cartProduct.Quantity, 2)}");
                        totalAmount = totalAmount + (Product.Price * cartProduct.Quantity);
                    }
                }
            }
        }

        AnsiConsole.Write(cartTable);
        AnsiConsole.WriteLine();


        // Calculate total discount
        double discount = OrderLogic.CalculateTotalDiscount();

        // Calculate delivery fee
        double deliveryFee = OrderLogic.DeliveryFee(totalAmount - discount);

       
        if (totalAmount + deliveryFee - discount == 0)
        {
            deliveryFee = 5;
        }
        // Summary box
        var panel = new Panel(
            new Markup($"[bold white]Discount:[/] [red]-€{Math.Round(discount, 2)}[/]\n[bold white]Delivery Fee:[/] [yellow]€{Math.Round(deliveryFee, 2)}[/]\n[bold white]Total price:[/] [bold green]€{Math.Round(totalAmount + deliveryFee - discount, 2)}[/]"))
            .Header("[bold white]Summary[/]", Justify.Left)
            .Border(BoxBorder.Rounded)
            .BorderColor(AsciiPrimary)
            .Expand();

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
        double finalAmount = totalAmount + deliveryFee - discount;

        Checkout(allUserProducts, allProducts, finalAmount);
    }

    public static void ShowChecklist()
    {
        int selectedIndex = 0;
        var checkedItems = new HashSet<int>();

        while (true)
        {
            Console.Clear();

            var allUserProducts = ChecklistLogic.AllUserProducts();
            var allProducts = ProductAccess.GetAllProducts();

            AnsiConsole.Write(
                new FigletText("Checklist")
                    .Centered()
                    .Color(AsciiPrimary));

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



    public static void Checkout(List<CartModel> cartProducts, List<ProductModel> allProducts, double totalAmount)
    {
        // Checkout or go back options
        var options = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .AddChoices(new[]{

            "Checkout",
            "Remove items",
            "Change quantity",
            "Go back"
        }));

        switch (options)
        {
            case "Checkout":
                // check if cart is empty
                if (cartProducts.Count == 0)
                {
                    AnsiConsole.MarkupLine("[red]Your cart is empty![/]");
                    AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                    Console.ReadKey();
                    return;
                }
                // Add reward points to user
                int rewardPoints = RewardLogic.CalculateRewardPoints(totalAmount);;
                RewardLogic.AddRewardPointsToUser(rewardPoints);
                // pay now or pay on pickup
                Console.Clear();
                AnsiConsole.Write(
                    new FigletText("Checkout")
                        .Centered()
                        .Color(Color.White));
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
                        List<OrderItemModel> allOrderItems = new List<OrderItemModel>();  // List to hold order items

                        foreach (var item in cartProducts)
                        {
                            var product = allProducts.FirstOrDefault(cartProduct => cartProduct.ID == item.ProductId);
                            if (product != null)
                            {
                                var orderItem = new OrderItemModel(item.ProductId, item.Quantity, product.Price);  // Create OrderItemModel
                                allOrderItems.Add(orderItem);
                            }
                        }
                        OrderLogic.AddOrderWithItems(allOrderItems, allProducts);  // Create order with items

                        AnsiConsole.WriteLine("Thank you purchase succesful!");
                        AnsiConsole.MarkupLine($"[italic yellow]Added {rewardPoints} reward points to your account![/]");
                        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                        Console.ReadKey();
                        OrderLogic.UpdateStock();
                        OrderLogic.ClearCart();
                        break;
                    case "Pay on pickup":
                        List<OrderItemModel> allOrderItem = new List<OrderItemModel>();  // List to hold order items

                        foreach (var item in cartProducts)
                        {
                            var product = allProducts.FirstOrDefault(cartProduct => cartProduct.ID == item.ProductId);
                            if (product != null)
                            {
                                var orderItem = new OrderItemModel(item.ProductId, item.Quantity, product.Price);  // Create OrderItemModel
                                allOrderItem.Add(orderItem);
                            }
                        }
                        OrderLogic.AddOrderWithItems(allOrderItem, allProducts);  // Create order with items
                        AnsiConsole.WriteLine("Thank you purchase succesful!");
                        AnsiConsole.MarkupLine($"[italic yellow]Added {rewardPoints} reward points to your account![/]");
                        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                        Console.ReadKey();
                        OrderLogic.UpdateStock();
                        OrderLogic.ClearCart();
                        break;
                    case "Pay Later":
                        List<OrderItemModel> OrderedItems = new List<OrderItemModel>();  // List to hold order items

                        foreach (var item in cartProducts)
                        {
                            var product = allProducts.FirstOrDefault(cartProduct => cartProduct.ID == item.ProductId);
                            if (product != null)
                            {
                                var orderItem = new OrderItemModel(item.ProductId, item.Quantity, product.Price);  // Create OrderItemModel
                                OrderedItems.Add(orderItem);
                            }
                        }
                        OrderLogic.AddOrderWithItems(OrderedItems, allProducts);  // Create order with items
                        AnsiConsole.WriteLine("Thank you purchase succesful!");
                        AnsiConsole.WriteLine("You have 30 days to complete your payment. Unpaid orders will be fined. You will receive an email with payment instructions.");
                        AnsiConsole.MarkupLine($"[italic yellow]Added {rewardPoints} reward points to your account![/]");
                        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                        Console.ReadKey();
                        OrderLogic.UpdateStock();
                        OrderLogic.ClearCart();
                        break;
                }
                break;

            case "Remove items":
                RemoveFromCart(cartProducts, allProducts);
                break;
            case "Change quantity":
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
                    ShowCart();
                }
                break;

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

public static void DisplayOrderHistory()
{
    while (true)
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Order History")
                .Centered()
                .Color(AsciiPrimary));

        var userOrders = OrderAccess.GetOrdersByUserId(SessionManager.CurrentUser.ID);
        if (userOrders.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No order history found.[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
            return;
        }

        AnsiConsole.MarkupLine("[grey](Press [yellow]ESC[/] to go back or any key to continue)[/]");
        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
            return;

        var orderChoices = userOrders
            .Select(order => $"Order #{order.ID} - {order.Date:yyyy-MM-dd HH:mm}")
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

        var orderItems = OrderItemsAccess.GetOrderItemsByOrderId(selectedOrderId);

        if (orderItems.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]This order has no items.[/]");
            Console.ReadKey();
            continue;
        }

        Console.Clear();
        AnsiConsole.Write(
            new FigletText($"Order #{selectedOrderId}")
                .Centered()
                .Color(AsciiPrimary));

        var orderTable = new Table()
            .BorderColor(AsciiPrimary)
            .AddColumn("[white]Product[/]")
            .AddColumn("[white]Quantity[/]")
            .AddColumn("[white]Price per Unit[/]")
            .AddColumn("[white]Total Price[/]");

        decimal totalOrderPrice = 0;

        foreach (var item in orderItems)
        {
            var product = ProductAccess.GetProductByID(item.ProductId);
            if (product != null)
            {
                decimal itemTotal = (decimal)(item.Quantity * item.Price);
                totalOrderPrice += itemTotal;
                orderTable.AddRow(
                    product?.Name ?? "[red]Unknown Product[/]",
                    item.Quantity.ToString(),
                    $"€{item.Price:F2}",
                    $"€{itemTotal:F2}"
                );
            }
        }
        if (totalOrderPrice < 25)
        {
            decimal deliveryFee = 5;
            totalOrderPrice += deliveryFee;
            orderTable.AddEmptyRow();
            orderTable.AddRow("[yellow]Delivery Fee[/]", "", "", $"[bold red]€{deliveryFee:F2}[/]");
        }
        orderTable.AddEmptyRow();
        orderTable.AddRow("[yellow]Total[/]", "", "", $"[bold green]€{totalOrderPrice:F2}[/]");

        AnsiConsole.Write(orderTable);
        AnsiConsole.MarkupLine("\nPress [green]ENTER[/] to return to your orders list");
        Console.ReadKey();
    }
}


}