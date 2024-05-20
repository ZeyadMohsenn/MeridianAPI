namespace StoreManagement.Domain.Dtos.Order
{
    public class GetOrdersDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public decimal PriceBeforeDiscount { get; set; }
        public decimal Discount { get; set; }
        public decimal PriceBeforeTax { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal NetPrice { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainedAmount { get; set; }
        public int NumberOfPieces { get; set; }
    }

}
