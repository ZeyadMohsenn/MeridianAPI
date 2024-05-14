using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Dtos.Order
{
    public class PayDto
    {
        [Required]
        public decimal PaymentAmount { get; set; }
    }
}
