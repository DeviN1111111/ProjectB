using System.Security.Cryptography.X509Certificates;
using Spectre.Console;

public class ManageAdminUI
{
    public static readonly Color Text = Color.FromHex("#E8F1F2");
    public static readonly Color Hover = Color.FromHex("#006494");
    public static readonly Color Confirm = Color.FromHex("#13293D");
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static readonly Color AsciiSecondary = Color.FromHex("#1B98E0");
    public static void DisplayMenu()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("Manage Users")
                    .Centered()
                    .Color(AsciiPrimary));

            var period = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .HighlightStyle(new Style(Hover))
                    .AddChoices(new[] { "Add User", "Delete User", "Change Role", "Go back" }));

            switch (period)
            {
                case "Go back":
                    return;

                case "Add User":
                    AddUser();
                    break;

                case "Delete User":
                    DeleteUser();
                    break;

                case "Change Role":
                    ChangeRole();
                    break;

                default:
                    AnsiConsole.MarkupLine("[red]Invalid selection[/]");
                    break;
            }
        }
    }

    public static void ChangeRole()
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Change Role")
                .Centered()
                .Color(AsciiPrimary));

        List<UserModel> AllUsers = AdminLogic.GetAllUsers();
        List<string> UsersToDeleteList = [];
        UsersToDeleteList.Add("Go back");
        foreach (UserModel user in AllUsers)
        {
            UsersToDeleteList.Add($"{user.ID} / {user.Name} / {user.AccountStatus}");
        }

        AnsiConsole.WriteLine();

        var UserToChangeRole = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Select a User to change role.[/]")
                .PageSize(10)
                .AddChoices(UsersToDeleteList));

        if (UserToChangeRole == "Go back")
        {
            return;
        }

        var NewRole = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Select a role.[/]")
                .PageSize(10)
                .AddChoices("User", "Admin", "SuperAdmin", "Go back"));
                
        if (NewRole == "Go back")
        {
            return;
        }

        string[] UserID = UserToChangeRole.Replace(" ", "").Split("/");
        if(AdminLogic.ChangeRole(Convert.ToInt32(UserID[0]), NewRole) == true)
        {
            AnsiConsole.MarkupLine("[green]Succesfully changed user role.[/]");
            Console.ReadKey();
        }
        else
        {
            Console.Clear();
            AnsiConsole.Write(
            new FigletText("ERROR")
                .Centered()
                .Color(AsciiPrimary));
            AnsiConsole.MarkupLine("[yellow]You can't change your own role![/]");
            Console.ReadKey();
        }
    }

    public static void DeleteUser()
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Delete User")
                .Centered()
                .Color(AsciiPrimary));

        List<UserModel> AllUsers = AdminLogic.GetAllUsers();
        List<string> UsersToDeleteList = [];
        UsersToDeleteList.Add("Go back");
        foreach (UserModel user in AllUsers)
        {
            UsersToDeleteList.Add($"{user.ID} / {user.Name} / {user.AccountStatus}");
        }

        AnsiConsole.WriteLine();

        var UserToDelete = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Select a User to delete.[/]")
                .PageSize(10)
                .AddChoices(UsersToDeleteList));
        
        if (UserToDelete == "Go back")
        {
            return;
        }
        AnsiConsole.WriteLine();

        string[] NewDelete = UserToDelete.Replace(" ", "").Split("/");

        AnsiConsole.MarkupLine($"Are you sure you want to delete [red]{NewDelete[1]}[/]? This action cannot be undone.");
        bool ConfirmForLines = false;

        var Confirm = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { "Confirm", "Cancel" }));
        
        switch (Confirm)
        {
            case "Confirm":
                ConfirmForLines = AdminLogic.DeleteUser(Convert.ToInt32(NewDelete[0]));
                break;
            case "Cancel":
                return;
        }

        if (ConfirmForLines)
        {
            AnsiConsole.MarkupLine("[green]Successfully deleted user.[/]");
            Console.ReadKey();
        }
        else
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("ERROR")
                    .Centered()
                    .Color(AsciiPrimary));
            AnsiConsole.MarkupLine("[yellow]You can't delete yourself![/]");
            Console.ReadKey();
        }
    }

    public static void AddUser()
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Add User")
                .Centered()
                .Color(AsciiPrimary));

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
            string AccountStatus;
            do
            {
                AnsiConsole.MarkupLine("[blue]Choose between User and Admin[/]");
                AccountStatus = AnsiConsole.Prompt(new TextPrompt<string>("What role should the account have?"));
            } while (AccountStatus != "User" && AccountStatus != "Admin");
            DateTime Birthdate;
            while (true)
            {
                // Prompt for birthdate until valid
                string birthdateInput = AnsiConsole.Prompt(new TextPrompt<string>("What's your birthdate? (DD-MM-YYYY)"));
                if (!DateTime.TryParseExact(
                        birthdateInput,
                        "dd-MM-yyyy", 
                        null, System.Globalization.DateTimeStyles.None, out Birthdate))
                {
                    break;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Invalid date format. Please use DD-MM-YYYY.[/]");
                }
            }
            List<string> Errors = LoginLogic.Register(name, lastName, email, password, Address, Zipcode, PhoneNumber,Birthdate, City, false, AccountStatus);
            if (Errors.Count == 0)
            {
                AnsiConsole.MarkupLine("[green]Registration successful![/]");
                Console.ReadKey();
                break;
            }
        }
    }
}