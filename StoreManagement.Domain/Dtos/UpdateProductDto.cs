namespace StoreManagement.Domain.Dtos
{
    public class UpdateProductDto
    {
        public required string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
