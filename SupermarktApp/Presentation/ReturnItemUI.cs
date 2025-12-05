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
        Utils.PrintTitle("Return Item");

        var user = SessionManager.CurrentUser!.ID;
        var orders = OrderLogic.GetAllUserOrders(user);

        var returnableOrders = ReturnItemLogic.CheckReturnableOrders(orders, DateTime.Today);

        var selectedOrder = Utils.CreateSelectionPrompt(
            returnableOrders,
            "Select an order",
            order => $"Order #{order.Id} - {order.Date:dd-MM-yyyy HH:mm}"
        );
    }
}