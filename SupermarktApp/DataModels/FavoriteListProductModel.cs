class FavoriteListProductModel
{
    public int Id { get; set; }
    public int FavoriteListId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public ProductModel Product { get; set; }
}