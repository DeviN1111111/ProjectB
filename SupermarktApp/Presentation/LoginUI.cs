using System.Net;
using Spectre.Console;

public static class LoginUI
{
    public static UserModel Login()
    {
        string email = AnsiConsole.Prompt(new TextPrompt<string>("What's your email?"));
        string password = AnsiConsole.Prompt(
            new TextPrompt<string>("What's your password?")
                .Secret());

        Console.Clear();
        UserModel Account = LoginLogic.Login(email, password);
        if (Account != null)
        {
            AnsiConsole.MarkupLine("[green]Login successful![/]");
            AnsiConsole.MarkupLine($"[blue]Welcome, {Account.Name} {Account.LastName}![/]");
            return Account;
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Login failed! Please check your email and password.[/]");
            Console.ReadKey();
            return null!;
        }
    }

    public static void Register()
    {
        string name = AnsiConsole.Prompt(new TextPrompt<string>("What's your first name?"));
        string lastName = AnsiConsole.Prompt(new TextPrompt<string>("What's your last name?"));
        string email = AnsiConsole.Prompt(new TextPrompt<string>("What's your email?"));
        string password = AnsiConsole.Prompt(new TextPrompt<string>("Create a password:").Secret());
        string Address = AnsiConsole.Prompt(new TextPrompt<string>("What's your street name?"));
        string Zipcode = AnsiConsole.Prompt(new TextPrompt<string>("What's your zipcode?"));
        string PhoneNumber = AnsiConsole.Prompt(new TextPrompt<string>("What's your phone number?"));
        string City = AnsiConsole.Prompt(new TextPrompt<string>("What's your city?"));

        List<string> Errors = LoginLogic.Register(name, lastName, email, password, Address, Zipcode, PhoneNumber, City);
        if (Errors.Count == 0)
        {
            AnsiConsole.MarkupLine("[green]Registration successful! You can now log in.[/]");
            Console.ReadKey(true);
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Registration unsuccessful![/]");
            AnsiConsole.MarkupLine("[red]------------------------------------------------------------------------------------------------------[/]");
            foreach (string errorLine in Errors)
            {
                AnsiConsole.MarkupLine($"[yellow]{errorLine}[/]");
            }
            AnsiConsole.MarkupLine("[red]------------------------------------------------------------------------------------------------------[/]");
            Console.ReadKey(true);
        }
    }
}