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
            $"[bold #00014d]Price:[/] [#5dabcf]${product.Price}[/]\n" +
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
    
    public static void CompareTwoProducts(ProductModel product1, ProductModel product2)
    {
        var beforeEditTable = new Table()
            .Title("[yellow]Before EDIT:[/]")
            .AddColumn("Field")
            .AddColumn("Value");


        beforeEditTable.AddRow("Name", product1.Name);
        beforeEditTable.AddRow("Price", product1.Price.ToString("C"));
        beforeEditTable.AddRow("Nutrition", product1.NutritionDetails);
        beforeEditTable.AddRow("Description", product1.Description);
        beforeEditTable.AddRow("Category", product1.Category);
        beforeEditTable.AddRow("Location", product1.Location.ToString());
        beforeEditTable.AddRow("Quantity", product1.Quantity.ToString());

        var afterTable = new Table()
            .Title("[green]After EDIT:[/]")
            .AddColumn("Field")
            .AddColumn("Value");

        afterTable.AddRow("Name", product2.Name);
        afterTable.AddRow("Price", product2.Price.ToString("C"));
        afterTable.AddRow("Nutrition", product2.NutritionDetails);
        afterTable.AddRow("Description", product2.Description);
        afterTable.AddRow("Category", product2.Category);
        afterTable.AddRow("Location", product2.Location.ToString());
        afterTable.AddRow("Quantity", product2.Quantity.ToString());

        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();
        grid.AddRow(beforeEditTable, afterTable);

        var panel = new Panel(grid)
            .Header("[bold cyan]Product Edit Comparison[/]")
            .Border(BoxBorder.Double);
        AnsiConsole.Write(panel);
        }
}