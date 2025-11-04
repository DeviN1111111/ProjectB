using Spectre.Console;
using System.Text.RegularExpressions;
public class UpdateShopInfo
{
    public static void UpdateDescription()
    {
        string description = AnsiConsole.Ask<string>("Enter the new [green]shop description[/]:");
        ShopInfoAccess.UpdateDescription(description);
        AnsiConsole.MarkupLine("[green]Shop description updated successfully![/]");
        Console.ReadLine();
    }
    public static string? PassDescription()
    {
        return ShopInfoAccess.GetDescription();
    }
    public static void UpdateOpeningHours()
    {
        string openingHour, closingHour;

        var options = new List<string>();
        options.AddRange(new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" });
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(options));

        if (choice == "Monday")
        {
            while (true)
            {
                openingHour = AnsiConsole.Ask<string>("Enter the new [green]opening hour[/] (HH:MM or HHMM):").Trim();
                closingHour = AnsiConsole.Ask<string>("Enter the new [green]closing hour[/] (HH:MM or HHMM):").Trim();

                if (Regex.IsMatch(openingHour, @"^\d{4}$"))
                    openingHour = openingHour.Insert(2, ":");
                if (Regex.IsMatch(closingHour, @"^\d{4}$"))
                    closingHour = closingHour.Insert(2, ":");

                if (Regex.IsMatch(openingHour, @"^\d{2}:\d{2}$") && Regex.IsMatch(closingHour, @"^\d{2}:\d{2}$"))
                    break;

                AnsiConsole.MarkupLine("[red]Invalid format. Please use HH:MM or HHMM (e.g., 07:00 or 0700).[/]");
            }

            ShopInfoAccess.UpdateOpeningHoursMonday(openingHour, closingHour);
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

            ShopInfoAccess.UpdateOpeningHoursTuesday(openingHour, closingHour);
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

            ShopInfoAccess.UpdateOpeningHoursWednesday(openingHour, closingHour);
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

            ShopInfoAccess.UpdateOpeningHoursThursday(openingHour, closingHour);
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

            ShopInfoAccess.UpdateOpeningHoursFriday(openingHour, closingHour);
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

            ShopInfoAccess.UpdateOpeningHoursSaturday(openingHour, closingHour);
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

            ShopInfoAccess.UpdateOpeningHoursSunday(openingHour, closingHour);
            AnsiConsole.MarkupLine("[green]Shop Sunday opening hours updated successfully![/]");
            Console.ReadLine();
        }
    }
    public static (string?, string?) PassOpeningHours(string day)
    {
        (string?, string?) monday = ShopInfoAccess.GetOpeningHoursMonday();
        (string?, string?) tuesday = ShopInfoAccess.GetOpeningHoursTuesday();
        (string?, string?) wednesday = ShopInfoAccess.GetOpeningHoursWednesday();
        (string?, string?) thursday = ShopInfoAccess.GetOpeningHoursThursday();
        (string?, string?) friday = ShopInfoAccess.GetOpeningHoursFriday();
        (string?, string?) saturday = ShopInfoAccess.GetOpeningHoursSaturday();
        (string?, string?) sunday = ShopInfoAccess.GetOpeningHoursSunday();
        return day switch
        {
            "Monday" => monday,
            "Tuesday" => tuesday,
            "Wednesday" => wednesday,
            "Thursday" => thursday,
            "Friday" => friday,
            "Saturday" => saturday,
            "Sunday" => sunday,
            _ => ("07:00", "22:00"),
        };
    }
}