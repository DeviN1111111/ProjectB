static class CouponLogic
{
    public static bool IsCouponValid(int couponId)
    {
        var coupon = CouponAccess.GetCouponById(couponId);

        if (coupon!.IsValid) return true;
        return false;
    }
    public static Coupon? GetCouponByUserId(int userId) => CouponAccess.GetCouponByUserId(userId);
    public static Coupon? GetCouponById(int id) => CouponAccess.GetCouponById(id);
    public static void EditCoupon(Coupon coupon)
    {
        if (coupon == null) return;
        
        if (coupon.Credit > Order.TotalPrice)
        {
            coupon.Credit -= Order.TotalPrice;
        }
        else
        {
            Order.TotalPrice -= coupon.Credit;
            coupon.Credit = 0;
        }
        if (coupon.Credit <= 0) coupon.IsValid = false;        
        CouponAccess.EditCoupon(coupon);
    }
    public static void UseCoupon(int couponId)
    {
        var coupon = CouponAccess.GetCouponById(couponId);
        if (coupon == null) return;
        
        if (coupon.Credit > Order.TotalPrice)
        {
            coupon.Credit -= Order.TotalPrice;
        }
        else
        {
            Order.TotalPrice -= coupon.Credit;
            coupon.Credit = 0;
        }
        if (coupon.Credit <= 0) coupon.IsValid = false;
        CouponAccess.EditCoupon(coupon);
    }
    public static void CreateCoupon(int userId, double credit)
    {
        if (credit < 1) credit = 1;
        if (credit > 500) credit = 500;
        CouponAccess.AddCoupon(userId, credit);
    }
    public static List<Coupon> GetAllCoupons(int userId) => CouponAccess.GetAllCouponsByUserId(userId);
    public static void ApplyCouponToCartProduct(Coupon coupon)
    {
        Order.SelectedCouponId = coupon.Id;
        Order.CouponCredit = Math.Round(coupon.Credit, 2);
    }
    public static void ResetCouponSelection()
    {
        Order.SelectedCouponId = null;
        Order.CouponCredit = 0;
    }
}
