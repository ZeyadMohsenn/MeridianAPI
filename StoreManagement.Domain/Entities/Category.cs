using StoreManagement.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Domain.Entities
{
    public class Category : BaseEntity<Guid>
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }  = string.Empty;
        public string? Description { get; set; }
        public string? Photo { get; set; }
    }
}
