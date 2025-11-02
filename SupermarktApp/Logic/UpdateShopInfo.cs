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
        options.AddRange(new[] { "Weekdays (Mon-Sat)", "Sunday" });
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(options));

        if (choice != "Sunday")
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

            ShopInfoAccess.UpdateOpeningHours(openingHour, closingHour);
            AnsiConsole.MarkupLine("[green]Shop opening hours updated successfully![/]");
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
    public static (string?, string?) PassOpeningHours()
    {
        return ShopInfoAccess.GetOpeningHours();
    }
    public static (string?, string?) PassOpeningHoursSunday()
    {
        return ShopInfoAccess.GetOpeningHoursSunday();
    }
}