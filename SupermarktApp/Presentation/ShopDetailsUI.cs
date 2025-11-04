using Spectre.Console;
public static class ShopDetails
{
    public static void Show()
    {
        Console.Clear();
        Color AsciiPrimary = Color.FromHex("#247BA0");
        AnsiConsole.Write(
            new FigletText("Welcome to our Supermarket!")
                .Centered()
                .Color(AsciiPrimary));


        var table = new Table();
        table.AddColumn(new TableColumn("[bold #00014d]Day[/]").Centered());
        table.AddColumn(new TableColumn("[bold #00014d]Date[/]").Centered());
        table.AddColumn(new TableColumn("[bold #00014d]Opening Hours[/]").Centered());

        var OpeningHourMonday = UpdateShopInfo.PassOpeningHours("Monday").Item1 != null ? UpdateShopInfo.PassOpeningHours("Monday").Item1 : "07:00";
        var ClosingHourMonday = UpdateShopInfo.PassOpeningHours("Monday").Item2 != null ? UpdateShopInfo.PassOpeningHours("Monday").Item2 : "22:00";
        var OpeningHourTuesday = UpdateShopInfo.PassOpeningHours("Tuesday").Item1 != null ? UpdateShopInfo.PassOpeningHours("Tuesday").Item1 : "07:00";
        var ClosingHourTuesday = UpdateShopInfo.PassOpeningHours("Tuesday").Item2 != null ? UpdateShopInfo.PassOpeningHours("Tuesday").Item2 : "22:00";
        var OpeningHourWednesday = UpdateShopInfo.PassOpeningHours("Wednesday").Item1 != null ? UpdateShopInfo.PassOpeningHours("Wednesday").Item1 : "07:00";
        var ClosingHourWednesday = UpdateShopInfo.PassOpeningHours("Wednesday").Item2 != null ? UpdateShopInfo.PassOpeningHours("Wednesday").Item2 : "22:00";
        var OpeningHourThursday = UpdateShopInfo.PassOpeningHours("Thursday").Item1 != null ? UpdateShopInfo.PassOpeningHours("Thursday").Item1 : "07:00";
        var ClosingHourThursday = UpdateShopInfo.PassOpeningHours("Thursday").Item2 != null ? UpdateShopInfo.PassOpeningHours("Thursday").Item2 : "22:00";
        var OpeningHourFriday = UpdateShopInfo.PassOpeningHours("Friday").Item1 != null ? UpdateShopInfo.PassOpeningHours("Friday").Item1 : "07:00";
        var ClosingHourFriday = UpdateShopInfo.PassOpeningHours("Friday").Item2 != null ? UpdateShopInfo.PassOpeningHours("Friday").Item2 : "22:00";
        var OpeningHourSaturday = UpdateShopInfo.PassOpeningHours("Saturday").Item1 != null ? UpdateShopInfo.PassOpeningHours("Saturday").Item1 : "08:00";
        var ClosingHourSaturday = UpdateShopInfo.PassOpeningHours("Saturday").Item2 != null ? UpdateShopInfo.PassOpeningHours("Saturday").Item2 : "20:00";
        var OpeningHourSunday = UpdateShopInfo.PassOpeningHours("Sunday").Item1 != null ? UpdateShopInfo.PassOpeningHours("Sunday").Item1 : "08:00";
        var ClosingHourSunday = UpdateShopInfo.PassOpeningHours("Sunday").Item2 != null ? UpdateShopInfo.PassOpeningHours("Sunday").Item2 : "20:00";

        foreach (var day in GetDayDate.getDayDate())
        {
            if (day[0] == "Monday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{OpeningHourMonday} - {ClosingHourMonday}[/]");
            }
            else if (day[0] == "Tuesday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{OpeningHourTuesday} - {ClosingHourTuesday}[/]");
            }
            else if (day[0] == "Wednesday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{OpeningHourWednesday} - {ClosingHourWednesday}[/]");
            }
            else if (day[0] == "Thursday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{OpeningHourThursday} - {ClosingHourThursday}[/]");
            }
            else if (day[0] == "Friday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{OpeningHourFriday} - {ClosingHourFriday}[/]");
            }
            else if (day[0] == "Saturday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{OpeningHourSaturday} - {ClosingHourSaturday}[/]");
            }
            else
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{OpeningHourSunday} - {ClosingHourSunday}[/]");
        }
        table.Border(TableBorder.Heavy);

        var description = UpdateShopInfo.PassDescription();

        var body = string.IsNullOrWhiteSpace(description) ? @"
        Welcome to our supermarket — where freshness comes first.
        Our bakery opens early with warm, freshly baked bread, and all our vegetables are kept perfectly cooled throughout the day.
        Most restocking takes place in the evening, so the shelves are full and ready for you every morning." : description;

        var panel = new Panel(body)
        {
            Border = BoxBorder.Heavy,
            Header = new PanelHeader($"[bold #1B98E0]Info[/]").Centered()
        };
        var columns = new Columns(table, panel);
        AnsiConsole.Write(columns);
        AnsiConsole.WriteLine();
        Console.ReadLine();
    }
}