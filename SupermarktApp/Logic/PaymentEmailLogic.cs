public static class PaymentEmailLogic
{
    private static readonly string PaymentTemplatePath = "EmailTemplates/PaymentCodeTemplate.html";
    private static readonly string PaymentTemplate = File.ReadAllText(PaymentTemplatePath);

    public static async Task SendEmailAsync(string email, int paymentCode)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException("Recipient email is null or empty.");

            var template = PaymentTemplate
                ?? throw new InvalidOperationException("PaymentTemplate is null (not loaded).");

            string emailBody = template.Replace("{{PAYMENT_CODE}}", paymentCode.ToString("D6"));

            var ok = await EmailLogic.SendEmailAsync(
                to: email,
                subject: "Your Payment Code",
                body: emailBody,
                isHtml: true
            );

            if (!ok)
                Console.WriteLine("EmailLogic.SendEmailAsync returned false.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SendEmailAsync] failed: {ex}");
            throw; // rethrow so callers can see it (or drop if you want to swallow)
        }
    }
}