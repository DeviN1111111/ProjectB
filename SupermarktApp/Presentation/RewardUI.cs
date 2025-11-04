using Spectre.Console;
public static class RewardUI
{
    public static void DisplayMenu()
    {
        while (true)
        {

            Console.Clear();
            AnsiConsole.Write(
                new FigletText("Reward System")
                    .Centered()
                    .Color(MenuUI.AsciiPrimary));

            AnsiConsole.MarkupLine($"Total Points: [green]{SessionManager.CurrentUser.AccountPoints}[/]");

            List<RewardProductDTO> AllRewardProducts = RewardLogic.GetAllRewardItems();

            AnsiConsole.MarkupLine("Available Reward Items:");

            var table = new Table();
            table.AddColumn("Product");
            table.AddColumn("Points");

            foreach (var item in AllRewardProducts)
            {
                table.AddRow(
                    $"[yellow]{item.Product.Name}[/]",
                    $"[green]{item.PriceInPoints}[/]"
                    );
            }

            AnsiConsole.Write(table);

            var options = new List<string> { "Use reward points", "Go back" }; ;

            var prompt = new SelectionPrompt<string>()
                .Title("Select an item to add to cart (this will be free in checkout)")
                .PageSize(10)
                .AddChoices(options);

            var selectedItem = AnsiConsole.Prompt(prompt);

            switch (selectedItem)
            {
                case "Use reward points":
                    SelectRewardMenu(AllRewardProducts);
                    break;
                case "Go back":
                    return;
            }
        }
    }

    public static void SelectRewardMenu(List<RewardProductDTO>? AllRewardProducts)
    {
        var options = new List<string>();

        foreach (var rp in AllRewardProducts)
        {
            options.Add(rp.Product.Name);
        }
        options.Add("Go back");
        var prompt = new SelectionPrompt<string>()
            .Title("Select an item to add to cart (this will be free in checkout)")
            .PageSize(10)
            .AddChoices(options);

        var selectedItem = AnsiConsole.Prompt(prompt);

        if (selectedItem == "Go back")
        {
            return;
        }

        ProductModel? selectedProduct = ProductLogic.GetProductByName(selectedItem);
        RewardProductDTO? selectedReward = RewardLogic.GetRewardItemByProductId(selectedProduct.ID);
        AddItemToCart(selectedReward);

    }
    
    public static void AddItemToCart(RewardProductDTO selectedProduct)
    {
        if(SessionManager.CurrentUser.AccountPoints < selectedProduct.PriceInPoints)
        {
            AnsiConsole.MarkupLine("[red]You do not have enough reward points to redeem this item![/]");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
            return;
        }
        OrderLogic.AddToCart(selectedProduct.Product, 1, selectedProduct.Product.Price, selectedProduct.PriceInPoints);
        
        RewardLogic.ChangeRewardPoints(SessionManager.CurrentUser.ID, SessionManager.CurrentUser.AccountPoints - selectedProduct.PriceInPoints);
        SessionManager.CurrentUser.AccountPoints -= selectedProduct.PriceInPoints;

        AnsiConsole.MarkupLine($"[green]{selectedProduct.Product.Name}[/] has been added to your cart using reward points!");
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
        Console.ReadKey();
    }
}