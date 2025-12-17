using System.Threading.Tasks;
using Spectre.Console;
public static class MenuUI
{
    public static async Task ShowMainMenu()
    {
        while (true)
        {
            Console.Clear();
            Utils.PrintTitle("Supermarket App");
            var options = new List<string>();

            List<ProductModel> products = NotificationLogic.GetAllLowQuantityProducts(50);
            int lowStockCount = products.Count;

            if (SessionManager.CurrentUser != null)
            {
                if(SessionManager.CurrentUser.AccountStatus == "Admin" || SessionManager.CurrentUser.AccountStatus == "SuperAdmin")
                {   // Notification for the Admins based on stock count
                    if (lowStockCount > 0)
                        AnsiConsole.MarkupLine($"[red]You have {lowStockCount} low stock notifications![/]");
                    else
                        AnsiConsole.MarkupLine($"[green]You have {lowStockCount} low stock notifications![/]");
                }
            }
            if (SessionManager.CurrentUser == null)
            {
                // Options when you're not logged in
                options.AddRange(new[] { "Login", "Register", "Shop Details", "Exit" });
            }
            else if (SessionManager.CurrentUser.AccountStatus == "User")
            {
                // Options when you're logged in as a regular user
                options.AddRange(new[] { 
                    "Order", 
                    "Cart", 
                    "Checklist", 
                    "Order History", 
                    "Favorite Lists", 
                    "Return Item",
                    "Rewards", 
                    "Discounts", 
                    "Shop Reviews", 
                    "Shop Details", 
                    "Settings", 
                    "Logout", 
                    "Exit"
                    });
            }
            else if (SessionManager.CurrentUser.AccountStatus == "Admin")
            {
                // Options when you're logged in as an admin
                options.AddRange(new[] { "Notification", "Management", "Statistics", "Shop Details", "Logout", "Exit" });
            }
            else if (SessionManager.CurrentUser.AccountStatus == "SuperAdmin")
            {
                // Options when you're logged in as a superadmin
                options.AddRange(new[] { "Notification", "Management", "Statistics", "Shop Details", "Logout", "Exit" });
            }
            else
            {
                // Option when account status is unrecognized
                options.Add("Exit");
            }

            var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(options));


            switch (choice)
            {
                case "Login":
                    LoginUI.Login();
                    break;
                case "Register":
                    LoginUI.Register();
                    break;
                case "Shop Details":
                    ShopDetailsUI.Show();
                    break;
                case "Favorite Lists":
                    FavoriteListUI.DisplayMenu();
                    break;
                case "Shop Reviews":
                    ShopReviewUI.ShowMenu();
                    break;
                case "Order":
                    ProductUI.SearchProduct();
                    break;
                case "Cart":
                    Order.ShowCartProduct();
                    break;
                case "Return Item":
                    ReturnItemUI.DisplayMenu();
                    break;
                case "Checklist":
                    Order.ShowChecklist();
                    break;
                case "Order History":
                    await Order.DisplayOrderHistory();
                    break;
                case "Management":
                    ManagementUI.DisplayMenu();
                    break;
                case "Notification":
                    NotificationUI.DisplayMenu();
                    break;
                case "Discounts":
                    DiscountsUI.DisplayMenu();
                    break;
                case "Statistics":
                    StatisticsUI.DisplayMenu();
                    break;
                case "Logout":
                    await ExitLogic.ApplicationExitAsync();  //send email
                    SessionManager.Logout();
                    break;
                case "Rewards":
                    RewardUI.DisplayMenu();
                    break;
                case "Settings":
                    SettingsUI.ShowSettingsMenu();
                    break;
                case "Exit":
                    await ExitLogic.ApplicationExitAsync();  //email 
                    SessionManager.Logout();
                    return;
            }
        }
    }
}