using Spectre.Console;
public static class StartScreen
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
                options.AddRange(new[] { "Order", "Logout", "Exit" });
            }
            else if (SessionManager.CurrentUser.AccountStatus == "Admin")
            {
                // Options when you're logged in as an admin
                options.AddRange(new[] { "Management", "Statistics", "Logout", "Exit" });
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
                    Console.WriteLine("[Continue as Guest placeholder]");
                    break;
                case "Order":
                    ProductUI.SearchProduct();
                    break;
                case "Management":
                    //todo
                    break;
                case "Statistics":
                    StatisticsUI.DisplayMenu();
                    break;
                case "Logout":
                    SessionManager.CurrentUser = null!;
                    break;
                case "Exit":
                    return;
            }


        }
    }
}