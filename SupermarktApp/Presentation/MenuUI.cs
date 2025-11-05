using Spectre.Console;
public static class MenuUI
{
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static void ShowMainMenu()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("Supermarket App")
                    .Centered()
                    .Color(AsciiPrimary));
            var options = new List<string>();

            List<ProductModel> products = NotificationLogic.GetAllLowQuantityProducts(100);
            int lowStockCount = products.Count;


            if (SessionManager.CurrentUser == null)
            {
                // Options when you're not logged in
                options.AddRange(new[] { "Login", "Register", "Shop Details", "Exit" });
            }
            else if (SessionManager.CurrentUser.AccountStatus == "User")
            {
                // Options when you're logged in as a regular user
                options.AddRange(new[] { "Order", "Discounted Products", "Cart", "Checklist", "Order History", "Rewards", "Shop Details", "Logout", "Exit"});
            }
            else if (SessionManager.CurrentUser.AccountStatus == "Admin")
            {
                // Options when you're logged in as an admin
                if (lowStockCount > 0)
                    AnsiConsole.MarkupLine($"[red]You have {lowStockCount} low stock notifications![/]");
                else
                    AnsiConsole.MarkupLine($"[green]You have {lowStockCount} low stock notifications![/]");
                options.AddRange(new[] { "Notification", "Management", "Statistics", "Shop Details", "Logout", "Exit" });
            }
            // else if (SessionManager.CurrentUser.AccountStatus == "Guest")
            // {
            //     // Options when you're logged in as a guest
            //     options.AddRange(new[] { "Order", "Discounted Products", "Cart", "Checklist", "Login", "Register", "Go back", "Exit" });
            // }
            else if (SessionManager.CurrentUser.AccountStatus == "SuperAdmin")
            {
                // Options when you're logged in as a guest
                if (lowStockCount > 0)
                    AnsiConsole.MarkupLine($"[red]You have {lowStockCount} low stock notifications![/]");
                else
                    AnsiConsole.MarkupLine($"[green]You have {lowStockCount} low stock notifications![/]");
                options.AddRange(new[] { "Notification", "Management", "Statistics", "Manage admin", "Shop Details", "Logout", "Exit" });
            }
            else
            {
                // Option when account status is unrecognized
                options.Add("Exit");
            }

            var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(options));

            if (choice == $"Notification[{lowStockCount}]")
            {   
                NotificationUI.DisplayMenu();
            }

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
                case "Order":
                    ProductUI.SearchProduct();
                    break;
                case "Cart":
                    Console.Clear();
                    Order.ShowCart();
                    break;
                case "Discounted Products":
                    DiscountedProductsUI.DisplayMenu();
                    break;
                case "Checklist":
                    Console.Clear();
                    Order.ShowChecklist();
                    break;
                case "Order History":
                    Order.DisplayOrderHistory();
                    break;
                case "Management":
                    ManagementUI.DisplayMenu();
                    break;
                case "Notification":
                    NotificationUI.DisplayMenu();
                    break;
                case "Statistics":
                    StatisticsUI.DisplayMenu();
                    break;
                case "Manage admin":
                    ManageAdminUI.DisplayMenu();
                    break;
                case "Logout":
                    SessionManager.CurrentUser = null!;
                    break;
                case "Rewards":
                    RewardUI.DisplayMenu();
                    break;
                case "Go back":
                    SessionManager.CurrentUser = null!;
                    break;
                case "Exit":
                    return;
            }



        }
    }
}