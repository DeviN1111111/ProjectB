public static class ExitLogic
{
    public static async Task ApplicationExitAsync()
    {
        var user = SessionManager.CurrentUser;

        if (user == null) return;
        if (user.AccountStatus != "User") return;

        await ProcessCartProductReminderAsync(user.ID, user.Email);
    }

    private static async Task ProcessCartProductReminderAsync(int userId, string userEmail)
    {
        var CartProductItems = CartProductAccess.GetAllUserProducts(userId);

        if (CartProductItems != null && CartProductItems.Count > 0)
        {
            await EmailReminderLogic.SendCartProductReminderAsync(userEmail, CartProductItems);
            SessionManager.HasSentExitCartProductEmail = true;
        }
    }

}
