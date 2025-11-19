public class ShopReviewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int Stars { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }

    public ShopReviewModel(int userId, int stars, string text, DateTime? createdAt = null)
    {
        UserId = userId;
        Stars = stars;
        Text = text;
        CreatedAt = createdAt ?? DateTime.Now;
    }
    
    public ShopReviewModel()
    {
        
    }
}