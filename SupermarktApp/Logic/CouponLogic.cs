static class CouponLogic
{
    private static readonly string CouponTemplatePath = "EmailTemplates/CouponTemplate.html";
    private static readonly string CouponTemplate = File.ReadAllText(CouponTemplatePath);
    public static bool IsCouponValid(int couponId)
    {
        var coupon = CouponAccess.GetCouponById(couponId);

        if (coupon!.IsValid) return true;
        return false;
    }
    public static Coupon? GetCouponByUserId(int userId) => CouponAccess.GetCouponByUserId(userId);
    public static Coupon? GetCouponByCode(int code)
    {
        var coupon = CouponAccess.GetCouponByCode(code);
        if (IsCouponValid(coupon!.Id)) return coupon;
        return coupon;
    }
    public static bool ActivateCoupon(int couponCode)
    {
        var coupon = CouponAccess.GetCouponByCode(couponCode);
        if (coupon == null) return false;
        if (!coupon.IsValid)
        {
            coupon.IsValid = true;
            CouponAccess.EditCoupon(coupon);
        }
        return true;
    }
    public static void UseCoupon(int couponId) => CouponAccess.UseCoupon(couponId);
    public static void CreateCoupon(int userId, double credit) => CouponAccess.AddCoupon(userId, credit);
    public static async Task SendEmail(int couponCode, string email)
    {
        string emailBody = CouponTemplate.Replace("{{COUPON_CODE}}", couponCode.ToString());

        await EmailLogic.SendEmailAsync(
            to: email,
            subject: "Your Coupon Code",
            body: emailBody,
            isHtml: true
        );
    }
    public static List<Coupon> GetAllCoupons(int userId) => CouponAccess.GetAllCouponsByUserId(userId);
}
