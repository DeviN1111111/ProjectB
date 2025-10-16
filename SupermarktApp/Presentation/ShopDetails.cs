using Spectre.Console;
public static class ShopDetails
{
    private static readonly string OpeningHour = "07:00";
    private static readonly string ClosingHour = "22:00";
    private static readonly string OpeningHourSunday = "12:00";
    private static readonly string ClosingHourSunday = "19:00";
    public static void Show()
    {
        Color AsciiPrimary = Color.FromHex("#247BA0");
        AnsiConsole.Write(
            new FigletText("Welcome to our Supermarket!")
                .Centered()
                .Color(AsciiPrimary));


        var table = new Table();
        table.AddColumn(new TableColumn("[bold #00014d]Day[/]").Centered());
        table.AddColumn(new TableColumn("[bold #00014d]Date[/]").Centered());
        table.AddColumn(new TableColumn("[bold #00014d]Opening Hours[/]").Centered());

        foreach (var day in GetDayDate.getDayDate())
        {
            if (day[0] != "Sunday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{OpeningHour} - {ClosingHour}[/]");
            }
            else
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{OpeningHourSunday} - {ClosingHourSunday}[/]");
        }
        table.Border(TableBorder.Heavy);

        var body = @"
        Welcome to our supermarket — where freshness comes first.
        Our bakery opens early with warm, freshly baked bread, and all our vegetables are kept perfectly cooled throughout the day.
        Most restocking takes place in the evening, so the shelves are full and ready for you every morning.";
        var panel = new Panel(body)
        {
            Border = BoxBorder.Heavy,
            Header = new PanelHeader($"[bold #1B98E0]Info[/]").Centered()
        };

        var columns = new Columns(table, panel);
        AnsiConsole.Write(columns);

        Console.ReadLine();
    }
}