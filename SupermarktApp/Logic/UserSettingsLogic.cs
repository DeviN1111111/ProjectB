public class UserSettingsLogic
{
    public static bool ChangeProfileSettings(string name, string lastname, string email, string address ,string zipcode, string phonenumber, string city, DateTime birthdate)
    {
        if (!ValidaterLogic.ValidateEmail(email))
            return false;
        if (!ValidaterLogic.ValidatePhoneNumber(phonenumber))
            return false;
        if (!ValidaterLogic.ValidateZipcode(zipcode))
            return false;

        UserAccess.ChangeProfileSettings(SessionManager.CurrentUser!.ID, name, lastname, email, address, zipcode, phonenumber, city, birthdate);
        SessionManager.CurrentUser.Name = name;
        SessionManager.CurrentUser.LastName = lastname;
        SessionManager.CurrentUser.Email = email;
        SessionManager.CurrentUser.Address = address;
        SessionManager.CurrentUser.Zipcode = zipcode;
        SessionManager.CurrentUser.PhoneNumber = phonenumber;
        SessionManager.CurrentUser.City = city;
        SessionManager.CurrentUser.Birthdate = birthdate;
        return true;
    }

    public static bool EmailExists(string email)
    {
        return UserAccess.EmailExists(email);
    }
}