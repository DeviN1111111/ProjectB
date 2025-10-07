using Spectre.Console;

public class Order
{    public static void ShowCart()
    {
        Console.Clear();
        double totalAmount = 0;
        CartHeader();

        List<CartModel> allUserProducts = OrderLogic.AllUserProducts();  // List of user Products in cart
        List<ProductModel> allProducts = ProductAccess.GetAllProducts();  // List of all products

        // Loop through user products in cart
        foreach (var cartProduct in allUserProducts)
        {
            // Get Product id and find match in all products
            foreach (ProductModel Product in allProducts)
            {
                if (cartProduct.ProductId == Product.Id)
                {
                    // Create two text elements
                    var product = new Text($"{Product.Name}", new Style(Color.Red));
                    var quantity = new Text($"{cartProduct.Quantity}", new Style(Color.Blue));
                    var price = new Text($"{Product.Price}", new Style(Color.Blue));
                    totalAmount += Product.Price;

                    // Create, apply padding on text elements
                    var paddedProduct = new Padder(product).PadRight(2).PadBottom(0).PadTop(0);
                    var paddedQuantity = new Padder(quantity).PadLeft(20).PadBottom(0).PadTop(0);
                    var paddedPrice = new Padder(price).PadLeft(2).PadBottom(0).PadTop(0);

                    // Insert the text elements into a single row grid
                    var row = new Grid();

                    row.AddColumn();
                    row.AddColumn();
                    row.AddColumn();
                    row.AddRow(paddedProduct, paddedQuantity, paddedPrice);

                    // Apply horizontal and vertical padding on the grid
                    var paddedRow = new Padder(row).Padding(4, 1);

                    // Write the padded grid to the Console
                    AnsiConsole.Write(paddedRow);
                }
            }


        }

        // Total Line
        var rule = new Rule("Total");
        rule.Justification = Justify.Left;
        AnsiConsole.Write(rule);
        // Total price
        var product1 = new Text($"{totalAmount}", new Style(Color.Red));
        var paddedProduct1 = new Padder(product1).PadLeft(41).PadBottom(0).PadTop(0);
        var row1 = new Grid();
        row1.AddColumn();
        row1.AddRow(paddedProduct1);
        var paddedRow1 = new Padder(row1).Padding(4, 1);
        AnsiConsole.Write(paddedRow1);

    }

    public static void CartHeader()
    {
        // Get List of user items in cart
        List<CartModel> allUserProducts = OrderLogic.AllUserProducts();
        // Create two text elements
        var productHeader = new Text("Product", new Style(Color.Red));
        var quantityHeader = new Text("Qty", new Style(Color.Blue));
        var priceHeader = new Text("Price", new Style(Color.Blue));

        // Create, apply padding on text elements
        var paddedProductHeader = new Padder(productHeader).PadRight(2).PadBottom(0).PadTop(0);
        var paddedQuantityHeader = new Padder(quantityHeader).PadLeft(20).PadBottom(0).PadTop(0);
        var paddedPriceHeader = new Padder(priceHeader).PadLeft(2).PadBottom(0).PadTop(0);

        // Insert the text elements into a single row grid
        var row = new Grid();

        row.AddColumn();
        row.AddColumn();
        row.AddColumn();
        row.AddRow(paddedProductHeader, paddedQuantityHeader, paddedPriceHeader);

        // Apply horizontal and vertical padding on the grid
        var paddedRow = new Padder(row).Padding(4, 1);

        // Write the padded grid to the Console
        AnsiConsole.Write(paddedRow);
    }
}