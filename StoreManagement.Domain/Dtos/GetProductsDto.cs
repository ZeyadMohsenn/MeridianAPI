namespace StoreManagement.Domain.Dtos;

public class GetProductsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal? Price { get; set; }
    public string Photo { get; set; }
    public int? StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public Guid SubCategoryId { get; set; }
    public string SubCategoryName { get; set; }
}
