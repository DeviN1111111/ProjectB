using System.Security.Cryptography.X509Certificates;
using Spectre.Console;

using System.Threading;
public class Order
{
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    private static string Safe(string text) => Markup.Escape(text);
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

        // Calculate total discount
        double discount = OrderLogic.CalculateTotalDiscount();

        // Summary box
        var panel = new Panel(
            new Markup($"[bold white]Total price:[/] [white]${Math.Round(totalAmount + deliveryFee - discount, 2)}[/]\n[bold white]Discount:[/] [white]${discount}[/]\n[bold white]Delivery Fee:[/] [white]${Math.Round(deliveryFee, 2)}[/]"))
            .Header("[bold white]Summary[/]", Justify.Left)
            .Border(BoxBorder.Rounded)
            .BorderColor(AsciiPrimary)
            .Expand();

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
        double finalAmount = totalAmount + deliveryFee - discount;

        Checkout(allUserProducts, allProducts, finalAmount);
    }

    public static void ShowChecklist()
    {
        while (true)
        {
            Console.Clear();
            List<ChecklistModel> allUserProducts = ChecklistLogic.AllUserProducts();
            List<ProductModel> allProducts = ProductAccess.GetAllProducts();

            AnsiConsole.Write(
                new FigletText("Checklist")
                    .Centered()
                    .Color(AsciiPrimary));

            if (allUserProducts.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Your checklist is empty![/]");
                var emptyAction = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold white]What would you like to do?[/]")
                        .AddChoices("Add new item", "Go back")
                );

                if (emptyAction == "Add new item")
                {
                    ProductUI.SearchProduct("checklist");
                    continue;
                }
                else
                {
                    return;
                }
            }

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold white]What would you like to do?[/]")
                    .AddChoices("Add new item", "Mark or remove items", "Clear checklist", "Go back")
            );

            switch (action)
            {
                case "Add new item":
                    ProductUI.SearchProduct("checklist");
                    break;

                case "Mark or remove items":
                    HandleChecklistActions(allUserProducts, allProducts);
                    break;

                case "Clear checklist":
                    ChecklistLogic.ClearChecklist();
                    AnsiConsole.MarkupLine("[green]Checklist cleared![/]");
                    System.Threading.Thread.Sleep(1000); 
                    Console.ReadKey();
                    break;

                case "Go back":
                    return;
            }
        }
    }


    private static void HandleChecklistActions(List<ChecklistModel> allUserProducts, List<ProductModel> allProducts)
    {
        while (true)
        {
            allUserProducts = ChecklistLogic.AllUserProducts();
            allProducts = ProductAccess.GetAllProducts();

            var checklistChoices = new List<string>();

            int index = 1;
            foreach (var checklistProduct in allUserProducts)
            {
                var product = allProducts.FirstOrDefault(p => p.ID == checklistProduct.ProductId);
                if (product != null)
                {
                    checklistChoices.Add($"{index}. {product.Name} (x{checklistProduct.Quantity})");
                    index++;
                }
            }

            var checkedItems = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("[bold white]Select items to mark or remove:[/]")
                    .NotRequired()
                    .PageSize(10)
                    .MoreChoicesText("[grey](Use ↑/↓ to navigate, [blue]<space>[/] to toggle, [green]<enter>[/] to confirm, [blue]<A>[/] to add new item)[/]")
                    .InstructionsText("[grey](Press [blue]<space>[/] to toggle, [blue]<A>[/] to add, [green]<enter>[/] to confirm)[/]")
                    .AddChoices(checklistChoices)
            );

            if (checkedItems.Contains("<A> Add new product"))
            {
                ProductUI.SearchProduct("checklist");
                continue;
            }

            if (checkedItems.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No items selected.[/]");
                Console.ReadKey();
                return;
            }

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold white]What would you like to do with the selected items?[/]")
                    .AddChoices("Mark as done", "Remove from checklist", "Cancel")
            );

            switch (action)
            {
                case "Mark as done":
                    foreach (var choice in checkedItems)
                    {
                        if (choice.StartsWith("<A>")) continue;
                        var productName = choice.Split(". ")[1].Split(" (x")[0];
                        var product = allProducts.FirstOrDefault(p => p.Name == productName);

                        if (product != null)
                        {
                            AnsiConsole.MarkupLine($"[green]✔ {product.Name} marked as done![/]");
                            ChecklistLogic.RemoveFromChecklist(product.ID);
                        }
                    }
                    break;

                case "Remove from checklist":
                    foreach (var choice in checkedItems)
                    {
                        if (choice.StartsWith("<A>")) continue;
                        var productName = choice.Split(". ")[1].Split(" (x")[0];
                        var product = allProducts.FirstOrDefault(p => p.Name == productName);

                        if (product != null)
                            ChecklistLogic.RemoveFromChecklist(product.ID);
                    }
                    AnsiConsole.MarkupLine("[red]Selected items removed from checklist.[/]");
                    break;

                case "Cancel":
                    AnsiConsole.MarkupLine("[grey]No changes made.[/]");
                    break;
            }

            AnsiConsole.MarkupLine("\nPress [green]ENTER[/] to continue...");
            Console.ReadKey();
            continue;
        }
    }

    public static void Checkout(List<CartModel> cartProducts, List<ProductModel> allProducts, double totalAmount)
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
                // check if cart is empty
                if (cartProducts.Count == 0)
                {
                    AnsiConsole.MarkupLine("[red]Your cart is empty![/]");
                    AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                    Console.ReadKey();
                    return;
                }
                // Add reward points to user
                int rewardPoints = RewardLogic.CalculateRewardPoints(totalAmount);
                RewardLogic.AddRewardPointsToUser(rewardPoints);
                // pay now or pay on pickup
                Console.Clear();
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
                        AnsiConsole.MarkupLine("Thank you purchase succesful!");
                        if(rewardPoints > 0)
                        {
                            AnsiConsole.MarkupLine($"You have earned [green]{rewardPoints}[/] reward points!");
                        }
                        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                        Console.ReadKey();
                        OrderLogic.UpdateStock();
                        OrderLogic.ClearCart();
                        break;
                    case "Pay on pickup":
                        AnsiConsole.MarkupLine("Thank you purchase succesful!");
                        if(rewardPoints > 0)
                        {
                            AnsiConsole.MarkupLine($"You have earned [green]{rewardPoints}[/] reward points!");
                        }
                        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
                        Console.ReadKey();
                        OrderLogic.UpdateStock();
                        OrderLogic.ClearCart();
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
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
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
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
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
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
        Console.ReadKey();

        // Refresh cart view
        ShowCart();
    }
    private static void RemoveItemFromCart(int productId)
    {
        OrderLogic.RemoveFromCart(productId);
    }
}