using Spectre.Console;
using System.Text;

public static class ShowProductDetails
{
    private static readonly string OpeningHour = "07:00";
    private static readonly string ClosingHour = "22:00";
    public static void Show()
    {
        Color AsciiPrimary = Color.FromHex("#247BA0");
        AnsiConsole.Write(
            new FigletText("Welcome to our Supermarket!")
                .Centered()
                .Color(AsciiPrimary));

        AnsiConsole.MarkupLine($"[bold #1B98E0]Opening Hours:[/] {OpeningHour} - {ClosingHour}");
        Console.ReadLine();
        Console.Clear();
    }
    private static string Center(string text, int targetWidth)
    {
        var lines = text.Replace("\r", "").Split('\n');
        var blockWidth = lines.Max(l => l.Length);
        var leftPad = Math.Max(0, (targetWidth - blockWidth) / 2);
        var pad = new string(' ', leftPad);

        var sb = new StringBuilder();
        foreach (var line in lines)
        {
            var trimmed = line.TrimEnd();
            sb.AppendLine(string.IsNullOrEmpty(trimmed) ? "" : pad + trimmed);
        }
        return sb.ToString();
    }
}