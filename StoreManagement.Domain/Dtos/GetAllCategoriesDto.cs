using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Dtos
{
    public class GetAllCategoriesDto
    {
        [Required]
        [MinLength(3), MaxLength(25)]
        public required string Name { get; set; } = string.Empty;
        [StringLength(250)]
        public string? Description { get; set; }
        public string? Photo { get; set; }
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public string? ImageUrl { get; set; }
    }
}
