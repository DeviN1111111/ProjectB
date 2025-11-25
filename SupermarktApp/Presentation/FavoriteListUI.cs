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
                    // ViewLists();
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

        // DisplayProductsInList(selectedList);
    }
}