public static class ExitLogic
{
    // public static async Task ApplicationExitAsync()
    // {
    //     var user = SessionManager.CurrentUser;

    //     if (user == null) return;
    //     if (user.AccountStatus != "User") return;

    //     await ProcessCartReminderAsync(user.ID, user.Email);
    // }
   
    public static async Task ApplicationExitAsync() //DEBUG
    {
        Console.WriteLine("ExitLogic: ApplicationExitAsync called");

        var user = SessionManager.CurrentUser;

        if (user == null)
        {
            Console.WriteLine("ExitLogic: user is null, aborting");
            return;
        }

        Console.WriteLine($"ExitLogic: user = {user.Email}, status = {user.AccountStatus}");

        if (user.AccountStatus == "User")
        {
            await ProcessCartReminderAsync(user.ID, user.Email);
        }
    }


    // private static async Task ProcessCartReminderAsync(int userId, string userEmail)
    // {
    //     var cartItems = CartAccess.GetAllUserProducts(userId);

    //     if (cartItems != null && cartItems.Count > 0)
    //     {
    //         await EmailReminderLogic.SendCartReminderAsync(userEmail, cartItems);
    //         SessionManager.HasSentExitCartEmail = true;
    //     }
    // }
         
    private static async Task ProcessCartReminderAsync(int userId, string userEmail) //DEBUG
    {
        Console.WriteLine($"ExitLogic: fetching cart items for user {userId}");

        var cartItems = CartAccess.GetAllUserProducts(userId);

        Console.WriteLine($"ExitLogic: cart item count = {cartItems?.Count ?? 0}");

        if (cartItems != null && cartItems.Count > 0)
        {
            Console.WriteLine("ExitLogic: sending cart reminder email...");
            await EmailReminderLogic.SendCartReminderAsync(userEmail, cartItems);
            SessionManager.HasSentExitCartEmail = true;
        }
        else
        {
            Console.WriteLine("ExitLogic: no items in cart, no email sent");
        }
    }

}
