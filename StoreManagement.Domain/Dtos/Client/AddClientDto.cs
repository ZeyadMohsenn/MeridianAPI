using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Dtos.Client
{
    public class AddClientDto
    {
        [Required]
        [MinLength(3), MaxLength(25)]
        public required string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public List<PhonesDto> Phones { get; set; } = new List<PhonesDto>();

    }

    public class PhonesDto
    {
        public string? Phone { get; set; }

    }
}
