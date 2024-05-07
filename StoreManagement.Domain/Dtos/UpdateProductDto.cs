using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Dtos
{
    public class UpdateProductDto
    {
        [Required]
        [MinLength(3), MaxLength(25)]
        public required string Name { get; set; } = string.Empty;
        [StringLength(250)]
        public string? Description { get; set; }
        public Guid SubCategoryId { get; set; }
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        [Range(0, int.MaxValue)]
        public decimal Price { get; set; } = 0;
        
    }
}
