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
                    totalAmount = totalAmount + (Product.Price * cartProduct.Quantity);
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

        Checkout(allUserProducts, allProducts);
    }
    public static void Checkout(List<CartModel> cartProducts, List<ProductModel> allProducts)
    {
        // Checkout or go back options
        var options = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .AddChoices(new[]{

            "Checkout",
            "Remove items",
            "Go back"
        })
);

        switch (options)
        {
            case "Checkout":
                Console.Clear();
                // check if cart is empty
                if (cartProducts.Count == 0)
                {
                    AnsiConsole.MarkupLine("[red]Your cart is empty![/]");
                    Thread.Sleep(1500);
                    return;
                }
                // pay now or pay on pickup
                AnsiConsole.Write(
                    new FigletText("Checkout")
                        .Centered()
                        .Color(Color.White));
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
                        OrderLogic.UpdateStock();
                        OrderLogic.ClearCart();
                        Thread.Sleep(3000);
                        break;
                    case "Pay on pickup":
                        AnsiConsole.WriteLine("Thank you purchase succesful!");
                        OrderLogic.UpdateStock();
                        OrderLogic.ClearCart();
                        Thread.Sleep(3000);
                        break;
                }
                break;

            case "Remove items":
                RemoveFromCart(cartProducts, allProducts);
                break;

            case "Go back":
                break;

        }
        return;
    }

    public static void RemoveFromCart(List<CartModel> cartProducts, List<ProductModel> allProducts)
    {
        // Build list of item names
        var cartChoices = new List<string>();
        foreach (var item in cartProducts)
        {
            var product = allProducts.FirstOrDefault(cartProduct => cartProduct.ID == item.ProductId);
            if (product != null)
                cartChoices.Add($"{product.Name} (x{item.Quantity})");
        }

        if (cartChoices.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Your cart is empty![/]");
            Thread.Sleep(1500);
            return;
        }

        var itemsToRemove = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("[bold white]Select items to remove from your cart:[/]")
                .NotRequired()
                .PageSize(10)
                .MoreChoicesText("[grey](Use ↑/↓ to navigate, [blue]<space>[/] to select, [green]<enter>[/] to confirm)[/]")
                .AddChoices(cartChoices)
        );

        if (itemsToRemove.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No items selected.[/]");
            Thread.Sleep(1000);
            return;
        }

        // Actually remove items
        foreach (var choice in itemsToRemove)
        {
            var productName = choice.Split(" (x")[0];
            var product = allProducts.FirstOrDefault(Product => Product.Name == productName);
            if (product != null)
                RemoveItemFromCart(product.ID);
        }

        AnsiConsole.MarkupLine("[green]Selected items have been removed![/]");
        Thread.Sleep(1500);

        // Refresh cart view
        ShowCart();
    }
    private static void RemoveItemFromCart(int productId)
    {
        OrderLogic.RemoveFromCart(productId);
    }
}