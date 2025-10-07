using System.Security.Cryptography.X509Certificates;
using Spectre.Console;

class StartScreen
{
    private static readonly string _supermarketName = @"
/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/
/\                                                                                      /\
\/                                                                                      \/
/\                                                                                      /\
\/                                                                                      \/
/\                                                                                      /\
\/      █████   ███   █████       ████                                                  \/
/\     ░░███   ░███  ░░███       ░░███                                                  /\
\/      ░███   ░███   ░███ ██████ ░███   ██████   ██████  █████████████    ██████       \/
/\      ░███   ░███   ░██████░░███░███  ███░░███ ███░░███░░███░░███░░███  ███░░███      /\
\/      ░░███  █████  ███░███████ ░███ ░███ ░░░ ░███ ░███ ░███ ░███ ░███ ░███████       \/
/\       ░░░█████░█████░ ░███░░░  ░███ ░███  ███░███ ░███ ░███ ░███ ░███ ░███░░░        /\
\/         ░░███ ░░███   ░░██████ █████░░██████ ░░██████  █████░███ █████░░██████       \/
/\          ░░░   ░░░     ░░░░░░ ░░░░░  ░░░░░░   ░░░░░░  ░░░░░ ░░░ ░░░░░  ░░░░░░        /\
\/                                                                                      \/
/\                                                                                      /\
\/                                                                                      \/
/\                                                                                      /\
\/                                                                                      \/
/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/
";
    /// <summary>
    /// Displays a menu and handles user input for navigation and selection.
    /// </summary>
    /// 
    public static UserModel user;
    public static void Menu()
    {
        bool running = true;
        while (running)
        {
            if (user == null)
            {
                Console.Clear();
                Console.WriteLine($"{_supermarketName}");
                var options = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices(new[]{
                        "Login",
                        "Register",
                        "Continue as Guest",
                        "Exit"
                    })
                );

                switch (options)
                {
                    case "Login":
                        Console.Clear();
                        user = LoginUI.Login();
                        break;
                    case "Register":
                        Console.Clear();
                        LoginUI.Register();
                        break;
                    case "Continue as Guest":
                        System.Console.WriteLine("[Continue as Guest placeholder]");
                        break;
                    case "Exit":
                        running = false;
                        break;
                }
            }
            else if (user.AccountStatus == "User")
            {
                var options = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices(new[]{
                        "Order",
                        "Logout",
                        "Exit"
                    })
                );

                switch (options)
                {
                    case "Order":
                        break;
                    case "Logout":
                        Console.Clear();
                        user = null!;
                        break;
                    case "Exit":
                        Environment.Exit(0);
                        break;
                }
            }
            else if (user.AccountStatus == "Admin")
            {
                var options = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices(new[]{
                        "Management",
                        "Logout",
                        "Exit"
                    })
                );

