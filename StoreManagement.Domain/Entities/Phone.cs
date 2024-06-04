using StoreManagement.Bases;


namespace StoreManagement.Domain.Entities
{
    public class Phone : BaseEntity<Guid>
    {
        public string Number { get; set; }
        public Guid ClientId { get; set; }
        public Client Client { get; set; }
    }
}

