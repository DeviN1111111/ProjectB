class ReturnItemUI
{
    /*
    1. Print the main menu for this
    2. Display the available orders that can be returned
        - Retrieve order history from orderhistory where date <= 3 days
    3. When order selected user can select all the items or specific items to return
    4. Return the price to the user
        - Remove item from orderhistory
        - Update product stock
    */
    public static void DisplayMenu()
    {
        Console.Clear();
        Utils.PrintTitle("Return Products");
        
        // ## Get dependencies ##
        var user = SessionManager.CurrentUser!.ID;
        var orders = OrderLogic.GetAllUserOrders(user);

        var returnableOrderHistories = ReturnItemLogic.CheckReturnableOrders(orders, DateTime.Today);
        
        // ## Printing ##
        var selectedOrderHistory = Utils.CreateSelectionPrompt(
            returnableOrderHistories,
            "Select an order",
            order => $"Order #{order.Id} - {order.Date:dd-MM-yyyy HH:mm}"
        );

        var orderId = selectedOrderHistory.Id;
        var orderLines = OrderAccess.GetOrderssByOrderId(orderId);
        double totalRefund = 0;

        var selectedChoice = Utils.CreateSelectionPrompt(new [] {"Return all products", "Return specific products", "[red]Go back[/]"});

        if (selectedChoice == "Return all products")
        {
            foreach (var orderedProduct in orderLines)
            {
                totalRefund += orderedProduct.Price;
            }

            OrderLogic.RemoveAllProductsFromOrder(orderId);
            OrderLogic.DeleteOrderHistory(orderId);
        }
        
        else if (selectedChoice == "Return specific products")
        {
            var itemsForSelection = ReturnItemLogic.GetReturnableProductsWithQuantity(orderLines);

            var selectedProducts = Utils.CreateMultiSelectionPrompt(
                itemsForSelection,
                "Choose products",
                item => $"{item.Product.Name} | x{item.Quantity} | [green]{item.UnitPrice}[/]"
            );

            foreach (var item in selectedProducts)
            {
                var qtyToReturn = Utils.AskInt(
                    $"How many of {item.Product.Name} do you want to return? (max {item.Quantity})",
                    1,
                    item.Quantity
                );
                
                if (qtyToReturn <= 0) continue;

                ReturnItemLogic.RemoveProductQuantityFromOrder(orderId, item.Product.ID, qtyToReturn);

                totalRefund += qtyToReturn * item.UnitPrice;
            } 

            if (!OrderLogic.GetOrderssByOrderId(orderId).Any())
            {
                OrderLogic.DeleteOrderHistory(orderId);
            }                     
        }

        else return;
    }
}