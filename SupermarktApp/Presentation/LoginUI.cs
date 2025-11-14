using System.Net;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography.X509Certificates;
using Spectre.Console;

public static class LoginUI
{
    public static readonly Color Hover = Color.FromHex("#006494");
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static void Login()
    {
        string email = AnsiConsole.Prompt(new TextPrompt<string>("What's your [green]email[/]?:"));
        string password = AnsiConsole.Prompt(
            new TextPrompt<string>("What's your [green]password[/]?:")
                .Secret());

        UserModel Account = LoginLogic.Login(email, password);
        if (Account != null)
        {
            if (TwoFALogic.Is2FAEnabled(Account.ID))
            {
                TwoFALogic.CreateInsertAndEmailSend2FACode(Account.ID);
                AnsiConsole.MarkupLine("[italic yellow]A 2FA code has been sent to your email![/]");
                while(true)
                {
                    string inputCode = AnsiConsole.Prompt(new TextPrompt<string>("Enter the [bold yellow]2FA[/] code sent to your email(or '[bold red]EXIT[/]' to [red]exit[/]):"));
                    if (inputCode.ToUpper() == "EXIT")
                    {
                        return;
                    }
                    if (!TwoFALogic.Validate2FACode(Account.ID, inputCode))
                    {
                        AnsiConsole.MarkupLine("[red]Error: Entered wrong code or code expired[/]");
                    }
                    else
                    {
                        SessionManager.CurrentUser = Account;
                        AnsiConsole.MarkupLine("[green]Login successful![/]");
                        AnsiConsole.MarkupLine("Press [green]ANY KEY[/] to continue...");
                        Console.ReadKey();
                        break;
                    }  
                }
            }
            else
            {
                SessionManager.CurrentUser = Account;
                AnsiConsole.MarkupLine("[green]Login successful![/]");
                AnsiConsole.MarkupLine($"[blue]Welcome, {SessionManager.CurrentUser.Name} {SessionManager.CurrentUser.LastName}![/]");
            }

        }
        else
        {
            Console.Clear();
            AnsiConsole.Write(
            new FigletText("Error")
                .Centered()
                .Color(AsciiPrimary));
            AnsiConsole.MarkupLine("[red]Login failed! Please check your email and password.[/]");
            Console.ReadKey();
        }
    }

    public static void Register()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[yellow]Press escape to return.[/]");
            AnsiConsole.MarkupLine("[green]Press any key to continue[/]");
            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                break;
            }
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("Supermarket App")
                    .Centered()
                    .Color(MenuUI.AsciiPrimary));

            string name = AnsiConsole.Prompt(new TextPrompt<string>("What's your first name?"));
            string lastName = AnsiConsole.Prompt(new TextPrompt<string>("What's your last name?"));
            string email;
            do
            {
                AnsiConsole.MarkupLine("[blue]Email must contain @ and a dot.[/]");
                email = AnsiConsole.Prompt(new TextPrompt<string>("What's your email?"));
            } while (ValidaterLogic.ValidateEmail(email) == false);
            string password;
            do
            {
                AnsiConsole.MarkupLine("[blue]Password must contain at least 1 digit and has to be 6 characters long (Example: Cheese1).[/]");
                password = AnsiConsole.Prompt(new TextPrompt<string>("Create a password:").Secret());
            } while (ValidaterLogic.ValidatePassword(password) == false);
            string Address = AnsiConsole.Prompt(new TextPrompt<string>("What's your street name?"));
            string Zipcode;
            do
            {
                AnsiConsole.MarkupLine("[blue]Zipcode must be in the format 0000AB (Example: 2353TL).[/]");
                Zipcode = AnsiConsole.Prompt(new TextPrompt<string>("What's your zipcode?"));
            } while (ValidaterLogic.ValidateZipcode(Zipcode) == false);
            string PhoneNumber;
            do
            {
                AnsiConsole.MarkupLine("[blue]Phonenumber must have 10 digits (Example: 1234567890).[/]");
                PhoneNumber = AnsiConsole.Prompt(new TextPrompt<string>("What's your phone number?"));
            } while (ValidaterLogic.ValidatePhoneNumber(PhoneNumber) == false);
            string City = AnsiConsole.Prompt(new TextPrompt<string>("What's your city?"));

            AnsiConsole.MarkupLine($"Do you want to [green]ENABLE[/] [yellow]2FA[/] for your account?");

            bool is2FAEnabled = false;
            var wantToEnable2FA = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { "Yes", "No" }));

            if (wantToEnable2FA == "Yes")
            {
                AnsiConsole.MarkupLine($"[italic yellow]A 2FA code has been sent to your email([italic green]{email}[/])![/]");
                string Register2FACode;
                string correct2FACode = TwoFALogic.Register2FAEmail(email).Result;
                do
                {
                    Register2FACode = AnsiConsole.Prompt(new TextPrompt<string>("Enter the [bold yellow]2FA[/] code sent to your email(or '[bold red]EXIT[/]' to [red]exit[/]):"));
                    if (Register2FACode.ToLower() == "exit")
                    {
                        return;
                    }
                    else if (Register2FACode != correct2FACode)
                    {
                        AnsiConsole.MarkupLine("[red]Error: Entered wrong code[/]");
                    }
                } while (Register2FACode != correct2FACode);
                
                is2FAEnabled = true;
                AnsiConsole.MarkupLine("[green]2FA has been enabled for your account![/]");
            }

            List<string> Errors = LoginLogic.Register(name, lastName, email, password, Address, Zipcode, PhoneNumber, City, is2FAEnabled);
            
            if (Errors.Count == 0)
            {
                AnsiConsole.MarkupLine("[green]Registration successful! You can now log in.[/]");
                AnsiConsole.MarkupLine("[yellow]Press any key to continue to the main menu...[/]");

                var createdUser = LoginLogic.GetUserByEmail(email);
                CouponLogic.CreateCoupon(createdUser!.ID, 5);
                Console.ReadKey();
                break;
            }
        }
    }
}