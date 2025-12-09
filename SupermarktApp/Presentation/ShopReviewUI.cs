using Spectre.Console;

public static class ShopReviewUI
{
    public static void ShowMenu()
    {

        var logic = new ShopReviewLogic();

        while (true)
        {
            Console.Clear();
            Utils.PrintTitle("Shop Reviews");

            var options = new List<string>
            {
                "View My Reviews",
                "Add a Review",
                "View All Reviews",
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

                case "View All Reviews":
                    ShopDetailsUI.Show();
                    break;

                case "Go Back":
                    return;
            }
        }
    }

    private static void ViewMyReviews(ShopReviewLogic logic)
    {
        Console.Clear();
        Utils.PrintTitle("Your Reviews");

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

    public static void AddReview(ShopReviewLogic logic)
    {
        Console.Clear();
        Utils.PrintTitle("Add Your Review");

        var currentUser = SessionManager.CurrentUser;
        if (currentUser == null)
        {
            AnsiConsole.MarkupLine("[red]You must be logged in to add a review![/]");
            AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
            Console.ReadKey();
            return;
        }
        // give stars
        int stars = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                    .Title("[yellow]How many stars would you rate the shop?[/]")
                    .AddChoices(1, 2, 3, 4, 5));

        string text = AnsiConsole.Ask<string>("Write your [green]review text[/]:");
        try
            {
                logic.AddReview(currentUser.ID, stars, text);
                AnsiConsole.MarkupLine("[green]Thank you! Your review has been submitted.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
            }
            AnsiConsole.MarkupLine("[grey]Press any key to return to shop details...[/]");
            Console.ReadKey();
        
            return; 

    }
}
