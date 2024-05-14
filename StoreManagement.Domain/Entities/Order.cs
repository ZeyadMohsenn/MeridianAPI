using StoreManagement.Bases;
using StoreManagement.Bases.Enums;

namespace StoreManagement.Domain.Entities
{
    public class Order : BaseEntity<Guid>
    {
        public DateTime DateTime { get; set; }
        public OrderStatus Status { get; set; }
        public decimal PriceBeforeDiscount { get; set; }
        public decimal Discount { get; set; }
        public decimal  PriceBeforeTax { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PaidAmount { get; set; } = 0;
        public decimal RemainedAmount { get; set; } 
        //public Guid UserId { get; set; }
        //public ApplicationUser User { get; set; }
        public ICollection<OrderProduct> OrderProducts { get; set; } = new HashSet<OrderProduct>();
    }

}
