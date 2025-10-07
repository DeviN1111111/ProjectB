public static class ValidaterLogic
{
    public static bool ValidateEmail(string email)
    {
        return email.Contains("@") && email.Contains(".");
    }

    public static bool ValidatePassword(string password)
    {
        return password.Length >= 6 && password.Any(char.IsDigit);
    }

    public static bool ValidatePhoneNumber(string phoneNumber)
    {
        return phoneNumber.Length == 10;
    }

    public static bool ValidateZipcode(string zipcode)
    {
        if (zipcode.Length != 6)
        {
            return false;
        }
        if (char.IsDigit(zipcode[0]) && char.IsDigit(zipcode[1]) && char.IsDigit(zipcode[2]) && char.IsDigit(zipcode[3]) && char.IsLetter(zipcode[4]) && char.IsLetter(zipcode[5]))
        {
            return true;
        }
        return false;
    } 
}