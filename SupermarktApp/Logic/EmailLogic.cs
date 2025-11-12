using System.Net;
using System.Net.Mail;


public static class EmailLogic
{
    private static readonly string smtpServer = "smtp.gmail.com";
    private static readonly int smtpPort = 587;
    private static readonly string RealPassword = "SupermarketAppPassword123!"; // Dit is het echte wachtwoord voor het GMAIL account
    private static readonly string Password = "hkbi rrku niuz ctxj"; // Dit is een app-wachtwoord voor het GMAIL account

    private static readonly string FromEmail = "SupermarketAppEmailSender@gmail.com";

    public static async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        EmailModel email = new EmailModel
        {
            From = FromEmail,
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = isHtml
        };

        try
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(email.From);
                mail.To.Add(email.To);
                mail.Subject = email.Subject;
                mail.Body = email.Body;
                mail.IsBodyHtml = email.IsHtml;

                using (SmtpClient smtp = new SmtpClient(smtpServer, smtpPort))
                {
                    smtp.Credentials = new NetworkCredential(FromEmail, Password);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}

