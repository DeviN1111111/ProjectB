using Spectre.Console;
using System.Text.RegularExpressions;
using System.Globalization;

public static class ShopDetailsUI
{
    public static void Show()
    {
        Console.Clear();
        Utils.PrintTitle("Welcome to our Supermarket!");
        // Table table = Utils.CreateTable(new [] { "[bold #00014d]Day[/]").Centered(), "[bold #00014d]Opening Hours[/]").Centered()});
        var table = new Table();

        List<ShopInfoModel> DescriptionAndWeeks = ShopInfoLogic.GetDescriptionAndAllDays();
        var description = DescriptionAndWeeks[0].Day;

        var body = description;
        var panel = new Panel(body)
        {
            Border = BoxBorder.Heavy,
            Header = new PanelHeader($"[bold #1B98E0]Info[/]").Centered()
        };
        table.Expand = true;

        panel.Expand = true;
        panel.Header = new PanelHeader("[bold #1B98E0]Info[/]").Centered();
        table.AddColumn(new TableColumn("[bold #00014d]Day[/]").Centered());
        table.AddColumn(new TableColumn("[bold #00014d]Opening Hours[/]").Centered());

        foreach (var day in DescriptionAndWeeks)
        {
            if (day.OpeningHour == null && day.ClosingHour == null)
            {
                continue;
            }
            else
            {
                table.AddRow($"[bold #125e81]{day.Day}[/]", $"[#5dabcf]{day.OpeningHour} - {day.ClosingHour}[/]");
            }
        }

        table.Border(TableBorder.Heavy);
        AnsiConsole.WriteLine();
        AnsiConsole.Write(panel); 
        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        // review
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold yellow]─────────────────────────────[/]");
        AnsiConsole.MarkupLine("[bold yellow]★ Customer Reviews ★[/]");
        AnsiConsole.MarkupLine("[bold yellow]─────────────────────────────[/]");
        AnsiConsole.WriteLine();
        var reviewLogic = new ShopReviewLogic();
        var allReviews = reviewLogic.GetAllReviews();

        if (allReviews.Any())
        {
            double avg = reviewLogic.GetAverageStars();
            AnsiConsole.MarkupLine($"[bold yellow]Average Rating:[/] [green]{avg:F1}★[/]");
            AnsiConsole.WriteLine();

            foreach (var review in allReviews.Take(5))
            {
                string stars = new string('★', review.Stars);
                AnsiConsole.MarkupLine($"[green]{stars}[/] [white]{review.Text}[/]");
                AnsiConsole.MarkupLine($"[grey]Posted on {review.CreatedAt}[/]");
                AnsiConsole.WriteLine();
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[grey]No reviews yet! Be the first to leave one![/]");
        }

        var currentUser = SessionManager.CurrentUser;
        if (currentUser!= null && currentUser.AccountStatus == "User")
        {
            var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Would you like to add a review?[/]")
                .AddChoices("Yes", "No")
            );
            switch(choice)
            {
                case "Yes":
                    ShopReviewUI.AddReview(reviewLogic);
                    Show();
                    return;
                case "No":
                    break;
            }
        }   
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue.");
        Console.ReadKey();
    }
    public static void ChangeDescription()
    {
        Console.Clear();
        Utils.PrintTitle("Update Description");
        string description = AnsiConsole.Ask<string>("Enter the new [green]shop description[/]:");
        ShopInfoLogic.UpdateDescription(description);
        AnsiConsole.MarkupLine("[green]Shop description updated successfully![/]");
        Console.ReadLine();
        return;
    }

    public static void ChangeOpeningHours()
    {
        Console.Clear();
        Utils.PrintTitle("Update Opening Hours");

        var options = new List<string>();

        options.AddRange(new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" });
        // var day = AnsiConsole.Prompt(new SelectionPrompt<string>()
        //         .AddChoices(options));

        var days = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("[bold white]Select the days you want to change for the opening/closing hours.[/]")
                .NotRequired()
                .PageSize(20)
                .AddChoiceGroup("Select all days", options)
        );

        if (days.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No days selected.[/]");
            AnsiConsole.MarkupLine("Press [green]ANY KEY[/] to continue.");
            Console.ReadKey();
            return;
        }

        string openingHour;
        string closingHour;
        while (true)
        {
            openingHour = AnsiConsole.Ask<string>($"Enter the new [green]opening hour[/] (HH:MM or HHMM):").Trim();
            closingHour = AnsiConsole.Ask<string>($"Enter the new [green]closing hour[/] (HH:MM or HHMM):").Trim();

            if (Regex.IsMatch(openingHour, @"^\d{4}$"))
                openingHour = openingHour.Insert(2, ":");
            if (Regex.IsMatch(closingHour, @"^\d{4}$"))
                closingHour = closingHour.Insert(2, ":");

            if (Regex.IsMatch(openingHour, @"^\d{2}:\d{2}$") && Regex.IsMatch(closingHour, @"^\d{2}:\d{2}$"))
                break;
            AnsiConsole.MarkupLine("[red]Invalid format. Please use HH:MM or HHMM (e.g., 07:00 or 0700).[/]");
        }

        foreach (var day in days)
        {
            ShopInfoLogic.UpdateOpeningHours(openingHour, closingHour, day);
        }
        AnsiConsole.MarkupLine("[green]Shop opening hours updated successfully![/]");
        AnsiConsole.MarkupLine("Press [green]ANY KEY[/] to continue.");
        Console.ReadKey();
    }
}