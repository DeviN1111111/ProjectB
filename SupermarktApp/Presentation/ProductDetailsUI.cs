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

        
        // string name = Markup.Escape(product.Name ?? "");
        string nutrition = Markup.Escape(product.NutritionDetails ?? "");
        string description = Markup.Escape(product.Description ?? "");
        string category = Markup.Escape(product.Category ?? "");
        string location = Markup.Escape(product.Location.ToString());
        string price = product.Price.ToString("0.00");
        string qty = product.Quantity.ToString();

        var body =
            $"[bold #00014d]Name:[/] [#5dabcf]{product.Name}[/]\n" +
            $"[bold #00014d]Price:[/] [#5dabcf]${price}[/]\n" +
            $"[bold #00014d]Nutrition Info:[/] [#5dabcf]{nutrition}[/]\n" +
            $"[bold #00014d]Description:[/] [#5dabcf]{description}[/]\n" +
            $"[bold #00014d]Category:[/] [#5dabcf]{category}[/]\n" +
            $"[bold #00014d]Location:[/] [#5dabcf]{location}[/]\n" +
            $"[bold #00014d]Stock Quantity:[/] [#5dabcf]{qty}[/]";

        var panel = new Panel(body)
        {
            Padding = new Padding(1, 1),
            Border = BoxBorder.Heavy,
            Header = new PanelHeader($"[bold #1B98E0]{product.Name}[/]")
        };

        AnsiConsole.Write(panel);
    }
}