using Spectre.Console;
public static class SettingsUI
{
    public static void ShowSettingsMenu()
    {
        while (true)
        {
            Console.Clear();
            Utils.PrintTitle("Settings Menu");

            AnsiConsole.MarkupLine($"[italic yellow]Changing settings for:[/] [bold green]{SessionManager.CurrentUser.Name}[/]");
            Console.WriteLine();

            var options = new List<string> { "Security Settings", "Profile Settings", "Go back" }; ;

            var prompt = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .PageSize(10)
                .AddChoices(options));

            switch (prompt)
            {
                case "Security Settings":
                    SecuritySettingsMenu();
                    break;
                case "Profile Settings":
                    ProfileSettingsMenu();
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
            Utils.PrintTitle("Security Settings Menu");

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
            Utils.PrintTitle("2FA Settings Menu");

            AnsiConsole.MarkupLine($"[italic yellow]Changing 2FA settings for: [/][bold green]{SessionManager.CurrentUser.Name}[/]");
            Console.WriteLine();
            AnsiConsole.MarkupLine($"[italic yellow]Current 2FA Status: [/][blue]{TwoFALogic.Is2FAEnabled(SessionManager.CurrentUser.ID).ToString()}[/]");
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
        if(TwoFALogic.Is2FAEnabled(SessionManager.CurrentUser.ID))
        {
            AnsiConsole.MarkupLine("[italic yellow]2FA Already enabled press [green]ANY KEY[/] to continue[/]");
            Console.ReadKey();
            return;
        }
        AnsiConsole.MarkupLine("Are you sure you want to enable 2FA?");
        var wantToEnable2FA = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(ColorUI.Hover))
                .AddChoices(new[] { "Yes", "No" }));

        if (wantToEnable2FA == "Yes")
        {
            AnsiConsole.MarkupLine($"[italic yellow]A 2FA code has been sent to your email([italic green]{SessionManager.CurrentUser.Email}[/])![/]");
            string Register2FACode;
            string correct2FACode = TwoFALogic.Register2FAEmail(SessionManager.CurrentUser.Email).Result;
            do
            {
                Register2FACode = AnsiConsole.Prompt(new TextPrompt<string>("Enter the [bold yellow]2FA[/] code sent to your email to [green]enable[/] 2FA(or '[bold red]EXIT[/]' to [red]exit[/]):"));
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
            SessionManager.UpdateCurrentUser(SessionManager.CurrentUser.ID);
            AnsiConsole.MarkupLine("[green]2FA has been enabled for your account![/]");
            Console.ReadKey();
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
                .HighlightStyle(new Style(ColorUI.Hover))
                .AddChoices(new[] { "Yes", "No" }));

        if (wantToEnable2FA == "Yes")
        {
            TwoFALogic.Disable2FA(SessionManager.CurrentUser.ID);
            SessionManager.UpdateCurrentUser(SessionManager.CurrentUser.ID);
            AnsiConsole.MarkupLine("[green] 2FA [red]Disabled[/] succesfully[/]");
            Console.ReadKey();
        }
        else
        {
            return;
        }
    }

