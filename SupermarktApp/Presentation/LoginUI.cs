using Spectre.Console;

public static class LoginUI
{
    public static void Login()
    {
        string email = AnsiConsole.Prompt(new TextPrompt<string>("What's your email?"));
        string password = AnsiConsole.Prompt(
            new TextPrompt<string>("What's your password?")
                .Secret());

        
    }
}