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
            options.AddRange(new[] { "View lists", "Create list", "Remove list", $"[red]Go back[/]" });

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
                    CreateList();
                    break;
                case "Remove list":
                    RemoveList();
                    break;
                case "[red]Go back[/]":
                    return;
            }
        }    
    }
    public static void RemoveList()
    {
        Console.Clear();
        Utils.PrintTitle("Remove List");

        var userId = SessionManager.CurrentUser.ID;
        List<FavoriteListModel> lists = FavoriteListLogic.GetAllListsByUserId(userId);

        var labels = lists
            .Select(l => l.Name)
            .ToList();
        labels.Add($"[red]Go back[/]");

        var selectedLabel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select list to remove")
                .AddChoices(labels));

        if (selectedLabel == $"[red]Go back[/]")
            return;

        int selectedIndex = labels.IndexOf(selectedLabel);
        FavoriteListModel selectedList = lists[selectedIndex];

        var confirm = Utils.CreateYesNoSelectionPrompt($"Are you sure you want to remove [yellow]{selectedList.Name}[/]?");

        if (!confirm)
            return;

        FavoriteListLogic.RemoveList(selectedList.Id);

        AnsiConsole.MarkupLine($"List [yellow]{selectedList.Name}[/] removed.");
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
        Console.ReadKey();
        return;
    }
    public static void CreateList()
    {
        Console.Clear();
        Utils.PrintTitle("Create List");

        var userId = SessionManager.CurrentUser.ID;

        var listName = Utils.CreateTextPrompt<string>("Enter list name: ");

        var newList = new FavoriteListModel(userId, listName);
        FavoriteListLogic.CreateList(newList);

        AddProductToList(newList);
    }
    public static void ViewLists()
    {
        Console.Clear();
        Utils.PrintTitle("Favorite Lists");

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
                    CreateList();
                    ViewLists();
                    break;
                case "No":
                    return;
            }
        }
        
        // Create the labels with the list names
        var labels = lists
                        .Select(l => l.Name)
                        .ToList();
        labels.Add($"[red]Go back[/]");

        var selectedLabel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select list to view")
                .AddChoices(labels));

        if (selectedLabel == "[red]Go back[/]") return;

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
        
        foreach (var item in allProductsInList)
        {
            var product = item.Product;
            int quantity = item.Quantity;
            double totalPrice = Math.Round(product.Price * quantity, 2);

            ProductDiscountDTO productDiscount = DiscountsLogic.CheckDiscountByProduct(product);

            if(productDiscount != null)
            {
                table.AddRow(
                    $"[#5dabcf]{product.Name}[/]", 
                    $"{quantity}",
                    $"[green]€{Math.Round(totalPrice * (1 - productDiscount.Discount.DiscountPercentage / 100), 2)}[/]");
            }
            else
            {
                table.AddRow(
                    $"[#5dabcf]{product.Name}[/]", 
                    $"{quantity}",
                    $"[green]€{totalPrice}[/]");
            }
        }

        AnsiConsole.Write(table);

        var choices = new [] {
            "Add products to cart",
            "Edit list",
            $"[red]Go back[/]"
        };
        var selectedChoice = Utils.CreateSelectionPrompt(choices);

        switch (selectedChoice)
        {
            case "Add products to cart":
                AddProductsToCart(list);
                ViewLists();
                break;
            case "Edit list":
                EditList(list);
                ViewLists();
                break;
            case "Go back":
                return;
        }
    }
    public static void EditList(FavoriteListModel list)
    {
        var allProductsInList = list.Products;

        var choices = new [] {
          "Add product",
          "Remove product",
          "Edit quantity",
          "Change list name",
          $"[red]Go back[/]"  
        };
        var selectedChoice = Utils.CreateSelectionPrompt(choices);

        switch (selectedChoice)
        {
            case "Add product":
                AddProductToList(list);
                break;
            case "Remove product":
                RemoveProductFromList(list);
                break;
            case "Edit quantity":
                EditQuantity(list, allProductsInList, true);
                break;
            case "Change list name":
                ChangeListName(list);
                break;
            case $"[red]Go back[/]":
                return;
        }

    }
    public static void ChangeListName(FavoriteListModel list)
    {
        Console.Clear();
        Utils.PrintTitle(list.Name);

        var newName = Utils.CreateTextPrompt<string>("Enter new name");

        FavoriteListLogic.ChangeListName(list.Id, newName);

        AnsiConsole.MarkupLine($"List name changed to [yellow]{newName}[/].");
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
        Console.ReadKey();
        return;
    }
    public static void AddProductToList(FavoriteListModel list)
    {
        bool confirmed = false;
        while(!confirmed)
        {        
            Func<int, ValidationResult> quantityValidator = quantity =>
                quantity < 1 || quantity > 99 ?
                    ValidationResult.Error("[red]Must be between 1 and 99.[/]"):
                    ValidationResult.Success();

            var product = SearchUI.SearchProductByNameOrCategory();

            if (product == null) continue;
        
            int quantity = Utils.CreateTextPrompt("Enter quantity (1-99)", quantityValidator);

            FavoriteListLogic.AddProductToList(product, quantity, list.Id);

            var decision = Utils.CreateSelectionPrompt(["Add more products", $"[red]Go back[/]"]);
            if (decision == $"[red]Go back[/]") confirmed = true;
        }
        AnsiConsole.MarkupLine("Product added to list.");
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
        Console.ReadKey();
        return;
    }
    public static void RemoveProductFromList(FavoriteListModel list)
    {
        Console.Clear();

        var products = list.Products;
        var title = "Choose a products to remove";
        DisplayOnlyProductsTable(list);
        
        var selectedProducts = Utils.CreateMultiSelectionPrompt(
            products,
            title,
            item => $"{item.Product.Name} | x{item.Quantity}"
        );

        if (selectedProducts is null || !selectedProducts.Any())
        {
            AnsiConsole.MarkupLine("No product selected.");
            AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
            Console.ReadKey();
            return;           
        }

        foreach (var item in selectedProducts)
        {
            FavoriteListLogic.RemoveProductFromList(item.Product, list.Id);
        }

        AnsiConsole.MarkupLine("Products removed from list.");
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
        Console.ReadKey();
        return;        
    }
    public static void AddProductsToCart(FavoriteListModel list)
    {
        var selectedChoice = Utils.CreateSelectionPrompt(new [] {"Add all products", "Add specific products", "[red]Go back[/]"});
        var allProductsInList = list.Products;
        
        if (selectedChoice == "Add all products")
        {
            foreach (var item in allProductsInList)
            {
                var product = ProductLogic.GetProductById(item.ProductId);
                if (product.Quantity >= item.Quantity)
                {
                    OrderLogic.AddToCart(item.Product, item.Quantity);
                }
                else
                {
                    OrderLogic.AddToCart(item.Product, product.Quantity);
                }
            }
        }
        else if (selectedChoice == "Add specific products")
        {
            Console.Clear();
            Utils.PrintTitle(list.Name);

            var products = allProductsInList;
            var title = "Select products";

            var selectedProducts = Utils.CreateMultiSelectionPrompt(
                products,
                title,
                item => $"{item.Product.Name} | x{item.Quantity}"
            );

            var listToAddToCart = EditQuantity(list, selectedProducts, false);

            foreach(var item in listToAddToCart)
            {
                var product = ProductLogic.GetProductById(item.ProductId);
                if (product.Quantity >= item.Quantity)
                {
                    OrderLogic.AddToCart(item.Product, item.Quantity);
                }
                else
                {
                    OrderLogic.AddToCart(item.Product, product.Quantity);
                }
            }
        }
        else return;

        AnsiConsole.MarkupLine("Products added to cart.");
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
        Console.ReadKey();
        return;
    }
    public static List<FavoriteListProductModel> EditQuantity(
        FavoriteListModel list,
        List<FavoriteListProductModel> selectedProducts, 
        bool updateToDatabase)
    {
        var updatedList = selectedProducts;

        var editQuantity = Utils.CreateYesNoSelectionPrompt("Edit Quantity?");

        if (!editQuantity)
        {
            return updatedList;
        }   

        bool confirmed = false;
        while (!confirmed)
        {               
            Console.Clear();
            Utils.PrintTitle(list.Name);
            DisplayOnlyProductsTable(list);

            var productToChange = Utils.CreateSelectionPrompt(
                selectedProducts, 
                "Choose a product", 
                item => $"{item.Product.Name} | x{item.Quantity}");

            // We make a validation lambda to pass as whole
            Func<int, ValidationResult> quantityValidator = quantity =>
                quantity < 1 || quantity > 99 ?
                    ValidationResult.Error("[red]Must be between 1 and 99.[/]"):
                    ValidationResult.Success();
            string textPrompt = $"Enter new quantity for [yellow]{productToChange.Product.Name}[/]:";

            var newQuantity = Utils.CreateTextPrompt<int>(textPrompt, quantityValidator);

            productToChange.Quantity = newQuantity;

            if (updateToDatabase)
            {
                FavoriteListLogic.EditProductQuantity(list, updatedList);
            }            

            var decision = Utils.CreateSelectionPrompt(["Change another?", $"[red]Go back[/]"]);
            if (decision == $"[red]Go back[/]") confirmed = true;
        }

        AnsiConsole.MarkupLine("Quantity updated");
        AnsiConsole.MarkupLine("Press [green]ENTER[/] to continue");
        Console.ReadKey();        

        return updatedList;             
    }
    public static void DisplayOnlyProductsTable(FavoriteListModel list)
    {
        var table = Utils.CreateTable(new [] {"Product", "Quantity", "Total Price"});

        var allProductsInList = list.Products;
        
        foreach (var item in allProductsInList)
        {
            var product = item.Product;
            int quantity = item.Quantity;
            double totalPrice = Math.Round(product.Price * quantity, 2);

            table.AddRow(
                $"[#5dabcf]{product.Name}[/]", 
                $"{quantity}",
                $"[green]€{totalPrice}[/]");
        }

        AnsiConsole.Write(table);
    }
}