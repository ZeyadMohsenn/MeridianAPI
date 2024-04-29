using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Domain.Dtos
{
    public class AddSubCategoryDto
    {
        public required string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public required Guid CategoryId { get; set;}
    }
}
