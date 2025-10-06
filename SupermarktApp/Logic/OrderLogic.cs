public class OrderLogic
{
    public static void AddToCart(ProductModel product, int quantity)
    {
        //Order.ProductsInCart.Add(1);
        CartAccess.AddToCart(1, product.Id, quantity);
    }

    // public static List<CartModel> AllUserProducts()
    // {
    // }
}