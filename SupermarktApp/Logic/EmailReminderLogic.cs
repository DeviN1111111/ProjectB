using System.Text;

public static class EmailReminderLogic
{
    public static async Task SendCartReminderAsync(string userEmail, List<CartModel> items)
    {
        string subject = "You left items in your cart!";
        string body = BuildEmailBody(items);

        await EmailLogic.SendEmailAsync(
            to: userEmail,
            subject: subject,
            body: body,
            isHtml: false
        );
    }

    private static string BuildEmailBody(List<CartModel> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Hey there!");
        sb.AppendLine();
        sb.AppendLine("You left the following items in your supermarket cart:");
        sb.AppendLine();

        foreach (var item in items)
        {
            sb.AppendLine($"• ProductID: {item.ProductId}, Quantity: {item.Quantity}");
        }

        sb.AppendLine();
        sb.AppendLine("Come back anytime to complete your purchase!");
        return sb.ToString();
    }
}
