using System.Text;

public static class EmailReminderLogic
{
    private static readonly string TemplatePath = 
        Path.Combine("EmailTemplates", "CartProductReminderTemplate.html");

    public static async Task SendCartProductReminderAsync(string userEmail, List<CartProductModel> items)
    {
        string subject = "You left items in your CartProduct!";

        string template = File.ReadAllText(TemplatePath);
        string body = BuildEmailBody(items, template);

        await EmailLogic.SendEmailAsync(
            to: userEmail,
            subject: subject,
            body: body,
            isHtml: true
        );
    }

    private static string BuildEmailBody(List<CartProductModel> items, string template)
    {
        var itemListBuilder = new StringBuilder();

        foreach (var item in items)
        {
            var product = ProductAccess.GetProductByID(item.ProductId);
            string name = product?.Name ?? "Unknown Product";

            itemListBuilder.AppendLine($"<li>{name} — Quantity: {item.Quantity}</li>");
        }

        return template.Replace("{{ITEM_LIST}}", itemListBuilder.ToString());
    }
}
