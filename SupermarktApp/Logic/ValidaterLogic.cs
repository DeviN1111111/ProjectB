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
        string zipcodeTrim = zipcode.Replace(" ", "");
        if (zipcodeTrim.Length != 6)
        {
            return false;
        }
        if (char.IsDigit(zipcodeTrim[0]) && char.IsDigit(zipcodeTrim[1]) && char.IsDigit(zipcodeTrim[2]) && char.IsDigit(zipcodeTrim[3]) && char.IsLetter(zipcodeTrim[4]) && char.IsLetter(zipcodeTrim[5]))
        {
            return true;
        }
        return false;
    } 
}