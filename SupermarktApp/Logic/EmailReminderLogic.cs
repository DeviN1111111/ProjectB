using System.Text;

public static class EmailReminderLogic
{
    private static readonly string TemplatePath = 
        Path.Combine("EmailTemplates", "CartReminderTemplate.html");

    public static async Task SendCartReminderAsync(string userEmail, List<CartModel> items)
    {
        string subject = "You left items in your cart!";

        string template = File.ReadAllText(TemplatePath);
        string body = BuildEmailBody(items, template);

        await EmailLogic.SendEmailAsync(
            to: userEmail,
            subject: subject,
            body: body,
            isHtml: true
        );
    }

    private static string BuildEmailBody(List<CartModel> items, string template)
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
