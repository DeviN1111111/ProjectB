using Spectre.Console;

public class DiscountedProductsUI
{
    public static readonly Color Text = Color.FromHex("#E8F1F2");
    public static readonly Color Hover = Color.FromHex("#006494");
    public static readonly Color Confirm = Color.FromHex("#13293D");
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static readonly Color AsciiSecondary = Color.FromHex("#1B98E0");
    public static void DisplayMenu()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("Discounts")
                    .Centered()
                    .Color(AsciiPrimary));

            var Choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .HighlightStyle(new Style(Hover))
                    .AddChoices(new[] { "Weekly Discounts", "Go back" }));

            switch (Choice)
            {
                case "Go back":
                    return;

                case "Weekly Discounts":
                    break;

                case "Personal Discounts}":
                    break;

                default:
                    AnsiConsole.MarkupLine("[red]Invalid selection[/]");
                    break;
            }
        }
    }
}