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

    public static bool ValidateHouseNumber(int houseNumber)
    {
        return houseNumber > 0;
    }

    public static bool ValidatePhoneNumber(int phoneNumber)
    {
        string stringPhoneNumber = phoneNumber.ToString();
        return stringPhoneNumber.Length == 10;
    } 
}