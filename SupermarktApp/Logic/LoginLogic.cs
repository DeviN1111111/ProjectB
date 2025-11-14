using Spectre.Console;

public class LoginLogic
{
    public static UserModel Login(string email, string password)
    {
        UserModel? Account = LoginAccess.Login(email, password);

        if (Account != null)
        {
            return Account;
        }
        return null!;
    }

    public static List<string> Register(string name, string lastName, string email, string password, string address, string zipcode, string phoneNumber, string city, bool is2FAEnabled, string AccountStatus = "User")
    {
        UserModel user = new UserModel(name, lastName, email, password, address, zipcode, phoneNumber, city, is2FAEnabled, AccountStatus);

        List<string> Errors = [];

        if (!ValidaterLogic.ValidateEmail(user.Email))
            Errors.Add("Email invalid, must contain @ and a punctuation.");
        if (!ValidaterLogic.ValidatePassword(user.Password))
            Errors.Add("Password invalid, must contain atleast 1 digit and has to be 6 characters long (Example: Cheese1).");
        if (!ValidaterLogic.ValidatePhoneNumber(user.PhoneNumber))
            Errors.Add("Phonenumber invalid, must have 10 digits (Example: 1234567890).");
        if (!ValidaterLogic.ValidateZipcode(user.Zipcode))
            Errors.Add("Zipcode invalid (Example: 2353TL).");

        if (Errors.Count == 0)
        {
            LoginAccess.Register(user);
            return Errors;
        }
        else
        {
            return Errors;
        }
    }
    public static UserModel? GetUserByEmail(string email) => UserAccess.GetUserByEmail(email);
}