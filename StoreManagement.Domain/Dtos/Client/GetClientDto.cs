namespace StoreManagement.Domain.Dtos.Client
{
    public class GetClientDto
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public List<PhonesDto> Phones { get; set; } = new List<PhonesDto>();
        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();

        public int TotalOrders { get; set; }
        public decimal TotalNetPrice { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalRemainedAmount { get; set; }
    }

    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public decimal NetPrice { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainedAmount { get; set; }

    }
}
