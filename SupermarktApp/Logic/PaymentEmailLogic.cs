public static class PaymentEmailLogic
{
    private static readonly string PaymentTemplatePath = "EmailTemplates/PaymentCodeTemplate.html";
    private static readonly string PaymentTemplate = File.ReadAllText(PaymentTemplatePath);

    public static async Task SendEmailAsync(string email, int paymentCode)
    {
        string emailBody = PaymentTemplate.Replace("{{PAYMENT_CODE}}", paymentCode.ToString());

        await EmailLogic.SendEmailAsync(
            to: email,
            subject: "Your Payment Code",
            body: emailBody,
            isHtml: true
        );
    }
}