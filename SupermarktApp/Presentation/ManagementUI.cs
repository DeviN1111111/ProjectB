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
            new FigletText("Product Management")
                .Centered()
                .Color(AsciiPrimary));

        var period = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { "Edit product details", "Add new product", "Delete product", "Go back" }));

        switch (period)
        {
            case "Go back":
                return;

            case "Add new product":
                AddProduct();
                break;

            case "Delete product":
                DeleteProduct();
                break;

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
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Change Product Details")
                .Centered()
                .Color(AsciiPrimary));

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

    public static void AddProduct()
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Add Product")
                .Centered()
                .Color(AsciiPrimary));

        var name = AnsiConsole.Prompt(new TextPrompt<string>("New name of product:"));
        var price = AnsiConsole.Prompt(new TextPrompt<double>("New price of product:"));
        var nutritionDetails = AnsiConsole.Prompt(new TextPrompt<string>("New nutritionDetails of product:"));
        var description = AnsiConsole.Prompt(new TextPrompt<string>("New description of product:"));
        var category = AnsiConsole.Prompt(new TextPrompt<string>("New category of product:"));
        int location;
        do
        {
            location = AnsiConsole.Prompt(new TextPrompt<int>("New location of product: (Max 43)"));
        } while (ValidaterLogic.ValidateLocationProduct(location) == false);
        var quantity = AnsiConsole.Prompt(new TextPrompt<int>("New quantity of product:"));

        ProductLogic.AddProduct(name, price, nutritionDetails, description, category, location, quantity);
        ProductDetailsUI.ShowProductDetails(new ProductModel(name, price, nutritionDetails, description, category, location, quantity));
        AnsiConsole.MarkupLine($"[green]Succesfully added [red]{name}[/], press enter to continue.[/]");
        Console.ReadKey();
    }
    
    public static void DeleteProduct()
    {
        Console.Clear();
        AnsiConsole.Write(
            new FigletText("Delete Product")
                .Centered()
                .Color(AsciiPrimary));

        ProductModel EditProduct = ProductLogic.SearchProductByNameOrCategory();
        AnsiConsole.MarkupLine($"Are you sure you want to delete [red]{EditProduct.Name}[/]? This action cannot be undone.");
        var confirm = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .HighlightStyle(new Style(Hover))
                .AddChoices(new[] { "Confirm", "Cancel" }));
        switch(confirm)
        {
            case "Confirm":
                ProductLogic.DeleteProductByID(EditProduct.ID);
                Console.Clear();
                AnsiConsole.Write(
                new FigletText("Delete Product")
                    .Centered()
                    .Color(AsciiPrimary));
                AnsiConsole.MarkupLine($"[green]Succesfully[/] deleted [red]{EditProduct.Name}[/], press [green]ENTER[/] to continue.");
                Console.ReadKey();
                break;
            case "Cancel":
                return;
        }
    }
}