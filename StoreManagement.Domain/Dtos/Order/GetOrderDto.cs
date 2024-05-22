namespace StoreManagement.Domain.Dtos.Order
{
    public class GetOrderDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string Client_Name { get; set; }
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
        public ICollection<ProductDto> Products { get; set; } = new List<ProductDto>();

    }
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? PieceDiscountAmount { get; set; }
    }
}
