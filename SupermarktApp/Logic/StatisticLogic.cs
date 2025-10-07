using Spectre.Console;


public static class StatisticLogic
{
    public static ProductModel MostSoldItem(DateTime date)
    {
        OrdersModel MostSold = OrderAccess.GetMostSoldProductAfterDate(date);

        if (MostSold == null)
        {
            return null;
        }

        ProductModel MostSoldProduct = ProductAccess.GetProductByID(MostSold.ProductID);

        if (MostSoldProduct == null)
        {
            return null;
        }

        return MostSoldProduct;
    }

    public static int MostSoldItemCount(DateTime date)
    {
        OrdersModel MostSold = OrderAccess.GetMostSoldProductAfterDate(date);

        if (MostSold == null)
        {
            return 0;
        }

        int count = OrderAccess.GetMostSoldCountUpToDate(date);
        return count;
    }

    public static List<ProductSalesDto> GetProductSalesData(DateTime date)
    {
        List<ProductSalesDto> sales = OrderAccess.SeedProductSalesDto(date);
        if (sales == null)
        {
            return null;
        }
        return sales;
    }

    public static BreakdownChart CreateBreakdownChart(List<ProductSalesDto> sales)
    {
        var categoryTotals = new Dictionary<string, int>();

        foreach (var sale in sales)
        {
            if (categoryTotals.ContainsKey(sale.Product.Category))
            {
                categoryTotals[sale.Product.Category] += sale.SoldCount;
            }
            else
            {
                categoryTotals[sale.Product.Category] = sale.SoldCount;
            }
        }

        var chart = new BreakdownChart()
            .Width(60);

        var colors = new List<Color>
        {
            Color.Red, Color.Green, Color.Blue, Color.Yellow,
            Color.Orange1, Color.Purple, Color.Aqua, Color.LightGreen
        };

        int colorIndex = 0;

        foreach (var category in categoryTotals)
        {
            var color = colors[colorIndex];
            chart.AddItem(category.Key, category.Value, color);
            colorIndex++;
            if (colorIndex >= colors.Count)
            {
                colorIndex = 0;
            }
        }
        return chart;
    }

    public static int TotalProfitSince(DateTime date)
    {
        List<ProductSalesDto> sales = GetProductSalesData(date);
        if (sales == null)
        {
            return 0;
        }

        int totalProfit = 0;
        foreach (var sale in sales)
        {
            totalProfit += (int)(sale.Product.Price * sale.SoldCount);
        }
        return totalProfit;
    }

}