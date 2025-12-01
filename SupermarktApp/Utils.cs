using NUnit.Framework.Interfaces;
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
    public static void PrintTitle(string title)
    {
            AnsiConsole.Write(
                new FigletText(title)
                    .Centered()
                    .Color(MenuUI.AsciiPrimary));        
    }
    /// <summary>
    /// Creates and prints selection prompt with the choices passed.
    /// </summary>
    /// <param name="choices">The IEnumerable of choices to print.</param>
    /// <param name="title">The title of the prompt.</param>
    /// <param name="format">The format to be printed.</param>
    /// <typeparam name="T">ProductModel.</typeparam>
    /// <returns>The selected choice as string.</returns>
    public static T CreateSelectionPrompt<T>(IEnumerable<T> choices, string title= "", Func<T, string>? format= null) where T : notnull
    {
        var prompt = new SelectionPrompt<T>()
            .PageSize(10);

        if (title != "")
            prompt.Title(title);

        if (format != null)
            prompt.UseConverter(format);

        prompt.AddChoices(choices);

        return AnsiConsole.Prompt(prompt);
    }
    /// <summary>
    /// Creates a selection prompt for yes or no for fast booleans.
    /// </summary>
    /// <param name="title">The title of the prompt</param>
    /// <returns>True or false</returns>
    public static bool CreateYesNoSelectionPrompt(string title= "")
    {
        string yes = $"[green]Yes[/]";
        string no =  $"[red]No[/]";
        if (title == "")
        {
            var prompt = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(10)
                    .AddChoices(yes, no));

            bool result = prompt switch
            {
                $"[green]Yes[/]" => true,
                $"[red]No[/]" => false,
                _ => false
            };

            return result;      
        }
        else
        {
            var prompt = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(title)
                    .PageSize(10)
                    .AddChoices(yes, no)); 

            bool result = prompt switch
            {
                $"[green]Yes[/]" => true,
                $"[red]No[/]" => false,
                _ => false
            };

            return result;            
        }
    }
    /// <summary>
    /// Creates a table with the strings passed.
    /// </summary>
    /// <param name="columns">IEnumerable of strings for the columns.</param>
    /// <returns>The table.</returns>
    public static Table CreateTable(IEnumerable<string> columns)
    {
        var table = new Table()
            .BorderColor(MenuUI.AsciiPrimary);

        foreach(string column in columns)
        {
            table.AddColumn(column);
        }

        return table;
    }
    /// <summary>
    /// Creates a MultiSelectionPrompt with the choices passed.
    /// And prints in the format passed.
    /// </summary>
    /// <param name="choices">IEnumerable of generic types for choices.</param>
    /// <param name="title">The title of the prompt.</param>
    /// <param name="format">The format to be printed.</param>
    /// <typeparam name="T">ProductModel.</typeparam>
    /// <returns>The selected choices as a List</returns>
    public static List<T> CreateMultiSelectionPrompt<T>(IEnumerable<T> choices, string title= "", Func<T, string>? format= null) where T : notnull
    {
        var prompt = new MultiSelectionPrompt<T>()
            .PageSize(10);

        if (title != "")
            prompt.Title(title);

        if (format != null)
            prompt.UseConverter(format);

        prompt.AddChoices(choices);

        return AnsiConsole.Prompt(prompt);
    }
    /// <summary>
    /// Prompts a text and validates user input.
    /// </summary>
    /// <param name="text">The text prompt</param>
    /// <param name="validator">The condition for the result to be valid</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>User input.</returns>
    public static T CreateTextPrompt<T>(string text, Func<T, ValidationResult> validator)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<T>(text)
                .Validate(validator)
        );
    }
    /// <summary>
    /// Prompts a text and gets user input.
    /// </summary>
    /// <param name="text">The text prompt</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>User input.</returns>
    public static T CreateTextPrompt<T>(string text)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<T>(text)
        );
    }
}