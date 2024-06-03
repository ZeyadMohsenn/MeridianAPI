using StoreManagement.Bases;


namespace StoreManagement.Domain.Entities
{
    public class SubCategory : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? StoredFileName { get; set; }
        public Guid Category_Id { get; set; }
        public Category Category { get; set; }
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();


    }
}
