using System.Net;
using System.Security.Cryptography.X509Certificates;
using Spectre.Console;

public static class LoginUI
{
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static void Login()
    {
        string email = AnsiConsole.Prompt(new TextPrompt<string>("What's your email?"));
        string password = AnsiConsole.Prompt(
            new TextPrompt<string>("What's your password?")
                .Secret());

        Console.Clear();
        UserModel Account = LoginLogic.Login(email, password);
        if (Account != null)
        {
            SessionManager.CurrentUser = Account;
            AnsiConsole.MarkupLine("[green]Login successful![/]");
            AnsiConsole.MarkupLine($"[blue]Welcome, {SessionManager.CurrentUser.Name} {SessionManager.CurrentUser.LastName}![/]");
        }
        else
        {
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
            AnsiConsole.Markup("[yellow]Press escape to return.[/]");
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

            List<string> Errors = LoginLogic.Register(name, lastName, email, password, Address, Zipcode, PhoneNumber, City);
            if (Errors.Count == 0)
            {
                AnsiConsole.MarkupLine("[green]Registration successful! You can now log in.[/]");
                Console.ReadKey();
                break;
            }
        }
    }
}