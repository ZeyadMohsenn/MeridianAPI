namespace StoreManagement.Domain.Dtos.Reports
{
    public class ProductStatsDto
    {
        public int ProductCount { get; set; }
        public List<ProductQuantityDto> Products { get; set; } = new List<ProductQuantityDto>();
    }

    public class ProductQuantityDto
    {
        public string? ProductName { get; set; }
        public int? QuantityInStock { get; set; }
    }
}
