public class OrderLogic
{
    public static void AddToCart(int product, int quantity)
    {
        //Order.ProductsInCart.Add(1);
        CartAccess.AddToCart(1, product, quantity);
    }

    public static List<CartModel> AllUserProducts()
    {
        List<CartModel> allUserProducts = CartAccess.GetAllUserProducts(1);
        return allUserProducts;
    }
}