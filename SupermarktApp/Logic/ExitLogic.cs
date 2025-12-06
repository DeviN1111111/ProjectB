public static class ExitLogic
{
    public static async Task ApplicationExitAsync()
    {
        var user = SessionManager.CurrentUser;

        if (user == null)
            return;
            
        if (user.AccountStatus == "User")
        {
            await ProcessCartReminderAsync(user.ID, user.Email);
        }
    }

    private static async Task ProcessCartReminderAsync(int userId, string userEmail)
    {
        var cartItems = CartAccess.GetAllUserProducts(userId);

        if (cartItems != null && cartItems.Count > 0)
        {
            await EmailReminderLogic.SendCartReminderAsync(userEmail, cartItems);
        }
    }
}
