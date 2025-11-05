using System.Threading.Tasks;

public static class TwoFALogic
{
    public static string Generate2FACode()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    public static async Task CreateInsertAndEmailSend2FACode(int userId, int validityMinutes = 10)
    {
        string code = Generate2FACode();

        await EmailLogic.SendEmailAsync(
            to: UserAccess.GetUserEmail(userId),
            subject: "Your 2FA Code",
            body: $"Your 2FA code is: {code} it is valid for {validityMinutes} minutes."
        );

        DateTime expiry = DateTime.Now.AddMinutes(validityMinutes);

        UserAccess.Insert2FACode(userId, code, expiry);
    }

    public static async Task<string> Register2FAEmail(string email)
    {
        string code = Generate2FACode();
        
        await EmailLogic.SendEmailAsync(
            to: email,
            subject: "Your 2FA Registration Code",
            body: $"Your 2FA registration code is: {code}"
        );

        return code;
    }
    public static bool Validate2FACode(int userId, string inputCode)
    {
        string correctCode = UserAccess.Get2FACode(userId);
        DateTime? expiry = UserAccess.Get2FAExpiry(userId);

        if (expiry == null || DateTime.Now > expiry)
        {
            return false;
        }

        return inputCode == correctCode;
    }

    public static bool Is2FAEnabled(int userId)
    {
        return UserAccess.Has2FAEnabled(userId);
    }

    public static void Enable2FA(int userId)
    {
        UserAccess.Set2FAStatus(userId, true);
    }
    public static void Disable2FA(int userId)
    {
        UserAccess.Set2FAStatus(userId, false);
    }
}