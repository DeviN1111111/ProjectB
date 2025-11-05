using Spectre.Console;
public static class SettingsUI
{
    public static readonly Color Hover = Color.FromHex("#006494");
    public static void ShowSettingsMenu()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("Settings Menu")
                    .Centered()
                    .Color(MenuUI.AsciiPrimary));


            AnsiConsole.MarkupLine($"[italic yellow]Changing settings for:[/] [bold green]{SessionManager.CurrentUser.Name}[/]");
            Console.WriteLine();

            var options = new List<string> { "Security Settings", "Go back" }; ;

            var prompt = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .PageSize(10)
                .AddChoices(options));

            switch (prompt)
            {
                case "Security Settings":
                    SecuritySettingsMenu();
                    break;
                case "Go back":
                    return;
            }
        }
    }

    public static void SecuritySettingsMenu()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("Security Settings Menu")
                    .Centered()
                    .Color(MenuUI.AsciiPrimary));


            AnsiConsole.MarkupLine($"[italic yellow]Changing Security settings for:[/] [bold green]{SessionManager.CurrentUser.Name}[/]");
            Console.WriteLine();

            var options = new List<string> { "2FA Settings", "Go back" }; ;

            var prompt = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .PageSize(10)
                .AddChoices(options));

            switch (prompt)
            {
                case "2FA Settings":
                    TWOFASettingsMenu();
                    break;
                case "Go back":
                    return;
            }
        }
    }

    public static void TWOFASettingsMenu()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("2FA Settings Menu")
                    .Centered()
                    .Color(MenuUI.AsciiPrimary));


            AnsiConsole.MarkupLine($"[italic yellow]Changing 2FA settings for: [/][bold green]{SessionManager.CurrentUser.Name}[/]");
            Console.WriteLine();
            AnsiConsole.MarkupLine($"[italic yellow]Current 2FA Status: [/][blue]{SessionManager.CurrentUser.TwoFAEnabled.ToString()}[/]");
            Console.WriteLine();

            var options = new List<string> { "Enable 2FA", "Disable 2FA", "Go back" };

            var prompt = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .PageSize(10)
                .AddChoices(options));

            switch (prompt)
            {
                case "Enable 2FA":
                    Enable2FA();
                    break;
                case "Disable 2FA":
                    Disable2FA();
                    break;
                case "Go back":
                    return;
            }
        }
    }

    public static void Enable2FA()
    {
        if(SessionManager.CurrentUser.TwoFAEnabled)
        {
            AnsiConsole.MarkupLine("[italic yellow]2FA Already enabled press [green]ANY KEY[/] to continue[/]");
            Console.ReadKey();
            return;
        }
        bool is2FAEnabled = false;
        AnsiConsole.MarkupLine("Are you sure you want to enable 2FA?");
        var wantToEnable2FA = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { "Yes", "No" }));

        if (wantToEnable2FA == "Yes")
        {
            AnsiConsole.MarkupLine($"[italic yellow]A 2FA code has been sent to your email([italic green]{SessionManager.CurrentUser.Email}[/])![/]");
            string Register2FACode;
            string correct2FACode = TwoFALogic.Register2FAEmail(SessionManager.CurrentUser.Email).Result;
            do
            {
                Register2FACode = AnsiConsole.Prompt(new TextPrompt<string>("Enter the [bold yellow]2FA[/] code sent to your email(or '[bold red]EXIT[/]' to [red]exit[/]):"));
                if (Register2FACode.ToLower() == "exit")
                {
                    AnsiConsole.MarkupLine("[italic yellow]2FA Settings have not been changed press [green]ANY KEY[/] to continue[/]");
                    Console.ReadKey();
                    return;
                }
                else if (Register2FACode != correct2FACode)
                {
                    AnsiConsole.MarkupLine("[red]Error: Entered wrong code[/]");
                }
            } while (Register2FACode != correct2FACode);

            TwoFALogic.Enable2FA(SessionManager.CurrentUser.ID);
            AnsiConsole.MarkupLine("[green]2FA has been enabled for your account![/]");
        }
        else
        {
            return;
        }
    }

    public static void Disable2FA()
    {
        if(!SessionManager.CurrentUser.TwoFAEnabled)
        {
            AnsiConsole.MarkupLine("[italic yellow]2FA Already [bold red]DISABLED[/] press [green]ANY KEY[/] to continue[/]");
            Console.ReadKey();
            return;
        }
        AnsiConsole.MarkupLine("Are you sure you want to disable 2FA?");
        var wantToEnable2FA = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { "Yes", "No" }));

        if (wantToEnable2FA == "Yes")
        {
            TwoFALogic.Disable2FA(SessionManager.CurrentUser.ID);
            AnsiConsole.MarkupLine("[green] 2FA [red]Disabled[/] succesfully[/]");
            Console.ReadKey();
        }
        else
        {
            return;
        }
    }
}