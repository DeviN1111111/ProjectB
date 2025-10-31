using Spectre.Console;
// --> after the user adds the item to the checklist

public class ChecklistLogic // ik volg hiervoor de code die in OrderLogic.cs staat
{
    public static void AddToChecklist(ProductModel product, int quantity)
    {

        List<ChecklistModel> allUserProducts = ChecklistAccess.GetAllUserProducts(SessionManager.CurrentUser.ID);
        var ChecklistItem = allUserProducts.FirstOrDefault(item => item.ProductId == product.ID);
        if (ChecklistItem != null)
        {
            int newQuantity = ChecklistItem.Quantity + quantity;
            if (newQuantity > 99)
            {
                newQuantity = 99;
            }
            ChecklistAccess.RemoveFromChecklist(SessionManager.CurrentUser.ID, product.ID);
            ChecklistAccess.AddToChecklist(SessionManager.CurrentUser.ID, product.ID, newQuantity);
            return;
        }
        ChecklistAccess.AddToChecklist(SessionManager.CurrentUser.ID, product.ID, quantity);

    }

    public static List<ChecklistModel> AllUserProducts()
    {
        List<ChecklistModel> allUserProducts = ChecklistAccess.GetAllUserProducts(SessionManager.CurrentUser.ID);
        return allUserProducts;
    }

    public static void ClearChecklist()
    {
        ChecklistAccess.ClearChecklist();
    }

    
    public static void RemoveFromChecklist(int productId)
    {
        ChecklistAccess.RemoveFromChecklist(SessionManager.CurrentUser.ID, productId);
    }

    
}