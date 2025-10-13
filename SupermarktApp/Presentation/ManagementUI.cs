using Spectre.Console;

public static class ManagementUI
{
    public static readonly Color Text = Color.FromHex("#E8F1F2");
    public static readonly Color Hover = Color.FromHex("#006494");
    public static readonly Color Confirm = Color.FromHex("#13293D");
    public static readonly Color AsciiPrimary = Color.FromHex("#247BA0");
    public static readonly Color AsciiSecondary = Color.FromHex("#1B98E0");
    public static void DisplayMenu()
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("SuperMart Analytics")
                .Centered()
                .Color(AsciiPrimary));

        var period = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { "Edit product details", "Go back" }));

        switch (period)
        {
            case "Go back":
                return;

            case "Edit product details":
                ChangeProductDetails();
                break;

            default:
                AnsiConsole.MarkupLine("[red]Invalid selection[/]");
                break;
        }

    }
    
    public static void ChangeProductDetails()
    {
        ProductModel EditProduct = ProductLogic.SearchProductByNameOrCategory();
        var name = AnsiConsole.Prompt(new TextPrompt<string>("New name of product:").DefaultValue(EditProduct.Name));
        var price = AnsiConsole.Prompt(new TextPrompt<double>("New price of product:").DefaultValue(EditProduct.Price));
        var nutritionDetails = AnsiConsole.Prompt(new TextPrompt<string>("New nutritionDetails of product:").DefaultValue(EditProduct.NutritionDetails));
        var description = AnsiConsole.Prompt(new TextPrompt<string>("New description of product:").DefaultValue(EditProduct.Description));
        var category = AnsiConsole.Prompt(new TextPrompt<string>("New category of product:").DefaultValue(EditProduct.Category));
        int location;
        do
        {
            location = AnsiConsole.Prompt(new TextPrompt<int>("New location of product: (Max 43)").DefaultValue(EditProduct.Location));
        } while (ValidaterLogic.ValidateLocationProduct(location) == false);
        var quantity = AnsiConsole.Prompt(new TextPrompt<int>("New quantity of product:").DefaultValue(EditProduct.Quantity));

        ProductLogic.ChangeProductDetails(EditProduct.ID, name, price, nutritionDetails, description, category, location, quantity);
        AnsiConsole.MarkupLine("[green]Succesfully edited press enter to continue.[/]");
        Console.ReadKey();
    }
}