using Spectre.Console;
public static class ShopDetails
{
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
        string? OpeningHour = ShopInfoAccess.GetOpeningHours().Item1;
        string? ClosingHour = ShopInfoAccess.GetOpeningHours().Item2;
        if (OpeningHour == null || ClosingHour == null)
        {
            OpeningHour = "07:00";
            ClosingHour = "22:00";
        }
        string? OpeningHourSunday = ShopInfoAccess.GetOpeningHoursSunday().Item1;
        string? ClosingHourSunday = ShopInfoAccess.GetOpeningHoursSunday().Item2;
        if (OpeningHourSunday == null || ClosingHourSunday == null)
        {
            OpeningHourSunday = "12:00";
            ClosingHourSunday = "19:00";
        }
        
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

        var description = ShopInfoAccess.GetDescription();

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

        var options = new List<string>();
        if (SessionManager.CurrentUser == null)
        {
            options.AddRange(new [] {"Go Back"} );
        }
        else if (SessionManager.CurrentUser.AccountStatus == "User")
        {
            options.AddRange(new[] { "Go Back" });
        }
        else if (SessionManager.CurrentUser.AccountStatus == "Admin")
        {
            options.AddRange(new[] { "Go Back", "Edit Shop Description", "Edit Opening Hours" });
        }
        else if (SessionManager.CurrentUser.AccountStatus == "Guest")
        {
            options.AddRange(new[] { "Go Back" });
        }
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(options));
        switch (choice)
        {
            case "Go Back":
                break;
            case "Edit Shop Description":
                UpdateShopInfo.UpdateDescription();
                break;
            case "Edit Opening Hours":
                UpdateShopInfo.UpdateOpeningHours();
                break;
        }
    }
}