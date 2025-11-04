using Spectre.Console;
using System.Text.RegularExpressions;
using System.Globalization;

public static class ShopDetailsUI
{
    public static void Show()
    {
        Console.Clear();
        ShopInfoModel shopInfo = ShopInfoLogic.GetShopInfo();
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
            DateTime date = DateTime.Parse(day[1]);
            string dayName = date.ToString("dddd", CultureInfo.InvariantCulture);

            if (dayName == "Monday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{shopInfo.OpeningHourMonday} - {shopInfo.ClosingHourMonday}[/]");
            }
            else if (dayName == "Tuesday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{shopInfo.OpeningHourTuesday} - {shopInfo.ClosingHourTuesday}[/]");
            }
            else if (dayName == "Wednesday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{shopInfo.OpeningHourWednesday} - {shopInfo.ClosingHourWednesday}[/]");
            }
            else if (dayName == "Thursday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{shopInfo.OpeningHourThursday} - {shopInfo.ClosingHourThursday}[/]");
            }
            else if (dayName == "Friday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{shopInfo.OpeningHourFriday} - {shopInfo.ClosingHourFriday}[/]");
            }
            else if (dayName == "Saturday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{shopInfo.OpeningHourSaturday} - {shopInfo.ClosingHourSaturday}[/]");
            }
            else
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#125e81]{day[1]}[/]", $"[#5dabcf]{shopInfo.OpeningHourSunday} - {shopInfo.ClosingHourSunday}[/]");
        }
        table.Border(TableBorder.Heavy);

        var description = shopInfo.Description;
        // var body = string.IsNullOrWhiteSpace(description) ? @"
        // Welcome to our supermarket — where freshness comes first.
        // Our bakery opens early with warm, freshly baked bread, and all our vegetables are kept perfectly cooled throughout the day.
        // Most restocking takes place in the evening, so the shelves are full and ready for you every morning." : description;
        var body = description;
        var panel = new Panel(body)
        {
            Border = BoxBorder.Heavy,
            Header = new PanelHeader($"[bold #1B98E0]Info[/]").Centered()
        };
        table.Expand = true;

        panel.Expand = true;
        panel.Header = new PanelHeader("[bold #1B98E0]Info[/]").Centered();

        var rows = new Rows(table, panel);
        AnsiConsole.Write(rows);
        AnsiConsole.WriteLine();
        Console.ReadLine();
    }
    public static void PromptDescription()
    {
        Console.Clear();
        Color AsciiPrimary = Color.FromHex("#247BA0");
        AnsiConsole.Write(
            new FigletText("Update Description")
                .Centered()
                .Color(AsciiPrimary));
        string description = AnsiConsole.Ask<string>("Enter the new [green]shop description[/]:");
        ShopInfoLogic.UpdateDescription(description);
        AnsiConsole.MarkupLine("[green]Shop description updated successfully![/]");
        Console.ReadLine();
    }
    public static void PromptOpeningHours()
    {
        string openingHour, closingHour;
        ShopInfoModel shopInfo = ShopInfoLogic.GetShopInfo();

        Console.Clear();
        Color AsciiPrimary = Color.FromHex("#247BA0");
        AnsiConsole.Write(
            new FigletText("Update Opening Hours")
                .Centered()
                .Color(AsciiPrimary));
        var options = new List<string>();
        options.AddRange(new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" });
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(options));

        if (choice == "Monday")
        {
            while (true)
            {
                openingHour = AnsiConsole.Ask<string>("Enter the new [green]Monday opening hour[/] (HH:MM or HHMM):").Trim();
                closingHour = AnsiConsole.Ask<string>("Enter the new [green]Monday closing hour[/] (HH:MM or HHMM):").Trim();

                if (Regex.IsMatch(openingHour, @"^\d{4}$"))
                    openingHour = openingHour.Insert(2, ":");
                if (Regex.IsMatch(closingHour, @"^\d{4}$"))
                    closingHour = closingHour.Insert(2, ":");

                if (Regex.IsMatch(openingHour, @"^\d{2}:\d{2}$") && Regex.IsMatch(closingHour, @"^\d{2}:\d{2}$"))
                    break;
                AnsiConsole.MarkupLine("[red]Invalid format. Please use HH:MM or HHMM (e.g., 07:00 or 0700).[/]");
            }
            shopInfo.OpeningHourMonday = openingHour; shopInfo.ClosingHourMonday = closingHour;
            ShopInfoLogic.UpdateOpeningHours(shopInfo);
            AnsiConsole.MarkupLine("[green]Shop opening hours updated successfully![/]");
            Console.ReadLine();
        }
        else if (choice == "Tuesday")
        {
            while (true)
            {
                openingHour = AnsiConsole.Ask<string>("Enter the new [green]Tuesday opening hour[/] (HH:MM or HHMM):").Trim();
                closingHour = AnsiConsole.Ask<string>("Enter the new [green]Tuesday closing hour[/] (HH:MM or HHMM):").Trim();

                if (Regex.IsMatch(openingHour, @"^\d{4}$"))
                    openingHour = openingHour.Insert(2, ":");
                if (Regex.IsMatch(closingHour, @"^\d{4}$"))
                    closingHour = closingHour.Insert(2, ":");

                if (Regex.IsMatch(openingHour, @"^\d{2}:\d{2}$") && Regex.IsMatch(closingHour, @"^\d{2}:\d{2}$"))
                    break;

                AnsiConsole.MarkupLine("[red]Invalid format. Please use HH:MM or HHMM (e.g., 07:00 or 0700).[/]");
            }
            shopInfo.OpeningHourTuesday = openingHour; shopInfo.ClosingHourTuesday = closingHour;
            ShopInfoLogic.UpdateOpeningHours(shopInfo);
            AnsiConsole.MarkupLine("[green]Shop Tuesday opening hours updated successfully![/]");
            Console.ReadLine();
        }
        else if (choice == "Wednesday")
        {
            while (true)
            {
                openingHour = AnsiConsole.Ask<string>("Enter the new [green]Wednesday opening hour[/] (HH:MM or HHMM):").Trim();
                closingHour = AnsiConsole.Ask<string>("Enter the new [green]Wednesday closing hour[/] (HH:MM or HHMM):").Trim();

                if (Regex.IsMatch(openingHour, @"^\d{4}$"))
                    openingHour = openingHour.Insert(2, ":");
                if (Regex.IsMatch(closingHour, @"^\d{4}$"))
                    closingHour = closingHour.Insert(2, ":");

                if (Regex.IsMatch(openingHour, @"^\d{2}:\d{2}$") && Regex.IsMatch(closingHour, @"^\d{2}:\d{2}$"))
                    break;

                AnsiConsole.MarkupLine("[red]Invalid format. Please use HH:MM or HHMM (e.g., 07:00 or 0700).[/]");
            }
            shopInfo.OpeningHourWednesday = openingHour; shopInfo.ClosingHourWednesday = closingHour;
            ShopInfoLogic.UpdateOpeningHours(shopInfo);
            AnsiConsole.MarkupLine("[green]Shop Wednesday opening hours updated successfully![/]");
            Console.ReadLine();
        }
        else if (choice == "Thursday")
        {
            while (true)
            {
                openingHour = AnsiConsole.Ask<string>("Enter the new [green]Thursday opening hour[/] (HH:MM or HHMM):").Trim();
                closingHour = AnsiConsole.Ask<string>("Enter the new [green]Thursday closing hour[/] (HH:MM or HHMM):").Trim();

                if (Regex.IsMatch(openingHour, @"^\d{4}$"))
                    openingHour = openingHour.Insert(2, ":");
                if (Regex.IsMatch(closingHour, @"^\d{4}$"))
                    closingHour = closingHour.Insert(2, ":");

                if (Regex.IsMatch(openingHour, @"^\d{2}:\d{2}$") && Regex.IsMatch(closingHour, @"^\d{2}:\d{2}$"))
                    break;

                AnsiConsole.MarkupLine("[red]Invalid format. Please use HH:MM or HHMM (e.g., 07:00 or 0700).[/]");
            }
            shopInfo.OpeningHourThursday = openingHour; shopInfo.ClosingHourThursday = closingHour;
            ShopInfoLogic.UpdateOpeningHours(shopInfo);
            AnsiConsole.MarkupLine("[green]Shop Thursday opening hours updated successfully![/]");
            Console.ReadLine();
        }
        else if (choice == "Friday")
        {
            while (true)
            {
                openingHour = AnsiConsole.Ask<string>("Enter the new [green]Friday opening hour[/] (HH:MM or HHMM):").Trim();
                closingHour = AnsiConsole.Ask<string>("Enter the new [green]Friday closing hour[/] (HH:MM or HHMM):").Trim();

                if (Regex.IsMatch(openingHour, @"^\d{4}$"))
                    openingHour = openingHour.Insert(2, ":");
                if (Regex.IsMatch(closingHour, @"^\d{4}$"))
                    closingHour = closingHour.Insert(2, ":");

                if (Regex.IsMatch(openingHour, @"^\d{2}:\d{2}$") && Regex.IsMatch(closingHour, @"^\d{2}:\d{2}$"))
                    break;

                AnsiConsole.MarkupLine("[red]Invalid format. Please use HH:MM or HHMM (e.g., 07:00 or 0700).[/]");
            }
            shopInfo.OpeningHourFriday = openingHour; shopInfo.ClosingHourFriday = closingHour;
            ShopInfoLogic.UpdateOpeningHours(shopInfo);
            AnsiConsole.MarkupLine("[green]Shop Friday opening hours updated successfully![/]");
            Console.ReadLine();
        }
        else if (choice == "Saturday")
        {
            while (true)
            {
                openingHour = AnsiConsole.Ask<string>("Enter the new [green]Saturday opening hour[/] (HH:MM or HHMM):").Trim();
                closingHour = AnsiConsole.Ask<string>("Enter the new [green]Saturday closing hour[/] (HH:MM or HHMM):").Trim();

                if (Regex.IsMatch(openingHour, @"^\d{4}$"))
                    openingHour = openingHour.Insert(2, ":");
                if (Regex.IsMatch(closingHour, @"^\d{4}$"))
                    closingHour = closingHour.Insert(2, ":");

                if (Regex.IsMatch(openingHour, @"^\d{2}:\d{2}$") && Regex.IsMatch(closingHour, @"^\d{2}:\d{2}$"))
                    break;

                AnsiConsole.MarkupLine("[red]Invalid format. Please use HH:MM or HHMM (e.g., 07:00 or 0700).[/]");
            }
            shopInfo.OpeningHourSaturday = openingHour; shopInfo.ClosingHourSaturday = closingHour;
            ShopInfoLogic.UpdateOpeningHours(shopInfo);
            AnsiConsole.MarkupLine("[green]Shop Saturday opening hours updated successfully![/]");
            Console.ReadLine();
        }
        else
        {
            while (true)
            {
                openingHour = AnsiConsole.Ask<string>("Enter the new [green]Sunday opening hour[/] (HH:MM or HHMM):").Trim();
                closingHour = AnsiConsole.Ask<string>("Enter the new [green]Sunday closing hour[/] (HH:MM or HHMM):").Trim();

                if (Regex.IsMatch(openingHour, @"^\d{4}$"))
                    openingHour = openingHour.Insert(2, ":");
                if (Regex.IsMatch(closingHour, @"^\d{4}$"))
                    closingHour = closingHour.Insert(2, ":");

                if (Regex.IsMatch(openingHour, @"^\d{2}:\d{2}$") && Regex.IsMatch(closingHour, @"^\d{2}:\d{2}$"))
                    break;

                AnsiConsole.MarkupLine("[red]Invalid format. Please use HH:MM or HHMM (e.g., 07:00 or 0700).[/]");
            }
            shopInfo.OpeningHourSunday = openingHour; shopInfo.ClosingHourSunday = closingHour;
            ShopInfoLogic.UpdateOpeningHours(shopInfo);
            AnsiConsole.MarkupLine("[green]Shop Sunday opening hours updated successfully![/]");
            Console.ReadLine();
        }
    }
}