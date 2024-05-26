using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Dtos
{
    public class AddProductDto
    {
        [Required]
        [MinLength(3), MaxLength(25)]
        public required string Name { get; set; } = string.Empty;
        [StringLength(250)]
        public string? Description { get; set; }
        public required Guid SubCategory_Id { get; set; }
      
        [Range(0, int.MaxValue)]
        public decimal Price { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;
    }
}
