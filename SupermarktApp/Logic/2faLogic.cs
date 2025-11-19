using System.Threading.Tasks;

public static class TwoFALogic
{
    private static string Register2FATemplatePath = "EmailTemplates/Register2FATemplate.html";
    private static string Login2FATemplatePath = "EmailTemplates/Login2FATemplate.html";
    private static string ForgetPassword2FATemplatePath = "EmailTemplates/ForgetPassword2FATemplate.html";
    private static string Login2FATemplate = File.ReadAllText(Login2FATemplatePath);
    private static string Register2FATemplate = File.ReadAllText(Register2FATemplatePath);
    private static string ForgetPassword2FATemplate = File.ReadAllText(ForgetPassword2FATemplatePath);
    public static string Generate2FACode()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    public static async Task CreateInsertAndEmailSend2FACode(int userId, int validityMinutes = 10!)
    {
        string code = Generate2FACode();
        Login2FATemplate = Login2FATemplate.Replace("{{CODE}}", code);
        Login2FATemplate = Login2FATemplate.Replace("{{VALIDITY}}", validityMinutes.ToString());

        await EmailLogic.SendEmailAsync(
            to: UserAccess.GetUserEmail(userId)!,
            subject: "Your 2FA Code",
            body: Login2FATemplate,
            isHtml: true
        );

        DateTime expiry = DateTime.Now.AddMinutes(validityMinutes);

        UserAccess.Insert2FACode(userId, code, expiry);
    }

    public static async Task<string> Register2FAEmail(string email)
    {
        string code = Generate2FACode();
        Register2FATemplate = Register2FATemplate.Replace("{{CODE}}", code);

        await EmailLogic.SendEmailAsync(
            to: email,
            subject: "Your 2FA Registration Code",
            body: Register2FATemplate,
            isHtml: true
        );

        return code;
    }
    public static async Task<string> ForgetPassword2FAEmail(int userId, string email)
    {
        string code = Generate2FACode();
        ForgetPassword2FATemplate = ForgetPassword2FATemplate.Replace("{{CODE}}", code);

        await EmailLogic.SendEmailAsync(
            to: email,
            subject: "Your 2FA Code for Password Reset",
            body: ForgetPassword2FATemplate,
            isHtml: true
        );
        UserAccess.Insert2FACode(userId, code, DateTime.Now.AddMinutes(10));

        return code;
    }
    public static bool Validate2FACode(int userId, string inputCode)
    {
        string correctCode = UserAccess.Get2FACode(userId)!;
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