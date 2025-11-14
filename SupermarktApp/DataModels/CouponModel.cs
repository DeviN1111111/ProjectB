class Coupon
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public double Credit { get; set; }
    public bool IsValid { get; set; }
    public int Code { get; set; }
    
    public Coupon() {}
}