using System.ComponentModel.DataAnnotations;
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
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("SuperMart Analytics")
                .Centered()
                .Color(AsciiPrimary));


        var period = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Select the [#{Text.ToHex()}]time period[/]")
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { "Today", "This Week", "This Month", "This Year", "Custom Range", "All Time", "Go back" }));

        switch (period)
        {
            case "Go back":
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
                DisplayStatistics(PromptForDate());
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
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
            if (date == DateTime.MinValue)
            {
                AnsiConsole.WriteLine($"Your total turnover of all time is {totalProfit} euro!");
            }
            else{
                AnsiConsole.WriteLine($"Your total turnover since {date.ToShortDateString()} is {totalProfit} euro!");
            }
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("Amount of sales per category:");
            AnsiConsole.WriteLine();
            AnsiConsole.Write(StatisticLogic.CreateBreakdownChart(sales, "Category"));
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("Total turnover per category in €:");
            AnsiConsole.WriteLine();
            AnsiConsole.Write(StatisticLogic.CreateBreakdownChart(sales, "Profit"));

        }

        if (mostSold != null && count > 0)
        {
            var Table = StatisticLogic.CreateMostSoldTable(date);
            if (Table == null)
            {
                AnsiConsole.MarkupLine("No data available for the selected period.");
            }
            else
            {
                AnsiConsole.WriteLine();
                AnsiConsole.WriteLine();
                if (date == DateTime.MinValue)
                {
                    AnsiConsole.Write($"Top 5 most sold items of all time were: ");
                }
                else
                {
                    AnsiConsole.Write($"Top 5 most sold items since {date.ToShortDateString()} were: ");
                }
                AnsiConsole.WriteLine();
                AnsiConsole.Write(Table);   
            }

        }

        AnsiConsole.MarkupLine("Press [green]ENTER[/] to return to the menu.");
        Console.ReadLine();
        DisplayMenu();
    }
    

    public static DateTime PromptForDate()
    {
        while (true)
        {
            var input = AnsiConsole.Ask<string>($"Enter the [#{Text.ToHex()}] date from when you want the analytics![/] (YYYY-MM-DD):");
            if (DateTime.TryParse(input, out DateTime date))
            {
                Console.Clear();
                AnsiConsole.Write(
                new FigletText("SuperMart Analytics")
                    .Centered()
                    .Color(AsciiPrimary));
                return date;
            }
            AnsiConsole.MarkupLine("[red]Invalid date format. Please try again.[/]");
        }
    }
}