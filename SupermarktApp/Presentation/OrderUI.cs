using System;
using System.Collections.Generic;
using Spectre.Console;
public static class Order
{
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");


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


        // Products in cart
        foreach (var cartProduct in allUserProducts)
        {
            // Get Product id and find match in all products
            foreach (ProductModel Product in allProducts)
            {
                if (cartProduct.ProductId == Product.ID)
                {
                    cartTable.AddRow(Product.Name, cartProduct.Quantity.ToString(), $"${Product.Price}", $"${Product.Price * cartProduct.Quantity}");
                    totalAmount = totalAmount + (Product.Price * cartProduct.Quantity);
                }
            }
        }

        AnsiConsole.Write(cartTable);
        AnsiConsole.WriteLine();

        // Calculate delivery fee
        double deliveryFee = OrderLogic.DeliveryFee(totalAmount);

        // Calculate total discount
        double discount = OrderLogic.CalculateTotalDiscount();

        // Summary box
        var panel = new Panel(
            new Markup($"[bold white]Total price:[/] [white]${Math.Round(totalAmount + deliveryFee - discount, 2)}[/]\n[bold white]Discount:[/] [white]${discount}[/]\n[bold white]Delivery Fee:[/] [white]${Math.Round(deliveryFee, 2)}[/]"))
            .Header("[bold white]Summary[/]", Justify.Left)
            .Border(BoxBorder.Rounded)
            .BorderColor(AsciiPrimary)
            .Expand();

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
        double finalAmount = totalAmount + deliveryFee - discount;

        Checkout(allUserProducts, allProducts, finalAmount);
    }
    public static void Checkout(List<CartModel> cartProducts, List<ProductModel> allProducts, double totalAmount)
    {
        // Checkout or go back options
        var options = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .AddChoices(new[]{

            "Checkout",
            "Remove items",
            "Go back"
        })
);

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
                int rewardPoints = RewardLogic.CalculateRewardPoints(totalAmount);
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

        // Actually remove items
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

        // Refresh cart view
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
                    $"${item.Price:F2}",
                    $"${itemTotal:F2}"
                );
            }
        }
        if (totalOrderPrice < 25)
        {
            decimal deliveryFee = 5;
            totalOrderPrice += deliveryFee;
            orderTable.AddEmptyRow();
            orderTable.AddRow("[yellow]Delivery Fee[/]", "", "", $"[bold red]${deliveryFee:F2}[/]");
        }
        orderTable.AddEmptyRow();
        orderTable.AddRow("[yellow]Total[/]", "", "", $"[bold green]${totalOrderPrice:F2}[/]");

        AnsiConsole.Write(orderTable);
        AnsiConsole.MarkupLine("\nPress [green]ENTER[/] to return to your orders list");
        Console.ReadKey();
    }
}


}