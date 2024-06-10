using StoreManagement.Bases.Enums;

namespace StoreManagement.Domain.Dtos.Reports
{
    public class OrderFilter
    {
        public OrderStatus? Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
