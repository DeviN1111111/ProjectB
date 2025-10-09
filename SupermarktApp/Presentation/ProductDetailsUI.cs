using System.Text;
using Spectre.Console;

public static class ProductDetailsUI
{
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static void ShowProductDetails(ProductModel product)
    {
        Console.Clear();

        AnsiConsole.Write(
            new FigletText("Product Details")
                .Centered()
                .Color(AsciiPrimary));

        var body =
            $"[bold #00014d]Name:[/] [#5dabcf]{product.Name}[/]\n" +
            $"[bold #00014d]Price:[/] [#5dabcf]${product.Price:0.00}[/]\n" +
            $"[bold #00014d]Nutrition Info:[/] [#5dabcf]{product.NutritionDetails}[/]\n" +
            $"[bold #00014d]Description:[/] [#5dabcf]{product.Description}[/]\n" +
            $"[bold #00014d]Category:[/] [#5dabcf]{product.Category}[/]\n" +
            $"[bold #00014d]Location:[/] [#5dabcf]{product.Location}[/]\n" +
            $"[bold #00014d]Stock Quantity:[/] [#5dabcf]{product.Quantity}[/]";

        var panel = new Panel(body)
        {
            Padding = new Padding(1, 1),
            Border = BoxBorder.Heavy,
            Header = new PanelHeader($"[bold #1B98E0]{product.Name}[/]")
        };

        AnsiConsole.Write(panel);
    }
}