using System.Net;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography.X509Certificates;
using Spectre.Console;

public static class LoginUI
{
    public static readonly Color Hover = Color.FromHex("#006494");
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    private static int attempts = 0;
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
                while (true)
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
                    attempts = 0;
                    // Check for birthday
                    var user = SessionManager.CurrentUser;
                    if (user.Birthdate.Month == DateTime.Today.Month &&
                        user.Birthdate.Day == DateTime.Today.Day)
                    {
                        // Check if they already got a gift this year
                        if (user.LastBirthdayGift == null || user.LastBirthdayGift.Value.Year < DateTime.Today.Year)
                        {
                            OrderLogic.AddBirthdayGiftToCart(user);

                            // Update last birthday gift date
                            user.LastBirthdayGift = DateTime.Today;
                            LoginLogic.UpdateLastBirthdayGiftDate(user.ID, user.LastBirthdayGift.Value);
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[yellow] You’ve already received your birthday gift this year![/]");
                        }
                    }
                }
            }
            else
            {
                // Ask twice then option to forget password

                Console.Clear();
                AnsiConsole.Write(
                new FigletText("Error")
                    .Centered()
                    .Color(AsciiPrimary));
                AnsiConsole.MarkupLine("[red]Login failed! Please check your email and password.[/]");
                Console.ReadKey();
                attempts++;

                if (attempts == 2)
                {
                    // option to go back or reset password
                    Console.Clear();
                    AnsiConsole.Write(
                        new FigletText("Supermarket App")
                            .Centered()
                            .Color(AsciiPrimary));
                    var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .AddChoices(new[] { "Reset Password", "Back to Main Menu" }));
                    switch (choice)
                    {
                    case "Reset Password":
                        for (int resetAttempts = 0; resetAttempts < 2; resetAttempts++)
                        {
                            email = AnsiConsole.Prompt(new TextPrompt<string>(
                                $"What's your [green]email[/]? (you have {2 - resetAttempts} attempts left):"));

                            var result = LoginLogic.GetUserByEmail(email);

                            if (result == null)
                            {
                                AnsiConsole.MarkupLine("[red]Email not found![/]");

                                // If this was their last allowed attempt
                                if (resetAttempts == 1)
                                {
                                    AnsiConsole.MarkupLine("[red]Too many failed attempts! Returning to main menu.[/]");
                                    Console.ReadKey();
                                    attempts = 0;
                                    return;
                                }
                                continue;
                            }
                            // Send 2FA code to email using logic layer
                            LoginLogic.ForgetPassword2FAEmail(result.ID, result.Email);
                            // Type in the 2FA code
                            AnsiConsole.MarkupLine("[italic yellow]A 2FA code has been sent to your email![/]");
                            while (true)
                            {
                                // Exit the loop option
                                string inputCode = AnsiConsole.Prompt(new TextPrompt<string>("Enter the [bold yellow]2FA[/] code sent to your email(or '[bold red]EXIT[/]' to [red]exit[/]):"));
                                if (inputCode.ToUpper() == "EXIT")
                                {
                                    return;
                                }
                                // Wrong code entered
                                if (!TwoFALogic.Validate2FACode(result.ID, inputCode))
                                {
                                    AnsiConsole.MarkupLine("[red]Error: Entered wrong code or code expired[/]");
                                }
                                // Correct code entered
                                else
                                {
                                    string newPassword;
                                    do
                                    {
                                        AnsiConsole.MarkupLine("[blue]Password must contain at least 1 digit and has to be 6 characters long (Example: Cheese1).[/]");
                                        newPassword = AnsiConsole.Prompt(
                                            new TextPrompt<string>("Create a new password:")
                                                .Secret());
                                    } while (ValidaterLogic.ValidatePassword(newPassword) == false);
                                    // Update password in database
                                    result.Password = newPassword;
                                    LoginLogic.UpdateUserPassword(result.ID, newPassword);
                                    AnsiConsole.MarkupLine("[green]Password reset successful! You can now log in with your new password.[/]");
                                    AnsiConsole.MarkupLine("[yellow]Press any key to continue to the main menu...[/]");
                                    attempts = 0;
                                    Console.ReadKey();
                                    return;
                                }
                            }
                        }
                        break;
                    case "Back to Main Menu":
                        attempts = 0;
                            break;
                    }
                }
            }

        
    }
// why after i click reset password and do it wrong twice and then go to main menu and do it wrong twice i dont go into forget password
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

                if (!ValidaterLogic.ValidateEmail(email))
                {
                    AnsiConsole.MarkupLine("[red]Invalid email format! Please try again.[/]");
                    continue;
                }

                if (UserAccess.EmailExists(email))
                {
                    AnsiConsole.MarkupLine($"[red]The email [yellow]{email}[/] is already registered. Please use a different one![/]");
                    continue;
                }
                break;

            } while (true);
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

            DateTime Birthdate;

            while (true)
            {
                string birthdateInput = AnsiConsole.Prompt(
                    new TextPrompt<string>("What's your birthdate? (DD-MM-YYYY)")
                );

                // First check format
                if (!System.Text.RegularExpressions.Regex.IsMatch(birthdateInput, @"^\d{2}-\d{2}-\d{4}$"))
                {
                    AnsiConsole.MarkupLine("[red]Invalid format. Please use DD-MM-YYYY (e.g., 25-12-2005).[/]");
                    continue;
                }

                // Then check if it's a real date
                if (!DateTime.TryParseExact(
                        birthdateInput,
                        "dd-MM-yyyy",
                        null,
                        System.Globalization.DateTimeStyles.None,
                        out Birthdate))
                {
                    AnsiConsole.MarkupLine("[red]Date is out of range — please enter a valid calendar date.[/]");
                    continue;
                }

                // Check logical range (0–100 years)
                if (!ValidaterLogic.ValidateDateOfBirth(Birthdate))
                {
                    AnsiConsole.MarkupLine("[red]Birthdate must be between 0 and 100 years old.[/]");
                    continue;
                }

                break;
            }


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

            List<string> Errors = LoginLogic.Register(name, lastName, email, password, Address, Zipcode, PhoneNumber, Birthdate, City, is2FAEnabled);
            
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