    public static void ProfileSettingsMenu()
    {
        while (true)
        {
            Console.Clear();
            Utils.PrintTitle("Profile Settings Menu");

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddRow("Name:", $"[blue]{SessionManager.CurrentUser!.Name}[/]");
            grid.AddRow("Lastname:", $"[blue]{SessionManager.CurrentUser.LastName}[/]");
            grid.AddRow("Email:", $"[blue]{SessionManager.CurrentUser.Email}[/]");
            grid.AddRow("Address:", $"[blue]{SessionManager.CurrentUser.Address}[/]");
            grid.AddRow("Zipcode:", $"[blue]{SessionManager.CurrentUser.Zipcode}[/]");
            grid.AddRow("City:", $"[blue]{SessionManager.CurrentUser.City}[/]");
            grid.AddRow("Phonenumber:", $"[blue]{SessionManager.CurrentUser.PhoneNumber}[/]");
            grid.AddRow("Birthdate:", $"[blue]{SessionManager.CurrentUser.Birthdate.ToString("dd-MM-yyyy")}[/]");
            var panel = new Panel(grid)
                .Header("[bold cyan]Account details[/]")
                .Border(BoxBorder.Double);
            AnsiConsole.Write(panel);

            Console.WriteLine();
            var options = new List<string> { "Change Profile Settings", "Go back" }; ;

            var prompt = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .PageSize(10)
                .AddChoices(options));

            switch (prompt)
            {
                case "Change Profile Settings":
                    ChangeProfileSettingsMenu();
                    break;
                case "Go back":
                    return;
            }
        }
    }

    public static void ChangeProfileSettingsMenu()
    {
        while (true)
        {
            Console.Clear();
            Utils.PrintTitle("Profile Settings Menu");

            string NewName = AnsiConsole.Prompt(new TextPrompt<string>("Enter your [bold yellow]Name[/]:").DefaultValue(SessionManager.CurrentUser!.Name).DefaultValue(SessionManager.CurrentUser.Name));
            string NewLastName = AnsiConsole.Prompt(new TextPrompt<string>("Enter your [bold yellow]Last Name[/]:").DefaultValue(SessionManager.CurrentUser.LastName).DefaultValue(SessionManager.CurrentUser.LastName));
            string NewEmail;
            do
            {
                AnsiConsole.MarkupLine("[blue]Email must contain @ and a dot.[/]");
                NewEmail = AnsiConsole.Prompt(new TextPrompt<string>("Enter your [bold yellow]Email[/]:").DefaultValue(SessionManager.CurrentUser.Email));

                if (NewEmail == SessionManager.CurrentUser.Email)
                {
                    break;
                }

                if (!ValidaterLogic.ValidateEmail(NewEmail))
                {
                    AnsiConsole.MarkupLine("[red]Invalid email format! Please try again.[/]");
                    continue;
                }

                if (UserSettingsLogic.EmailExists(NewEmail))
                {
                    AnsiConsole.MarkupLine($"[red]The email [yellow]{NewEmail}[/] is already registered. Please use a different one![/]");
                    continue;
                }
                break;

            } while (true);

            string NewAddress = AnsiConsole.Prompt(new TextPrompt<string>("Enter your [bold yellow]Address[/]:").DefaultValue(SessionManager.CurrentUser.Address).DefaultValue(SessionManager.CurrentUser.Address));
            string NewZipcode;
            do
            {
                AnsiConsole.MarkupLine("[blue]Zipcode must be in the format 0000AB (Example: 2353TL).[/]");
                NewZipcode = AnsiConsole.Prompt(new TextPrompt<string>("Enter your [bold yellow]Zipcode[/]:").DefaultValue(SessionManager.CurrentUser.Zipcode).DefaultValue(SessionManager.CurrentUser.Zipcode));
            } while (ValidaterLogic.ValidateZipcode(NewZipcode) == false);
            
            string NewCity = AnsiConsole.Prompt(new TextPrompt<string>("Enter your [bold yellow]City[/]:").DefaultValue(SessionManager.CurrentUser.City).DefaultValue(SessionManager.CurrentUser.City));
            string NewPhoneNumber;
            do
            {
                AnsiConsole.MarkupLine("[blue]Phonenumber must have 10 digits (Example: 1234567890).[/]");
                NewPhoneNumber = AnsiConsole.Prompt(new TextPrompt<string>("Enter your [bold yellow]Phone Number[/]:").DefaultValue(SessionManager.CurrentUser.PhoneNumber).DefaultValue(SessionManager.CurrentUser.PhoneNumber));
            } while (ValidaterLogic.ValidatePhoneNumber(NewPhoneNumber) == false);

            string newBirthdate;
            DateTime Birthdate;
            do
            {
                AnsiConsole.MarkupLine("[blue]Birthday must be in this format (DD-MM-YYYY).[/]");
                newBirthdate = AnsiConsole.Prompt(new TextPrompt<string>("Enter your [bold yellow]Birthdate[/]:").DefaultValue(SessionManager.CurrentUser.Birthdate.ToString("dd-MM-yyyy")));
            } while (DateTime.TryParse(newBirthdate, out Birthdate) == false);



            AnsiConsole.WriteLine();
            Table beforeEditTable = Utils.CreateTable(new [] { "Field", "Value" }, "[red]Before EDIT:[/]");

            beforeEditTable.AddRow("Name", $"[red]{SessionManager.CurrentUser.Name}[/]");
            beforeEditTable.AddRow("Lastname", $"[red]{SessionManager.CurrentUser.LastName}[/]");
            beforeEditTable.AddRow("Email", $"[red]{SessionManager.CurrentUser.Email}[/]");
            beforeEditTable.AddRow("Address", $"[red]{SessionManager.CurrentUser.Address}[/]");
            beforeEditTable.AddRow("Zipcode", $"[red]{SessionManager.CurrentUser.Zipcode}[/]");
            beforeEditTable.AddRow("City", $"[red]{SessionManager.CurrentUser.City}[/]");
            beforeEditTable.AddRow("Phonenumber", $"[red]{SessionManager.CurrentUser.PhoneNumber}[/]");
            beforeEditTable.AddRow("Birthdate", $"[red]{SessionManager.CurrentUser.Birthdate:dd-MM-yyyy}[/]");

            Table afterTable = Utils.CreateTable(new [] { "Field", "Value" }, "[green]After EDIT:[/]");

            afterTable.AddRow("Name", $"[green]{NewName}[/]");
            afterTable.AddRow("Lastname", $"[green]{NewLastName}[/]");
            afterTable.AddRow("Email", $"[green]{NewEmail}[/]");
            afterTable.AddRow("Address", $"[green]{NewAddress}[/]");
            afterTable.AddRow("Zipcode", $"[green]{NewZipcode}[/]");
            afterTable.AddRow("City", $"[green]{NewCity}[/]");
            afterTable.AddRow("Phonenumber", $"[green]{NewPhoneNumber}[/]");
            afterTable.AddRow("Birthdate", $"[green]{Birthdate:dd-MM-yyyy}[/]");

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddRow(beforeEditTable, afterTable);
            
            var panel = new Panel(grid)
                .Header("[bold cyan]Account details Edit Comparison[/]")
                .Border(BoxBorder.Double);
                AnsiConsole.Write(panel);

            AnsiConsole.MarkupLine("Are you sure you want to apply these changes?");
            var confirmChange = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .HighlightStyle(new Style(ColorUI.Hover))
                    .AddChoices(new[] { "Yes", "No" }));

            if (confirmChange == "No")
            {
                AnsiConsole.MarkupLine("Profile settings [red]have not been changed[/] press [green]ANY KEY[/] to continue");
                Console.ReadKey();
                break;
            }

            bool change = UserSettingsLogic.ChangeProfileSettings(NewName, NewLastName, NewEmail, NewAddress, NewZipcode, NewPhoneNumber, NewCity, Birthdate);
            if (change)
            {
                AnsiConsole.MarkupLine("[green]Profile settings changed successfully![/]");
                AnsiConsole.MarkupLine("Press [green]ANY KEY[/] to continue");
                Console.ReadKey();
                break;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Error: Could not change profile settings. Please check your input and try again.[/]");
                AnsiConsole.MarkupLine("Press [green]ANY KEY[/] to continue");
                Console.ReadKey();
                break;
            }
        }
    }
}