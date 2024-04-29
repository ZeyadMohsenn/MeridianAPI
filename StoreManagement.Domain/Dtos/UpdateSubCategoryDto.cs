namespace StoreManagement.Domain.Dtos
{
    public class UpdateSubCategoryDto
    {
        public required string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
