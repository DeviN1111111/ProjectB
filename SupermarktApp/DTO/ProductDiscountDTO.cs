public class ProductDiscountDTO
{
    public ProductModel? Product { get; set; }
    public DiscountsModel? Discount { get; set; }

    public ProductDiscountDTO(ProductModel product, DiscountsModel discount)
    {
        Product = product;
        Discount = discount;
    }
}