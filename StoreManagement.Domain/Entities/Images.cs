using StoreManagement.Bases;

namespace StoreManagement.Domain.Entities
{
    public class Image : BaseEntity<Guid>
    {
        public string? StoredFileName { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }


    }
}
