using StoreManagement.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Domain.Entities
{
    public class SubCategory : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Photo { get; set; }
        public Guid Category_Id { get; set; }
        public Category Category { get; set; }

    }
}
