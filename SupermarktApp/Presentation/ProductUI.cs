using Spectre.Console;

public class ProductUI
{
    public static List<ProductModel> SearchCategory()
    {
        AnsiConsole.MarkupLine("[blue]Type the category you want to find[/]");
        List<ProductModel> productList = ProductLogic.SearchProductByCategory();
        return productList;
    }

    public static ProductModel SearchItem()
    {
        AnsiConsole.MarkupLine("[blue]Type the category you want to find[/]");
        ProductModel Product = ProductLogic.SearchProductByName();
        return Product;
    }
}