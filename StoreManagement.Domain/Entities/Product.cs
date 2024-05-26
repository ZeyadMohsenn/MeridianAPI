using StoreManagement.Bases;

namespace StoreManagement.Domain.Entities
{
    public class Product : BaseEntity<Guid>
    {

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Photo { get; set; }
        public decimal? Price { get; set; }
        //public decimal Discount { get; set; }
        public int? StockQuantity { get; set; }
        public bool isActive { get; set; } = true;
        public Guid SubCategory_Id { get; set; }
        public SubCategory SubCategory { get; set; }
        public ICollection<OrderProduct> OrderProducts { get; set; } = new HashSet<OrderProduct>();

    }
}
