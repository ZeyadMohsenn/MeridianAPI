using StoreManagement.Bases;

namespace StoreManagement.Domain.Entities
{
    public class Client : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public ICollection<Phone> Phones { get; set; } = new List<Phone>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
