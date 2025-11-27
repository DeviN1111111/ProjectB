using Spectre.Console;
/// <summary>
/// A list of Utils for printing in AnsiConsole.
/// </summary>
static class Utils
{
    /// <summary>
    /// Prints a title.
    /// </summary>
    /// <param name="title">The string to print.</param>
    public static void Title(string title)
    {
            AnsiConsole.Write(
                new FigletText(title)
                    .Centered()
                    .Color(MenuUI.AsciiPrimary));        
    }
    /// <summary>
    /// Creates and prints selection prompt with the choices passed.
    /// </summary>
    /// <param name="choices">The list of choices to print.</param>
    /// <param name="title">The title of the prompt.</param>
    /// <returns>The selected choice as string.</returns>
    public static string SelectionPrompt(List<string> choices, string title= "")
    {
        if (title == "")
        {
            var prompt = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(10)
                    .AddChoices(choices));
            return prompt;           
        }
        else
        {
            var prompt = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(title)
                    .PageSize(10)
                    .AddChoices(choices));
            return prompt;             
        }
    }
    /// <summary>
    /// Creates a table with the strings passed (3 columns).
    /// </summary>
    /// <param name="column1">The first column.</param>
    /// <param name="column2">The second column.</param>
    /// <param name="column3">The third column.</param>
    /// <returns>The table.</returns>
    public static Table Table(string column1, string column2, string column3)
    {
        var table = new Table()
            .BorderColor(MenuUI.AsciiPrimary)
            .AddColumn(column1)
            .AddColumn(column2)
            .AddColumn(column3);
        return table;
    }
}