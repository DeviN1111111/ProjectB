using Spectre.Console;

class ReturnItemUI
{
    public static void DisplayMenu()
    {
        Console.Clear();
        Utils.PrintTitle("Return Products");
        
        // ## Get dependencies ##
        var user = SessionManager.CurrentUser!.ID;
        var orders = OrderLogic.GetAllUserOrders(user);

        var returnableOrderHistories = ReturnItemLogic.CheckReturnableOrders(orders, DateTime.Today);

        // ## No returnable orders ##
        if (!returnableOrderHistories.Any())
        {
            AnsiConsole.MarkupLine("[yellow]You have no orders eligible for return.[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
            return;
        }
        
        // ## Printing ##
        var selectedOrderHistory = Utils.CreateSelectionPrompt(
            returnableOrderHistories,
            "Select an order",
            order => $"Order #{order.Id} - {order.Date:dd-MM-yyyy HH:mm}"
        );

        var orderId = selectedOrderHistory.Id;
        var orderLines = OrderLogic.GetOrderssByOrderId(orderId);
        double totalRefund = 0;

        var selectedChoice = Utils.CreateSelectionPrompt(new [] {"Return all products", "Return specific products", "[red]Go back[/]"});

        if (selectedChoice == "Return all products")
        {
            foreach (var orderedProduct in orderLines)
            {
                ProductLogic.UpdateStock(orderedProduct.ProductID, 1);
                totalRefund += orderedProduct.Price;
            }

            OrderLogic.RemoveAllProductsFromOrder(orderId);
            OrderLogic.DeleteOrderHistory(orderId);

            AnsiConsole.MarkupLine($"[green]Successfully returned all products. Total refund: {Utils.ChangePriceFormat(totalRefund)}[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
        }
        
        else if (selectedChoice == "Return specific products")
        {
            var itemsForSelection = ReturnItemLogic.GetReturnableProductsWithQuantity(orderLines);

            var selectedProducts = Utils.CreateMultiSelectionPrompt(
                itemsForSelection,
                "Choose products",
                item => $"{item.Product.Name} | x{item.Quantity} | [green]€{item.UnitPrice}[/]"
            );

            foreach (var item in selectedProducts)
            {
                var qtyToReturn = Utils.AskInt(
                    $"How many of [yellow]{item.Product.Name}[/] do you want to return? (max {item.Quantity})",
                    1,
                    item.Quantity
                );
                
                if (qtyToReturn <= 0) continue;

                ProductLogic.UpdateStock(item.Product.ID, qtyToReturn);
                ReturnItemLogic.RemoveProductQuantityFromOrder(orderId, item.Product.ID, qtyToReturn);

                totalRefund += qtyToReturn * item.UnitPrice;
            } 

            if (!OrderLogic.GetOrderssByOrderId(orderId).Any())
            {
                OrderLogic.DeleteOrderHistory(orderId);
            }

            AnsiConsole.MarkupLine($"[green]Successfully returned selected products. Total refund: {Utils.ChangePriceFormat(totalRefund)}[/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();                
        }
        
        else return;
    }
}