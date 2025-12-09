using Spectre.Console;
public static class SuggestionsUI
{
    public static Panel GetSuggestionsPanel(int userId)
    {
        var suggestions = SuggestionsLogic.GetSuggestedItems(userId);

        // If nothing to show, return a grey placeholder
        if (suggestions == null || suggestions.Count == 0)
        {
            return new Panel("[grey]No suggestions yet[/]")
                .Header("[bold aqua]Suggested Items[/]")
                .Border(BoxBorder.Double)
                .BorderStyle(new Style(Color.Grey));
        }

        // create the list
        var lines = new List<string>();
        for (int i = 0; i< suggestions.Count; i++)
        {   
            //user can chooose from suggestions 
            var s = suggestions[i];
            lines.Add($"[yellow]{i +1}.[/]{s.Name}");
        }

        string listText = string.Join("\n", lines);
        return new Panel(listText)
            .Header("[bold aqua]Suggested Items[/]")
            .Padding(1, 1)
            .Border(BoxBorder.Double);
    }
}