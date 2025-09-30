using Spectre.Console;
public class UserMenu
{
    public static void StartMenu(bool isAdmin)
    {
        if (isAdmin)
        {
            AdminMenu();
        }
        else if (!isAdmin)
        {
            CustomerMenu();
        }
    }

    public static void AdminMenu()
    {
        // Admin menu
        Console.Clear();
        var adminMenuChoice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Admin Menu")
            .PageSize(10)
            .AddChoices(new[] {
                "Storage Menu",
        }));

        if (adminMenuChoice == "Storage Menu")
        {
            StorageMenu();
        }
    }


    public static void StorageMenu()
    {
        Console.Clear();
        var adminStorageChoice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Storage Menu")
            .PageSize(10)
            .AddChoices(new[] {
                "Add Product","Delete Product", "Edit Product"
        }));

        // Echo the fruit back to the terminal
        AnsiConsole.WriteLine($"This feature: {adminStorageChoice} is not available yet.");
    }












    public static void CustomerMenu()
    {
        // User menu
        var userMenuChoice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .PageSize(10)
            .AddChoices(new[] {
                "Products","Order", "Map", "Store Information"
        }));
        AnsiConsole.WriteLine($"This feature: {userMenuChoice} is not available yet.");

    }

}