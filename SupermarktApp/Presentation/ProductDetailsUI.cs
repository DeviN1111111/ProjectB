using System.Text;
using Spectre.Console;

public static class ProductDetailsUI
{
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static void ShowProductDetails(ProductModel product)
    {
        Console.Clear();
        double ProductPrice = product.Price;
        AnsiConsole.Write(
            new FigletText("Product Details")
                .Centered()
                .Color(AsciiPrimary));
        var body = string.Empty;

        ProductDiscountDTO productDiscount = DiscountsLogic.CheckDiscountByProduct(product);
        string expiryText = product.ExpiryDate.Date.ToString("dd-MM-yyyy");

        if(productDiscount != null) // if u got a discount print the discounted price
        {
            string discountType = productDiscount.Discount!.DiscountType;
            double DiscountPercentage = productDiscount.Discount.DiscountPercentage;

            ProductPrice = Math.Round(product.Price * (1 - DiscountPercentage / 100), 2);
            body =
                $"[bold #00014d]Name:[/] [#5dabcf]{product.Name}[/]\n" +
                $"[bold #00014d]Price:[/] [#5dabcf][red strike]€{product.Price}[/] €{ProductPrice} [italic yellow]({discountType} Discount)[/][/]\n" +
                $"[bold #00014d]Expiry Date:[/] [#5dabcf]{expiryText}[/]\n" +
                $"[bold #00014d]Nutrition Info:[/] [#5dabcf]{product.NutritionDetails}[/]\n" +
                $"[bold #00014d]Description:[/] [#5dabcf]{product.Description}[/]\n" +
                $"[bold #00014d]Category:[/] [#5dabcf]{product.Category}[/]\n" +
                $"[bold #00014d]Stock Quantity:[/] [#5dabcf]{product.Quantity}[/]";
        }
        else if (SessionManager.CurrentUser.AccountStatus == "User" || SessionManager.CurrentUser.AccountStatus == "Guest")
        {
            body =
                $"[bold #00014d]Name:[/] [#5dabcf]{product.Name}[/]\n" +
                $"[bold #00014d]Price:[/] [#5dabcf]€{ProductPrice}[/]\n" +
                $"[bold #00014d]Expiry Date:[/] [#5dabcf]{expiryText}[/]\n" +
                $"[bold #00014d]Nutrition Info:[/] [#5dabcf]{product.NutritionDetails}[/]\n" +
                $"[bold #00014d]Description:[/] [#5dabcf]{product.Description}[/]\n" +
                $"[bold #00014d]Category:[/] [#5dabcf]{product.Category}[/]\n" +
                $"[bold #00014d]Stock Quantity:[/] [#5dabcf]{product.Quantity}[/]";
        }
        else if(SessionManager.CurrentUser.AccountStatus == "Admin" || SessionManager.CurrentUser.AccountStatus == "SuperAdmin")
            body =
                $"[bold #00014d]Name:[/] [#5dabcf]{product.Name}[/]\n" +
                $"[bold #00014d]Price:[/] [#5dabcf]€{ProductPrice}[/]\n" +
                $"[bold #00014d]Expiry Date:[/] [#5dabcf]{expiryText}[/]\n" +
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
        beforeEditTable.AddRow("Visible", product1.Visible.ToString());

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
        afterTable.AddRow("Visible", product2.Visible.ToString());

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