using Spectre.Console;
public static class StatisticsUI
{
    public static readonly Color Text = Color.FromHex("#E8F1F2");
    public static readonly Color Hover = Color.FromHex("#006494");
    public static readonly Color Confirm = Color.FromHex("#13293D");
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static readonly Color AsciiSecondary = Color.FromHex("#1B98E0");

    public static void DisplayMenu()
    {
        AnsiConsole.Write(
            new FigletText("SuperMart Analytics")
                .Centered()
                .Color(AsciiPrimary));


        var period = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Select the [#{Text.ToHex()}]time period[/]")
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { "Today", "This Week", "This Month", "This Year", "Custom Range", "All Time", "Exit" }));

        switch (period)
        {
            case "Exit":
                return;

            case "Today":
                DisplayStatistics(DateTime.Today);
                break;

            case "This Week":
                DisplayStatistics(DateTime.Today.AddDays(-7));
                break;

            case "This Month":
                DisplayStatistics(DateTime.Today.AddMonths(-1));
                break;

            case "This Year":
                DisplayStatistics(DateTime.Today.AddYears(-1));
                break;

            case "Custom Range":
                // doeklaterwel
                break;

            case "All Time":
                DisplayStatistics(DateTime.MinValue);
                break;

            default:
                AnsiConsole.MarkupLine("[red]Invalid selection[/]");
                break;
        }

    }

    public static void DisplayStatistics(DateTime date)
    {
        ProductModel mostSold = StatisticLogic.MostSoldItem(date);
        int count = StatisticLogic.MostSoldItemCount(date);
        List<ProductSalesDto> sales = StatisticLogic.GetProductSalesData(date);
        int totalProfit = StatisticLogic.TotalProfitSince(date);

        if (sales != null)
        {
            AnsiConsole.Write(StatisticLogic.CreateBreakdownChart(sales));
            AnsiConsole.WriteLine($"Total profit since {date.ToShortDateString()} is {totalProfit} euro!");
        }

        if (mostSold != null && count > 0)
        {
            AnsiConsole.Write($"Most sold item: {mostSold.Name} Sold {count} times.");
        }

        
    }
}