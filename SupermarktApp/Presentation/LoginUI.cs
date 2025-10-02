using Spectre.Console;

public static class LoginUI
{
    public static UserModel Login()
    {
        string email = AnsiConsole.Prompt(new TextPrompt<string>("What's your email?"));
        string password = AnsiConsole.Prompt(
            new TextPrompt<string>("What's your password?")
                .Secret());

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
            return null!;
        }
    }

    public static void Register()
    {
        string name = AnsiConsole.Prompt(new TextPrompt<string>("What's your first name?"));
        string lastName = AnsiConsole.Prompt(new TextPrompt<string>("What's your last name?"));
        string email = AnsiConsole.Prompt(new TextPrompt<string>("What's your email?"));
        string password = AnsiConsole.Prompt(
            new TextPrompt<string>("Create a password:")
                .Secret());
        string Address = AnsiConsole.Prompt(new TextPrompt<string>("What's your street name?"));
        string Zipcode = AnsiConsole.Prompt(new TextPrompt<string>("What's your zipcode?"));
        string PhoneNumber = AnsiConsole.Prompt(new TextPrompt<string>("What's your phone number?"));
        string City = AnsiConsole.Prompt(new TextPrompt<string>("What's your city?"));

        bool Account = LoginLogic.Register(name, lastName, email, password, Address, Zipcode, PhoneNumber, City);
        if (Account)
        {
            AnsiConsole.MarkupLine("[green]Registration successful! You can now log in.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Registration failed! Please check your email and password.[/]");
            AnsiConsole.MarkupLine("[yellow]Hint: Email must contain '@' and '.' characters.[/]");
            AnsiConsole.MarkupLine("[yellow]Hint: Password must be at least 6 characters long and contain at least one digit.[/]");
            AnsiConsole.MarkupLine("[yellow]Hint: Phone number must be exactly 10 digits long.[/]");
        }
    }
}