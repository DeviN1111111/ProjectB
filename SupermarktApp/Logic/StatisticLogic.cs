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

    public static BreakdownChart CreateBreakdownChart(List<ProductSalesDto> sales, string Type)
    {
        var Totals = new Dictionary<string, double>();

        foreach (var sale in sales)
        {
            if (!Totals.ContainsKey(sale.Product.Category))
            {
                Totals[sale.Product.Category] = 0;
            }

            if (Type == "Category")
            {
                Totals[sale.Product.Category] += sale.SoldCount;
            }
            else if (Type == "Profit")
            {
                Totals[sale.Product.Category] += sale.SoldCount * (double)sale.Product.Price;
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

        foreach (var category in Totals)
        {
            var color = colors[colorIndex];
            double roundedValue = Math.Round(category.Value, 2);
            chart.AddItem(category.Key, roundedValue, color);
            colorIndex++;
            if (colorIndex >= colors.Count)
            {
                colorIndex = 0;
            }
        }
        Totals.Clear();
        return chart;
    }
    public static void CreateBreakdownChartForSingleProduct(ProductModel Product)
    {
        ProductSalesDto saleDTO = OrderAccess.GetSalesOfSingleProductByID(Product.ID);
        if (saleDTO == null)
        {
            AnsiConsole.MarkupLine("No sales data available for this product.");
            return;
        }
        double totalProfit = saleDTO.SoldCount * (double)Product.Price;
        var table = new Table();
        table.AddColumn("Product Name");
        table.AddColumn("Units Sold");
        table.AddColumn("Price per Unit");
        table.AddColumn("Total Revenue from this product");

        table.AddRow(
            saleDTO.Product.Name,
            saleDTO.SoldCount.ToString(),
            $"{saleDTO.Product.Price} euro",
            $"{totalProfit} euro"
        );
        AnsiConsole.Write(table);
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

    public static Table CreateMostSoldTable(DateTime date)
    {
        var table = new Table();
        table.AddColumn("Product Name");
        table.AddColumn("Units Sold");
        table.AddColumn("Category");
        table.AddColumn("Price per Unit");
        table.AddColumn("Total Revenue from this product");

        var topSales = OrderAccess.GetTop5MostSoldProductsUpToDate(date);
        if (topSales == null || topSales.Count == 0)
        {
            table.AddRow("No sales data available", "-", "-", "-", "-");
            return table;
        }

        foreach (var sale in topSales)
        {
            var product = ProductAccess.GetProductByID(sale.ProductID);
            if (product != null)
            {
                double totalRevenue = (double)(product.Price * (double)sale.SoldCount);
                table.AddRow(product.Name, sale.SoldCount.ToString(), product.Category, $"{product.Price} euro", $"{totalRevenue} euro");
            }
        }

        return table;
    }

}