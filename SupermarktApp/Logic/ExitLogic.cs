public static class ExitLogic
{
    public static void ApplicationExit()
    {
        if (SessionManager.CurrentUser == null) 
            return;


        if (SessionManager.CurrentUser.AccountStatus == "User")
        {
            ProcessCartReminder(SessionManager.CurrentUser.ID);
        }
    }

    private static void ProcessCartReminder(int userId)
    {
        // get the items out of cart
        var cartItems = CartAccess.GetAllUserProducts(userId);

        // if there are items send send to next step.
        if (cartItems != null && cartItems.Count > 0)
        {
            EmailReminderLogic.SendCartReminder(SessionManager.CurrentUser, cartItems);
        }
    }
}