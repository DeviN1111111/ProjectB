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

        foreach (var day in ShopInfoLogic.getDayDate())
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
            double avg = allReviews.Average(r => r.Stars);
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
        if (currentUser != null)
        {
            var addChoice = AnsiConsole.Confirm("[green]Would you like to add a review?[/]");
            if (addChoice)
            {
                Console.Clear();
                AnsiConsole.Write(
                    new FigletText("Add Your Review")
                        .Centered()
                        .Color(Color.Yellow));

                int stars = AnsiConsole.Prompt(
                    new SelectionPrompt<int>()
                        .Title("[yellow]How many stars would you rate the shop?[/]")
                        .AddChoices(1, 2, 3, 4, 5));

                string text = AnsiConsole.Ask<string>("Write your [green]review text[/]:");

                try
                {
                    var logic = new ShopReviewLogic();
                    logic.AddReview(currentUser.ID, stars, text);
                    AnsiConsole.MarkupLine("[green]✅ Thank you! Your review has been submitted.[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
                }

                AnsiConsole.MarkupLine("[grey]Press any key to return to shop details...[/]");
                Console.ReadKey();
                Show();
                return;
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[grey]Log in to leave your own review![/]");
        }
        // enter to continue
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