                switch (options)
                {
                    case "Management":
                        ManagementMenu();
                        break;
                    case "Logout":
                        Console.Clear();
                        user = null!;
                        break;
                    case "Exit":
                        Environment.Exit(0);
                        break;
                }
            }
        }


        // var options = AnsiConsole.Prompt(
        //     new SelectionPrompt<string>()
        //         .AddChoices(new[]{
        //             "Login",
        //             "Register",
        //             "Continue as Guest",
        //             "Exit"
        //         })
        // );

        // switch (options)
        // {
        //     case "Login":
        //         LoginUI.Login();
        //         break;
        //     case "Register":
        //         LoginUI.Register();
        //         break;
        //     case "Continue as Guest":
        //         System.Console.WriteLine("[Continue as Guest placeholder]");
        //         break;
        //     case "Exit":
        //         Environment.Exit(0);
        //         break;
        // }
    }
    /// <summary>
    /// Displays the start screen with the supermarket name and menu options.
    /// </summary>
    public static void Show()
    {
        AnsiConsole.Clear();
        // AnsiConsole.MarkupLine($"[bold yellow]{_supermarketName}[/]");
        System.Console.WriteLine();
        Menu();
    }

    public static void ManagementMenu()
    {
        var options = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices(new[]{
                        "Add Product",
                        "Edit Product",
                        "Delete Product",
                        "Return"
                    })
                );

                switch (options)
                {
                    case "Add Product":
                        ManagementMenu();
                        break;
                    case "Edit Product":
                        break;
                    case "Delete Product":
                        break;
                    case "Return":
                        Menu();
                        break;
                }
    }

    
    /// <summary>
    /// Displays a menu and handles user input for navigation and selection.
    /// </summary>
    // public void Menu()
    // {
    //     // Options for the menu
    //     string[] options = { "Login", "Register", "Continue as Guest", "Exit" };
    //     int selected = 0;
    //     // Hide the cursor for better visual effect
    //     Console.CursorVisible = false;
    //     int top = Console.CursorTop;
    //     // Main loop to handle user input
    //     while (true)
    //     {
    //         // Build the line with the current selection
    //         string line = "";
    //         for (int i = 0; i < options.Length; i++)
    //         {
    //             line += (i == selected) ? $"[ {options[i]} ] " : $"  {options[i]}  ";
    //         }
    //         // Calculate padding to center the line
    //         int leftPad = Math.Max(0, (Console.WindowWidth - line.Length) / 2);
    //         //  Clear the current line and set cursor position
    //         Console.SetCursorPosition(0, top);
    //         Console.Write(new string(' ', Console.WindowWidth - 1));
    //         Console.SetCursorPosition(leftPad, top);
    //         // Display the options with the selected one highlighted
    //         for (int i = 0; i < options.Length; i++)
    //         {
    //             // Highlight the selected option
    //             if (i == selected)
    //             {
    //                 Console.ForegroundColor = ConsoleColor.Black;
    //                 Console.BackgroundColor = ConsoleColor.DarkRed;
    //                 Console.Write($" {options[i]} ");
    //                 Console.ResetColor();
    //                 Console.Write(" ");
    //             }
    //             else
    //             {
    //                 Console.Write($"  {options[i]}  ");
    //             }
    //         }
    //         // Read user input
    //         var key = Console.ReadKey(true);
    //         if (key.Key == ConsoleKey.LeftArrow || key.KeyChar == 'a' || key.KeyChar == 'A' ||
    //             key.Key == ConsoleKey.UpArrow || key.KeyChar == 'w' || key.KeyChar == 'W')
    //         {
    //             if (selected > 0) selected--;
    //         }
    //         else if (key.Key == ConsoleKey.RightArrow || key.KeyChar == 'd' || key.KeyChar == 'D' ||
    //                 key.Key == ConsoleKey.DownArrow || key.KeyChar == 's' || key.KeyChar == 'S')
    //         {
    //             if (selected < options.Length - 1) selected++;
    //         }
    //         else if (key.Key == ConsoleKey.Enter)
    //         {
    //             Console.SetCursorPosition(0, top + 2);

    //             switch (selected)
    //             {
    //                 case 0:
    //                     // TODO: Call Login method here
    //                     Console.WriteLine("[Login placeholder]");
    //                     break;
    //                 case 1:
    //                     // TODO: Call Register method here
    //                     Console.WriteLine("[Register placeholder]");
    //                     break;
    //                 case 2:
    //                     // TODO: Call ContinueAsGuest method here
    //                     Console.WriteLine("[Continue as Guest placeholder]");
    //                     break;
    //                 case 3:
    //                     // TODO: Call Exit method here
    //                     Environment.Exit(0);
    //                     break;
    //             }
    //             break;
    //         }
    //         else if (key.Key == ConsoleKey.Escape)
    //         {
    //             Environment.Exit(0);
    //             break;
    //         }
    //     }

    //     Console.CursorVisible = true;
    // }

    /// <summary>
    /// Displays the start screen with the supermarket name and menu options.
    /// </summary>

    // public static string CenterAscii(string asciiArt)
    // {
    //     if (string.IsNullOrEmpty(asciiArt)) return asciiArt;

    //     int width = Console.WindowWidth;
    //     var lines = asciiArt.Split('\n');
    //     var centered = new List<string>();

    //     foreach (var line in lines)
    //     {
    //         string trimmed = line.TrimEnd('\r'); 
    //         int totalPadding = width - trimmed.Length;
    //         if (totalPadding < 0) totalPadding = 0;

    //         int leftPadding = totalPadding / 2;
    //         int rightPadding = totalPadding - leftPadding;

    //         string padded = new string(' ', leftPadding) + trimmed + new string(' ', rightPadding);
    //         centered.Add(padded);
    //     }

    //     return string.Join(Environment.NewLine, centered);
    // }
}