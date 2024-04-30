namespace StoreManagement.Domain.Dtos
{
    public class AddProductDto
    {
        public required string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public required Guid SubCategory_Id { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; } = 0;
    }
}
