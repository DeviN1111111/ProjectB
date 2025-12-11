using System.ComponentModel.DataAnnotations;
using Spectre.Console;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
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
        Utils.PrintTitle("Supermarket Analytics");

        var period = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Select the [#{Text.ToHex()}]time period[/]")
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { 
                    "Today", 
                    "This Week", 
                    "This Month",
                    "This Year",
                    "All Time", 
                    "Custom Range", 
                    "Search Statistics per product", 
                    "Competitor prices",
                    "Go back" }));

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
                ProductModel product = SearchUI.SearchProductByNameOrCategory();
                if(product == null)
                {
                    break;
                }
                DisplayStatisticsPerProduct(product);
                break;
            case "Competitor prices":
                DisplayCompetitorPrices();
                break;

            default:
                AnsiConsole.MarkupLine("[red]Invalid selection[/]");
                break;
        }

    }

    public static void DisplayStatistics(DateTime startDate, DateTime endDate)
    {
        DateTime start = DateTime.Today.AddMonths(-1);
        DateTime end = DateTime.Today;

        double revenue = StatisticLogic.TotalRevenue(startDate, endDate);
        double cost = StatisticLogic.TotalPurchaseCost(startDate, endDate);

        ShowTotalRevenue(revenue);
        ShowTotalPurchaseCost(cost);
        
        double profit = revenue - cost;
        ShowTotalProfit(profit);


        ProductModel mostSold = StatisticLogic.MostSoldItem(startDate, endDate);
        int count = StatisticLogic.MostSoldItemCount(startDate, endDate);
        List<ProductSalesDto> sales = StatisticLogic.GetProductSalesData(startDate, endDate);
        double totalProfit = StatisticLogic.TotalProfitSince(startDate, endDate);

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
                        Utils.PrintTitle("Supermarket Analytics");
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
        Utils.PrintTitle("Supermarket Analytics");

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
    public static void ShowTotalRevenue(double revenue)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold underline]Total Revenue[/]");
        AnsiConsole.WriteLine();
        double maxRevenue = 10000.0;                 // Max value = full bar
        double percent = Math.Min(revenue / maxRevenue, 1.0); // Convert revenue to percantage

        int barWidth = 40;                          // Total bar length
        int filled = (int)(barWidth * percent);     // Number of filled blocks
        int empty = barWidth - filled;              // Number of empty blocks

        string bar =
            "[green]" + new string('█', filled) + "[/]" +
            "[grey]" + new string('░', empty) + "[/]";

        AnsiConsole.MarkupLine(bar);
        AnsiConsole.WriteLine();

        var legend = "[green]■[/] Revenue";
        AnsiConsole.MarkupLine(legend);
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine($"Total Revenue: [green]€{revenue:F2}[/]");
    }

    public static void ShowTotalPurchaseCost(double cost)
    {
        double maxCost = 10000.0;
        double percent = Math.Min(cost / maxCost, 1); // Convert to percentage

        int width = 40;
        int filled = (int)(width * percent);
        int empty = width - filled;
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[bold underline]Total Purchase Cost[/]");
        AnsiConsole.WriteLine();

        string bar = 
                "[red]" + new string('█', filled) + "[/]" +
                "[grey]" + new string('░', empty) + "[/]";

        AnsiConsole.MarkupLine(bar);
        AnsiConsole.WriteLine();

        var legend = "[red]■[/] Purchase Cost";
        AnsiConsole.MarkupLine(legend);
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine($"Total Purchase Cost: [Red]€{cost:F2}[/]");
    }

    public static void ShowTotalProfit(double profit)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold underline]Total Profit[/]");
        AnsiConsole.WriteLine();
        double maxProfit = 10000.0;                 // Max value = full bar
        // if profit is negetive bar turn red and shows it negative
        if (profit < 0)
        {
            profit = Math.Abs(profit);
            double percentNeg = Math.Min(profit / maxProfit, 1.0); // Convert profit to percantage

            int barWidthNeg = 40;                          // Total bar length
            int filledNeg = (int)(barWidthNeg * percentNeg);     // Number of filled blocks
            int emptyNeg = barWidthNeg - filledNeg;              // Number of empty blocks

            string barNeg =
                "[red]" + new string('█', filledNeg) + "[/]" +
                "[grey]" + new string('░', emptyNeg) + "[/]";

            AnsiConsole.MarkupLine(barNeg);
            AnsiConsole.WriteLine();

            var legendNeg = "[red]■[/] Loss";
            AnsiConsole.MarkupLine(legendNeg);
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine($"Total Loss: [red]€{profit:F2}[/]");
            return;
        }
        double percent = Math.Min(profit / maxProfit, 1.0); // Convert profit to percantage

        int barWidth = 40;                          // Total bar length
        int filled = (int)(barWidth * percent);     // Number of filled blocks
        int empty = barWidth - filled;              // Number of empty blocks

        string bar =
            "[yellow]" + new string('█', filled) + "[/]" +
            "[grey]" + new string('░', empty) + "[/]";

        AnsiConsole.MarkupLine(bar);
        AnsiConsole.WriteLine();

        var legend = "[yellow]■[/] Profit";
        AnsiConsole.MarkupLine(legend);
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine($"Total Profit: [green]€{profit:F2}[/]");
    }
    
    public static void DisplayCompetitorPrices()
    {
        Console.Clear();
        Utils.PrintTitle("Competitor Prices");

        // ** DEPENDENCIES **
        var allProducts = ProductLogic.GetAllProducts();
        var overPricedProducts = ProductLogic.GetOverpricedProducts(allProducts);
        Func<double, string, string> priceFormatter = (price, color) => Utils.ChangePriceFormat(price, color);
        Func<double, string> ownPriceFormatter = price => Utils.ChangePriceFormat(price, "red");

        // ** DISPLAY **
        var competitorPricesTable = Utils.CreateTable(
            new[] { "Product Name", "Our Price", "Competitor Price" }
        );

        foreach (var product in overPricedProducts)
        {
            competitorPricesTable.AddRow(
                product.Name,
                priceFormatter(product.Price, "red"),
                priceFormatter(product.CompetitorPrice, "green")
            );
        }

        AnsiConsole.Write(competitorPricesTable);

        // ** USER PROMPT **
        var selectedChoice = Utils.CreateSelectionPrompt(
            ["Edit prices", "[red]Go back[/]"]
        );

        switch (selectedChoice)
        {
            case "Edit prices":
                EditOverpricedProducts(overPricedProducts, priceFormatter);
                break;
            case "[red]Go back[/]":
                return;
        }
    }
     
    public static void EditOverpricedProducts(List<ProductModel> overPricedProducts, Func<double, string, string> priceFormatter)
    {
        Console.Clear();
        Utils.PrintTitle("Edit Competitor Prices");
        
        var selectedProducts = Utils.CreateMultiSelectionPrompt(
            overPricedProducts,
            "Select products to change price:",
            p => $"{p.Name} - Current Price: {priceFormatter(p.Price, "red")} | Competitor Price: {priceFormatter(p.CompetitorPrice, "green")}"
        );

        foreach (var product in selectedProducts)
        {
            double newPrice = Utils.AskDouble(
                $"Enter new price for [yellow]{product.Name}[/] " +
                $"Current Price: {priceFormatter(product.Price, "red")} | Competitor Price: {priceFormatter(product.CompetitorPrice, "green")}): "
            );

            ProductLogic.LowerPriceForOverpricedProduct(product, newPrice);
        }

        AnsiConsole.MarkupLine("[green]Prices updated successfully![/]");
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to return to the menu.");
        Console.ReadLine();     
    }

}