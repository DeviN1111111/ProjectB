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

    public static bool Register(string name, string lastName, string email, string password, string address, string zipcode, string phoneNumber, string city)
    {
        UserModel user = new UserModel(name, lastName, email, password, address, zipcode, phoneNumber, city);

        bool CheckLogin = ValidaterLogic.ValidateEmail(user.Email);
        bool CheckPassword = ValidaterLogic.ValidatePassword(user.Password);
        bool CheckPhoneNumber = ValidaterLogic.ValidatePhoneNumber(user.PhoneNumber);

        if (CheckLogin && CheckPassword && CheckPhoneNumber)
        {
            LoginAccess.Register(user);
            return true;
        }
        else
        {
            return false;
        }
    }
}