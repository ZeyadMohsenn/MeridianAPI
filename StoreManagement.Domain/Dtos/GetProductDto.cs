namespace StoreManagement.Domain.Dtos
{
    public class GetProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public Guid SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public List<GetImagesDto> Images { get; set; } = new List<GetImagesDto>();
    }

    public class GetImagesDto
    {
        public string? ImageUrl { get; set; }
    }
}
