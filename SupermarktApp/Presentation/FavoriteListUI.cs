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
                AddProductsToCart(list);
                break;
            case "Edit list":
                EditList(list);
                break;
            case "Go back":
                return;
        }
    }
    public static void EditList(FavoriteListModel list)
    {
        /*
        1. Make layout the options are: add item, remove item, edit quantity, change list name
        2. Create the data access and logic to access the Products property and Add or Remove the product in the dictionary
        both methods should take in the quantity
        3. Use the EditQuantity() method
        4. Create the data access and logic to access the Name property and let user change it. Validation: can't be longer than 18 characters
        # Use same pattern as AddProductsToCart so there is no problem with EditQuantity #
        */
        var allProductsInList = list.Products;

        var choices = new [] {
          "Add product",
          "Remove product",
          "Edit quantity",
          "Change list name",
          "Go back"  
        };
        var selectedChoice = Utils.CreateSelectionPrompt(choices);

        switch (selectedChoice)
        {
            case "Add product":
                AddProductToList(list);
                break;
            case "Remove product":
                // RemoveProductFromList(list);
                break;
            case "Edit quantity":   // Need to update the database 
                EditQuantity(allProductsInList.Keys.ToList(), allProductsInList); // We pass both but the first argument expects a List<ProductModel>
                break;
            case "Change list name":
                // ChangeListName();
                break;
            case "Go back":
                return;
        }

    }
    public static void AddProductToList(FavoriteListModel list)
    {
        Func<int, ValidationResult> quantityValidator = quantity =>
            quantity < 1 || quantity > 99 ?
                ValidationResult.Error("[red]Must be between 1 and 99.[/]"):
                ValidationResult.Success();

        var product = SearchUI.SearchProductByNameOrCategory();
        int quantity = Utils.CreateTextPrompt("Enter quantity (1-99)", quantityValidator);

        FavoriteListLogic.AddProductToList(product, quantity, list.Id);
    }
    public static void RemoveProductFromList(FavoriteListModel list)
    {
        
    }
    public static void AddProductsToCart(FavoriteListModel list)
    {
        var selectedChoice = Utils.CreateSelectionPrompt(new [] {"Add all products", "Add specific products", "Go back"});
        var allProductsInList = list.Products;
        
        if (selectedChoice == "Add all products")
        {
            foreach (var kv in allProductsInList)
            {
                var product = kv.Key;
                var quantity = kv.Value;

                OrderLogic.AddToCart(product, quantity);
            }
        }
        else if (selectedChoice == "Add specific products")
        {
            Console.Clear();
            Utils.PrintTitle(list.Name);

            var products = allProductsInList.Keys;
            var title = "Select products";

            var selectedProducts = Utils.CreateMultiSelectionPrompt(
                products,
                title,
                product => $"{product.Name} | x{allProductsInList[product]}" // allProductsInList[product] is the value of the dictonary in this case quantity
            );

            var listToAddToCart = EditQuantity(selectedProducts, allProductsInList);

            foreach(var kv in listToAddToCart)
            {
                var product = kv.Key;
                var quantity = kv.Value;

                OrderLogic.AddToCart(product, quantity);
            }
        }

        else return;

        AnsiConsole.MarkupLine("Products added to cart.");
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
        Console.ReadKey();
    }
    public static Dictionary<ProductModel, int> EditQuantity(
        List<ProductModel> selectedProducts, 
        Dictionary<ProductModel, int> allProductsInList)
    {
        var updatedList = new Dictionary<ProductModel, int>();

        foreach(var product in selectedProducts)
        {
            updatedList[product] = allProductsInList[product];
        }

        var editQuantity = Utils.CreateYesNoSelectionPrompt("Edit Quantity?");

        if (!editQuantity)
        {
            return updatedList;
        }   

        bool confirmed = false;
        while (!confirmed)
        {               
            Console.Clear();

            var productToChange = Utils.CreateSelectionPrompt(
                selectedProducts, 
                "Choose a product", 
                product => $"{product.Name} | x{updatedList[product]}");

            // We make a validation lambda to pass as whole
            Func<int, ValidationResult> quantityValidator = quantity =>
                quantity < 1 || quantity > 99 ?
                    ValidationResult.Error("[red]Must be between 1 and 99.[/]"):
                    ValidationResult.Success();
            string textPrompt = $"Enter new quantity for [yellow][/]:";

            var newQuantity = Utils.CreateTextPrompt<int>(textPrompt, quantityValidator);

            updatedList[productToChange] = newQuantity;

            confirmed = !AnsiConsole.Confirm("Change another?");
        }

        AnsiConsole.MarkupLine("Quantity updated");
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
        Console.ReadKey();        

        return updatedList;             
    }
}