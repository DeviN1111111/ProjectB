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

            if (SessionManager.CurrentUser == null)
            {
                // Options when you're not logged in
                options.AddRange(new[] { "Login", "Register", "Continue as Guest", "Exit" });
            }
            else if (SessionManager.CurrentUser.AccountStatus == "User")
            {
                // Options when you're logged in as a regular user
                options.AddRange(new[] { "Order", "Cart", "Logout", "Exit" });
            }
            else if (SessionManager.CurrentUser.AccountStatus == "Admin")
            {
                // Options when you're logged in as an admin
                options.AddRange(new[] { "Management", "Statistics", "Logout", "Exit" });
            }
            else if (SessionManager.CurrentUser.AccountStatus == "Guest")
            {
                // Options when you're logged in as a guest
                options.AddRange(new[] { "Order", "Login", "Register", "Go back", "Exit" });
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
                case "Continue as Guest":
                    SessionManager.CurrentUser = new UserModel { Name = "Guest", LastName = "Guest", Email = "Guest@gmail.com", Password = "Guest", Address = "newstraat 12", Zipcode = "2234LB", PhoneNumber = "31432567897", City = "Guest", AccountStatus = "Guest" };
                    break;
                case "Order":
                    ProductUI.SearchProduct();
                    break;
                case "Cart":
                    Console.Clear();
                    Order.ShowCart();
                    break;
                case "Management":
                    ManagementUI.DisplayMenu();
                    break;
                case "Statistics":
                    StatisticsUI.DisplayMenu();
                    break;
                case "Logout":
                    SessionManager.CurrentUser = null!;
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