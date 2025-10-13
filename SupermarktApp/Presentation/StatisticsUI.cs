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
                .AddChoices(new[] { "Today", "This Week", "This Month", "This Year", "All Time", "Custom Range", "Search Statistics per product", "Go back" }));

        switch (period)
        {
            case "Go back":
                return;

            case "Today":
                DisplayStatistics(DateTime.Today, DateTime.Now);
                break;

            case "This Week":
                DisplayStatistics(DateTime.Today.AddDays(-7), DateTime.Now);
                break;

            case "This Month":
                DisplayStatistics(DateTime.Today.AddMonths(-1), DateTime.Now);
                break;

            case "This Year":
                DisplayStatistics(DateTime.Today.AddYears(-1), DateTime.Now);
                break;

            case "Custom Range":
                DateTime FirstOrderDate = StatisticLogic.GetDateOfFirstOrder();
                var (startDate, endDate) = PromptForDate(FirstOrderDate);
                DisplayStatistics(startDate, endDate);
                break;

            case "All Time":
                DisplayStatistics(DateTime.MinValue, DateTime.Now);
                break;
            case "Search Statistics per product":
                DisplayStatisticsPerProduct(ProductLogic.SearchProductByNameOrCategory());
                break;

            default:
                AnsiConsole.MarkupLine("[red]Invalid selection[/]");
                break;
        }

    }

    public static void DisplayStatistics(DateTime startDate, DateTime endDate)
    {
        ProductModel mostSold = StatisticLogic.MostSoldItem(startDate, endDate);
        int count = StatisticLogic.MostSoldItemCount(startDate, endDate);
        List<ProductSalesDto> sales = StatisticLogic.GetProductSalesData(startDate, endDate);
        int totalProfit = StatisticLogic.TotalProfitSince(startDate, endDate);

        if (sales != null && sales.Count > 0 && totalProfit > 0)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
            if (startDate == DateTime.MinValue)
            {
                AnsiConsole.WriteLine($"Your total turnover of all time is {totalProfit} euro!");
            }
            else
            {
                AnsiConsole.WriteLine($"Your total turnover from {startDate.ToShortDateString()} till {endDate.ToShortDateString()} is {totalProfit} euro!");
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

        if (mostSold != null)
        {
            var Table = StatisticLogic.CreateMostSoldTable(startDate, endDate);
            if (Table == null)
            {
                AnsiConsole.MarkupLine("No data available for the selected period.");
            }
            else
            {
                AnsiConsole.WriteLine();
                AnsiConsole.WriteLine();
                if (startDate == DateTime.MinValue)
                {
                    AnsiConsole.Write($"Top 5 most sold items of all time were: ");
                }
                else
                {
                    AnsiConsole.Write($"Top 5 most sold items from {startDate.ToShortDateString()} to {endDate.ToShortDateString()} were: ");
                }
                AnsiConsole.WriteLine();
                AnsiConsole.Write(Table);
            }

        }
        else
        {
            AnsiConsole.MarkupLine("No data available for the selected period.");
        }
        
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to return to the menu.");
        Console.ReadLine();
        DisplayMenu();
    }

    public static (DateTime, DateTime) PromptForDate(DateTime firstOrderDate)
    {
        while (true)
        {
            var startDate = AnsiConsole.Prompt(new TextPrompt<string>($"Enter the start date for the analytics!:").DefaultValue(firstOrderDate.ToShortDateString()));
            var endDate = AnsiConsole.Prompt(new TextPrompt<string>($"Enter the end date for the analytics!:").DefaultValue(DateTime.Now.ToShortDateString()));

            if (DateTime.TryParse(startDate, out DateTime startDate1) && DateTime.TryParse(endDate, out DateTime endDate1))
            {
                if (startDate1 >= firstOrderDate && endDate1 <= DateTime.Now)
                {
                    if (startDate1 <= endDate1)
                    {
                        Console.Clear();
                        AnsiConsole.Write(
                        new FigletText("SuperMart Analytics")
                            .Centered()
                            .Color(AsciiPrimary));
                        return (startDate1, endDate1);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Start date must be earlier than or equal to end date. Please try again.[/]");
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Invalid date, you cant go back in the time before the first order[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Invalid format please try again[/]");
            }
    
        }
    }

    public static void DisplayStatisticsPerProduct(ProductModel product)
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("SuperMart Analytics")
                .Centered()
                .Color(AsciiPrimary));

        var table = StatisticLogic.CreateBreakdownChartForSingleProduct(product);
        if (table == null)
        {
            AnsiConsole.MarkupLine("No data available for the selected product.");
            Console.ReadLine();
            return;
        }


        AnsiConsole.Write(table);
        Console.ReadLine();
    }
}