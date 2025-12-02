class FavoriteListModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<FavoriteListProductModel> Products { get; set; }

    public FavoriteListModel(int userId, string name)
    {
        UserId = userId;
        Name = name;
        Products = new List<FavoriteListProductModel>();
    }

    public FavoriteListModel()
    {
        Products = new List<FavoriteListProductModel>();
    }
}