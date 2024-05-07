using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Dtos
{
    public class DropDownCategoriesDto
    {
        [Required]
        [MinLength(3), MaxLength(25)]
        public required string Name { get; set; } = string.Empty;
        public Guid Id { get; set; }
    }
}
