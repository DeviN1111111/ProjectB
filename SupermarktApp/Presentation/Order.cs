using Spectre.Console;
public class Order
{
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");

    public static void ShowCart()
    {
        Console.Clear();
        double totalAmount = 0;
        List<CartModel> allUserProducts = OrderLogic.AllUserProducts();  // List of user Products in cart
        List<ProductModel> allProducts = ProductAccess.GetAllProducts();  // List of all products dit moet via logic

        // Title
        AnsiConsole.Write(
            new FigletText("Cart")
                .Centered()
                .Color(AsciiPrimary));


        // Cart table
        var cartTable = new Table()
            .BorderColor(AsciiPrimary)
            .AddColumn("[white]Product[/]")
            .AddColumn("[white]Quantity[/]")
            .AddColumn("[white]Price[/]")
            .AddColumn("[white]Total[/]");


        // Products in cart
        foreach (var cartProduct in allUserProducts)
        {
            // Get Product id and find match in all products
            foreach (ProductModel Product in allProducts)
            {
                if (cartProduct.ProductId == Product.ID)
                {
                    cartTable.AddRow(Product.Name, cartProduct.Quantity.ToString(), $"${Product.Price}", $"${Product.Price * cartProduct.Quantity}");
                    totalAmount = totalAmount + ( Product.Price * cartProduct.Quantity);
                }
            }
        }

        AnsiConsole.Write(cartTable);
        AnsiConsole.WriteLine();

        // Calculate delivery fee
        double deliveryFee = OrderLogic.DeliveryFee(totalAmount);

        // Summary box
        var panel = new Panel(

            new Markup($"[bold white]Total:[/] [white]${totalAmount}[/]\n[bold white]Delivery Fee:[/] [white]${deliveryFee}[/]\n[bold white]Total:[/] [white]${totalAmount + deliveryFee}[/]"))
            .Header("[bold white]Summary[/]", Justify.Left)
            .Border(BoxBorder.Rounded)
            .BorderColor(AsciiPrimary)
            .Expand();

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();

        Checkout();
    }
    public static void Checkout()
    {
        // Checkout or go back options
        var options = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .AddChoices(new[]{
                    "Checkout",
                    "Go back"
        })
);

        switch (options)
        {
            case "Checkout":
                Console.Clear();
                // pay now or pay on pickup
                AnsiConsole.Write(
                    new FigletText("Checkout")
                        .Centered()
                        .Color(Color.White));
                AnsiConsole.WriteLine("Choose payment method:");
                var option1 = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .AddChoices(new[]{
                            "Pay now",
                            "Pay on pickup",
                }));

                switch (option1)
                {
                    case "Pay now":
                        AnsiConsole.WriteLine("Thank you purchase succesful!");
                        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                        Console.ReadKey();
                        OrderLogic.UpdateStock();
                        OrderLogic.ClearCart();
                        break;
                    case "Pay on pickup":
                        AnsiConsole.WriteLine("Thank you purchase succesful!");
                        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                        Console.ReadKey();
                        OrderLogic.UpdateStock();
                        OrderLogic.ClearCart();
                        break;
                }
                break;
            case "Go back":
                return;
        }
    }
}