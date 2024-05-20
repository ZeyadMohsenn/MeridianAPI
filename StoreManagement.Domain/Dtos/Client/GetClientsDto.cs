namespace StoreManagement.Domain.Dtos.Client
{
    public class GetClientsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public List<PhonesDto> Phones { get; set; } = new List<PhonesDto>();
    }
}
