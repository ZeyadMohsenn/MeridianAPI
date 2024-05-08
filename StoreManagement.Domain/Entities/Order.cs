using StoreManagement.Bases;

namespace StoreManagement.Domain.Entities
{
    public class Order : BaseEntity<Guid>
    {
        public DateTime DateTime { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        //public Guid UserId { get; set; }
        //public ApplicationUser User { get; set; }
        public ICollection<OrderProduct> OrderProducts { get; set; } = new HashSet<OrderProduct>();
    }
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

}
