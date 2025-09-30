public static class ValidaterLogic
{
    public static bool ValidateEmail(string email)
    {
        // Simple email validation logic
        return email.Contains("@") && email.Contains(".");
    }

    public static bool ValidatePassword(string password)
    {
        // Check of wachtwoord minimaal 6 tekens lang is en ten minste één cijfer bevat
        return password.Length >= 6 && password.Any(char.IsDigit);
    }
}