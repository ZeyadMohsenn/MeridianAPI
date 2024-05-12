using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Dtos.Order
{
    public class AddOrderDto
    {
        //[Required]
        //public Guid UserId { get; set; }
        [Required]
        public List<OrderProductDto> OrderProducts { get; set; }
        public decimal Discount { get; set; }
        public DiscountEnum DiscountType { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal PaidAmount { get; set; } = 0;    

        // discount w tax w df3 ad eh
    }
    public class OrderProductDto
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public int Quantity { get; set; } 
    }
    public enum DiscountEnum
    {
        Percentage,
        Fixed
    }
}
