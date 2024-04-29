using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Bases.Domain.Model
{
    public class GetAllSubCategoriesFilter : PagingModel
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }    
        public bool Is_Deleted { get; set; }
    }
}
