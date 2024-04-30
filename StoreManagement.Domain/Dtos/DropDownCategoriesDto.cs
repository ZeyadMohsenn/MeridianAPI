using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Domain.Dtos
{
    public class DropDownCategoriesDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid Id { get; set; }
    }
}
