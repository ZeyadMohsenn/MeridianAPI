using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Dtos;

public class UpdateCategoryDto
{
    [Required]
    [MinLength(3), MaxLength(25)]
    public required string Name { get; set; } = string.Empty;
    [StringLength(250)]
    public string? Description { get; set; }
}
