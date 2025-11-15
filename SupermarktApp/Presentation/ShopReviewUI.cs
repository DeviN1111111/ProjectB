using Spectre.Console;

public static class ShopReviewUI
{
    public static void ShowMenu()
    {
        var logic = new ShopReviewLogic();

        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("Shop Reviews")
                    .Centered()
                    .Color(Color.Gold1));

            var options = new List<string>
            {
                "View My Reviews",
                "Add a Review",
                "View Average Rating",
                "Go Back"
            };

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Select an option[/]")
                    .AddChoices(options));

            switch (choice)
            {
                case "View My Reviews":
                    ViewMyReviews(logic);
                    break;

                case "Add a Review":
                    AddReview(logic);
                    break;

                case "View Average Rating":
                    ViewAverage(logic);
                    break;

                case "Go Back":
                    return;
            }
        }
    }

    private static void ViewMyReviews(ShopReviewLogic logic)
    {
        Console.Clear();

        var user = SessionManager.CurrentUser;
        if (user == null)
        {
            AnsiConsole.MarkupLine("[red]You must be logged in to view reviews![/]");
            AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
            Console.ReadKey();
            return;
        }

        var reviews = logic.GetReviews(user.ID);

        if (!reviews.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No reviews yet![/]");
        }
        else
        {
            foreach (var review in reviews)
            {
                AnsiConsole.MarkupLine($"[green]{review.Stars}★[/] - {review.Text} [grey]({review.CreatedAt})[/]");
            }
        }

        AnsiConsole.MarkupLine("\n[grey]Press any key to return...[/]");
        Console.ReadKey();
    }

    private static void AddReview(ShopReviewLogic logic)
    {
        Console.Clear();
        var user = SessionManager.CurrentUser;
        if (user == null)
        {
            AnsiConsole.MarkupLine("[red]You must be logged in to add a review![/]");
            Console.ReadKey();
            return;
        }

        int stars = AnsiConsole.Ask<int>("Enter star rating (1-5):");
        string text = AnsiConsole.Ask<string>("Enter your review:");

        try
        {
            logic.AddReview(user.ID, stars, text);
            AnsiConsole.MarkupLine("[green]Review added successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
        }

        Console.ReadKey();
    }

    private static void ViewAverage(ShopReviewLogic logic)
    {
        Console.Clear();
        var user = SessionManager.CurrentUser;
        if (user == null)
        {
            AnsiConsole.MarkupLine("[red]You must be logged in to view averages![/]");
            Console.ReadKey();
            return;
        }

        var avg = logic.GetAverageStars(user.ID);
        AnsiConsole.MarkupLine($"[yellow]Your average rating:[/] [green]{avg:F1}★[/]");
        Console.ReadKey();
    }
}
