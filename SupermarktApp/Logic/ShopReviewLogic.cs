using NUnit.Framework.Constraints;
using Spectre.Console;

public class ShopReviewLogic
{
    public List<ShopReviewModel> GetReviews(int userId)
    {
        if (userId <= 0)
            throw new Exception("Invalid userId");
        return ShopReviewAcces.GetAllReviews(userId);
    }

    public List<ShopReviewModel> GetAllReviews()
    {
        return ShopReviewAcces.GetAllReviews();
    }

    public void AddReview(int userId, int stars, string text)
    {
        if (userId <= 0)
            throw new Exception("INVALID USER ID");

        if (stars < 1 || stars > 5)
            throw new Exception("Stars must be between 1 and 5.");

        if (string.IsNullOrWhiteSpace(text))
            text = null;

        ShopReviewAcces.AddReview(userId, stars, text, DateTime.UtcNow);
    }
    public double GetAverageStars(int userId)
    {
        var reviews = GetReviews(userId);

        if (reviews.Count == 0)
            return 0;

        return reviews.Average(r => r.Stars);
    }

    public static void DeleteReviewByID(int reviewID)
    {
        ShopReviewAcces.DeleteReviewByID(reviewID);
    }
}