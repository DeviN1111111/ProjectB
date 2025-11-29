using Microsoft.VisualBasic;

class FavoriteListModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<ProductModel, int> Products { get; set; }

    public FavoriteListModel(int userId, string name)
    {
        UserId = userId;
        Name = name;
        Products = new Dictionary<ProductModel, int>();
    }
    public FavoriteListModel()
    {
        Products = new Dictionary<ProductModel, int>();
    }
}