using Spectre.Console;
static class FavoriteListUI
{
    public static void DisplayMenu()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("Favorite List")
                    .Centered()
                    .Color(MenuUI.AsciiPrimary));
            
            var options = new List<string>();
            options.AddRange(new[] { "View lists", "Create list", "Remove list", "Go back" });

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(10)
                    .AddChoices(options));
            
            switch (choice)
            {
                case "View lists":
                    ViewLists();
                    break;
                case "Create list":
                    // CreateList();
                    break;
                case "Remove list":
                    // RemoveList();
                    break;
                case "Go back":
                    return;
            }
        }    
    }
    public static void ViewLists()
    {
        /*
        1. First I need to call the logic layer to get the favorite lists of the user using user ID from session manager,
        2. Then I need to display the lists names in a selection prompt just like coupons,
        3. After the user selects a list we will call another method that will display the products in that list so that method will have to work
        with any list passed to it, call it DisplayProductsInList(FavoriteList list),
        */
        Console.Clear();
        var userId = SessionManager.CurrentUser.ID;
        List<FavoriteListModel> lists = FavoriteListLogic.GetAllListsByUserId(userId);
    
        if (lists == null || lists.Count == 0)
        {
            AnsiConsole.MarkupLine("You have no favorite lists. Create one?");

            var options = new List<string>();
            options.AddRange("Yes", "No");
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(10)
                    .AddChoices(options));
            
            switch (choice)
            {
                case "Yes":
                    // CreateList();
                    break;
                case "No":
                    return;
            }
        }
        
        // Create the labels with the list names
        var labels = lists
                        .Select(l => l.Name)
                        .ToList();
        labels.Add("Go back");

        var selectedLabel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select list to view")
                .AddChoices(labels));

        if (selectedLabel == "Go back") return;

        // Find selected list
        int selectedIndex = labels.IndexOf(selectedLabel);
        FavoriteListModel selectedList = lists[selectedIndex];

        DisplayProductsInList(selectedList);
    }
    public static void DisplayProductsInList(FavoriteListModel list)
    {
        /*
        1. First I need to create the layout, the title should be the name of the chosen list. So I need to pass list.Name into ansiConsole title
        2. Then I need to get all ProductModels in that list and print them: {Name} {Quantity} {TotalPrice}
        Loop through all the Products in the list and print them in a table
        3. The options in this UI are: 
        {Add products to cart}(User can choose to add all products or specific ones)
        {Edit list}(Lets user add, remove and edit products in the list)
        {Go back}(Goes back to lists view)
        4. 
        */
        Console.Clear();
        Utils.Title(list.Name);
        var table = Utils.Table("Product", "Quantity", "Total Price");

        var allProductsInList = list.Products;
        Dictionary<int, int> productQuantities = allProductsInList  // #Pass ID to use#
            .GroupBy(p => p.ID) // Create a groups of ProductModels with the same ID's
            .ToDictionary(
                g => g.Key, // Store the ID's as keys
                g => g.Count());    // Store the count of ProductModels as value

        foreach (ProductModel product in allProductsInList)
        {
            int quantity = productQuantities[product.ID];
            double totalPrice = Math.Round(product.Price * quantity, 2);

            table.AddRow(
                $"[#5dabcf]{product.Name}[/]", 
                $"{quantity}",
                $"[green]{totalPrice}[/]");
        }

        AnsiConsole.Write(table);

        // Create the options in step 3.
    }
}