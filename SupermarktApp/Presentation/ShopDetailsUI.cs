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

        var description = shopInfo.Description;

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

        foreach (var day in ShopInfoLogic.getDayDate())
        {
            DateTime date = DateTime.Parse(day[1]);
            string dayName = date.ToString("dddd", CultureInfo.InvariantCulture);

            if (dayName == "Monday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#5dabcf]{shopInfo.OpeningHourMonday} - {shopInfo.ClosingHourMonday}[/]");
            }
            else if (dayName == "Tuesday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#5dabcf]{shopInfo.OpeningHourTuesday} - {shopInfo.ClosingHourTuesday}[/]");
            }
            else if (dayName == "Wednesday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#5dabcf]{shopInfo.OpeningHourWednesday} - {shopInfo.ClosingHourWednesday}[/]");
            }
            else if (dayName == "Thursday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#5dabcf]{shopInfo.OpeningHourThursday} - {shopInfo.ClosingHourThursday}[/]");
            }
            else if (dayName == "Friday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#5dabcf]{shopInfo.OpeningHourFriday} - {shopInfo.ClosingHourFriday}[/]");
            }
            else if (dayName == "Saturday")
            {
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#5dabcf]{shopInfo.OpeningHourSaturday} - {shopInfo.ClosingHourSaturday}[/]");
            }
            else
                table.AddRow($"[bold #125e81]{day[0]}[/]", $"[#5dabcf]{shopInfo.OpeningHourSunday} - {shopInfo.ClosingHourSunday}[/]");
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
                    AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue.");
                    Console.ReadKey();
                    break;
            }
        }   
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue.");
        Console.ReadKey();
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

        while (true)
        {
            openingHour = AnsiConsole.Ask<string>($"Enter the new [green]{choice} opening hour[/] (HH:MM or HHMM):").Trim();
            closingHour = AnsiConsole.Ask<string>($"Enter the new [green]{choice} closing hour[/] (HH:MM or HHMM):").Trim();

            if (Regex.IsMatch(openingHour, @"^\d{4}$"))
                openingHour = openingHour.Insert(2, ":");
            if (Regex.IsMatch(closingHour, @"^\d{4}$"))
                closingHour = closingHour.Insert(2, ":");

            if (Regex.IsMatch(openingHour, @"^\d{2}:\d{2}$") && Regex.IsMatch(closingHour, @"^\d{2}:\d{2}$"))
                break;
            AnsiConsole.MarkupLine("[red]Invalid format. Please use HH:MM or HHMM (e.g., 07:00 or 0700).[/]");
        }
        switch(choice)
        {
            case "Monday":
                shopInfo.OpeningHourMonday = openingHour; shopInfo.ClosingHourMonday = closingHour;
                break;
            case "Tuesday":
                shopInfo.OpeningHourTuesday = openingHour; shopInfo.ClosingHourTuesday = closingHour;
                break;
            case "Wednesday":
                shopInfo.OpeningHourWednesday = openingHour; shopInfo.ClosingHourWednesday = closingHour;
                break;
            case "Thursday":
                shopInfo.OpeningHourThursday = openingHour; shopInfo.ClosingHourThursday = closingHour;
                break;
            case "Friday":
                shopInfo.OpeningHourFriday = openingHour; shopInfo.ClosingHourFriday = closingHour;
                break;
            case "Saturday":
                shopInfo.OpeningHourSaturday = openingHour; shopInfo.ClosingHourSaturday = closingHour;
                break;
            case "Sunday":
                shopInfo.OpeningHourSunday = openingHour; shopInfo.ClosingHourSunday = closingHour;
                break;

        }
        ShopInfoLogic.UpdateOpeningHours(shopInfo);
        AnsiConsole.MarkupLine("[green]Shop opening hours updated successfully![/]");
        AnsiConsole.MarkupLine("Press [green]ANY KEY[/] to continue.");
        Console.ReadKey();
    }
}