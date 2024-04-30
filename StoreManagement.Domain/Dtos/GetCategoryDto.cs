namespace StoreManagement.Domain.Dtos
{
    public class GetCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Photo { get; set; }
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
