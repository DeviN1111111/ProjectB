public class OrderLogic
{ 
    public static void AddToCart( ProductModel product, int quantity)
    {
        //Order.ProductsInCart.Add(1);
        CartAccess.AddToCart(SessionManager.CurrentUser.ID, product.ID, quantity);
    }

    public static List<CartModel> AllUserProducts()
    {
        List<CartModel> allUserProducts = CartAccess.GetAllUserProducts(1);
        return allUserProducts;
    }
}