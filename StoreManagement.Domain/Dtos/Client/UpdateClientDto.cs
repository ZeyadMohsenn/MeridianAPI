using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Dtos.Client
{
    public class UpdateClientDto
    {
        [MinLength(3), MaxLength(25)]
        public string? Name { get; set; } = string.Empty;

        [EmailAddress]
        public string? EmailAddress { get; set; }

        public string? Address { get; set; }

        public List<PhonesDto>? Phones { get; set; }
    }
}
