using System.Runtime.CompilerServices;
using NUnit.Framework;
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
                    .Color(ColorUI.AsciiPrimary));        
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
    /// Creates an empty table with the strings passed as columns.
    /// </summary>
    /// <param name="columns">IEnumerable of strings for the columns.</param>
    /// <returns>The table.</returns>
    public static Table CreateTable(IEnumerable<string> columns, string title = "")
    {

        var table = new Table()
            .BorderColor(ColorUI.AsciiPrimary);

        if (title != "")
            table.Title(title);

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
            .PageSize(10)
            .NotRequired();

        if (title != "")
            prompt.Title(title);

        if (format != null)
            prompt.UseConverter(format);

        prompt.AddChoices(choices);

        return AnsiConsole.Prompt(prompt);
    }

    public static List<T> CreateMultiSelectionPromptWithSelectAll<T>(IEnumerable<T> choices, T selectAll, string title= "", Func<T, string>? format= null) where T : notnull
    {
        var prompt = new MultiSelectionPrompt<T>()
            .PageSize(10);

        if (title != "")
            prompt.Title(title);

        if (format != null)
            prompt.UseConverter(format);

        prompt.AddChoiceGroup<T>(selectAll ,choices);

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
    /// <summary>
    /// Formats price to 0,00
    /// </summary>
    /// <param name="price">The price</param>
    /// <param name="color">Optional color for the price</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Formatted price string</returns>
    public static string ChangePriceFormat<T>(T price, string? color = null)
    {
        if (color != null)
        {
            return $"[{color}]€" + Math.Round(Convert.ToDecimal(price), 2).ToString("0.00").Replace(",",".") + "[/]";
        }
        return "€" + Math.Round(Convert.ToDecimal(price), 2).ToString("0.00").Replace(",",".");
    }
    /// <summary>
    /// Calculates discounted price and returns a formatted string with old and new price.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="PriceBeforeDiscount">Price of product before discount</param>
    /// <param name="discountPercentage">Discount percentage to apply</param>
    /// <returns>the formatted string with old and new price</returns>
    public static string CalculateDiscountedPriceString<T>(T PriceBeforeDiscount, T discountPercentage)
    {
        decimal PriceAfterDiscount = Convert.ToDecimal(PriceBeforeDiscount) * (1 - (Convert.ToDecimal(discountPercentage) / 100m));

        string newPrice = ChangePriceFormat(PriceAfterDiscount);

        string oldPrice = ChangePriceFormat(PriceBeforeDiscount);

        return $"[strike red]{oldPrice}[/] [green]{newPrice}[/]";
    }
    /// <summary>
    /// Prompts the user to enter an integer within an optional range.
    /// </summary>
    /// <param name="text">The text prompt</param>
    /// <param name="min">The minimum acceptable value (inclusive)</param>
    /// <param name="max">The maximum acceptable value (inclusive)</param>
    /// <returns>User input as an integer.</returns>
    public static int AskInt(string text, int? min = null, int? max = null)
    {
        var prompt = new TextPrompt<int>(text ?? "Enter a number")
            .PromptStyle("green")
            .ValidationErrorMessage("[red]Invalid number.[/]")
            .Validate(num =>
            {
                if (min.HasValue && num < min.Value)
                    return ValidationResult.Error($"[red]Minimum is {min.Value}[/]");
                if (max.HasValue && num > max.Value)
                    return ValidationResult.Error($"[red]Maximum is {max.Value}[/]");
                return ValidationResult.Success();
            });

        return AnsiConsole.Prompt(prompt);
    }
    
    /// <summary>
    /// Calculates discounted price the formatted value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="PriceBeforeDiscount"></param>
    /// <param name="discountPercentage"></param>
    /// <returns>formattedPrice as single string without old price</returns>
    public static string CalculateDiscountedPrice<T>(T PriceBeforeDiscount, T discountPercentage)
    {
        decimal PriceAfterDiscount = Convert.ToDecimal(PriceBeforeDiscount) * (1 - (Convert.ToDecimal(discountPercentage) / 100m));

        return ChangePriceFormat(PriceAfterDiscount);
    }
    /// <summary>
    /// Prompts the user to enter a double within an optional range.
    /// </summary>
    /// <param name="text">The text prompt</param>
    /// <param name="min">The minimum acceptable value (inclusive)</param>
    /// <param name="max">The maximum acceptable value (inclusive)</param>
    /// <returns>User input as a double.</returns
    public static double AskDouble(string text, double? min = null, double? max = null)
    {
        var prompt = new TextPrompt<double>(text ?? "Enter a number")
            .PromptStyle("green")
            .ValidationErrorMessage("[red]Invalid number.[/]")
            .Validate(num =>
            {
                if (min.HasValue && num < min.Value)
                    return ValidationResult.Error($"[red]Minimum is {min.Value}[/]");
                if (max.HasValue && num > max.Value)
                    return ValidationResult.Error($"[red]Maximum is {max.Value}[/]");
                return ValidationResult.Success();
            });

        return AnsiConsole.Prompt(prompt);
    }
}