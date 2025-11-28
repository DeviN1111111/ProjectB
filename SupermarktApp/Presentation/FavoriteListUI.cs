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

        Utils.PrintTitle(list.Name);
        var table = Utils.CreateTable(new [] {"Product", "Quantity", "Total Price"});

        var allProductsInList = list.Products;
        
        foreach (var kv in allProductsInList)
        {
            var product = kv.Key;
            int quantity = kv.Value;
            double totalPrice = Math.Round(product.Price * quantity, 2);

            table.AddRow(
                $"[#5dabcf]{product.Name}[/]", 
                $"{quantity}",
                $"[green]{totalPrice}[/]");
        }

        AnsiConsole.Write(table);

        var choices = new [] {
            "Add products to cart",
            "Edit list",
            "Go back"
        };
        var selectedChoice = Utils.CreateSelectionPrompt(choices);

        switch (selectedChoice)
        {
            case "Add products to cart":
                // AddProductsToCart()
                break;
            case "Edit list":
                // EditList()
                break;
            case "Go back":
                return;
        }
    }
    public static void AddProductsToCart(FavoriteListModel list)
    {
        /*
        1. First display choices "Add all products" "Add specific products"
        2. Add all products: loop through all ProductModels in list.Products and call OrderLogic.AddToCart()
        3. Add specific products: 
            create MultiSelectionPrompt with each product name and quantity
            prompt the user to select products to add to cart and quantity
            leave the selected products with new quantity marked
            use space to select and enter to proceed
            loop through each selected product and call OrderLogic.AddToCart()
        */
        Console.Clear();

        Utils.PrintTitle(list.Name);
        var selectedOption = Utils.CreateSelectionPrompt(new [] {"Add all products", "Add specific products"});

        var allProductsInList = list.Products;
        
        if (selectedOption == "Add all products")
        {
            foreach (var kv in allProductsInList)
            {
                var product = kv.Key;
                var quantity = kv.Value;

                OrderLogic.AddToCart(product, quantity);
            }
        }
        else
        {
            var products = allProductsInList.Keys;
            var title = "Select products";

            var selectedProducts = Utils.CreateMultiSelectionPrompt(
                products,
                title,
                product => $"{product.Name} | x{allProductsInList[product]}" // allProductsInList[product] is the value of the dictonary in this case quantity
            );

            var editQuantity = Utils.Create_YesNo_SelectionPrompt("Edit Quantity?");

            if (editQuantity)
            {
                /*
                First loop to each product and prompt to select the product to edit quantity. Then proceed with the rest.
                Create new method for EditQuantity
                */
                // We make a validation lambda to pass as whole
                Func<int, ValidationResult> quantityValidator = quantity =>
                    quantity < 1 || quantity > 9 ?
                        ValidationResult.Error("[red]Must be between 1 and 99.[/]"):
                        ValidationResult.Success();
                string textPrompt = $"Enter new quantity for [yellow][/]:";
                var newQuantity = Utils.CreateTextPrompt<int>(textPrompt, quantityValidator);
            }

        }
    }
}