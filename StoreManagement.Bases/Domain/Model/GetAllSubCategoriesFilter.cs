using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Bases.Domain.Model
{
    public class GetAllSubCategoriesFilter : PagingModel
    {
        public Guid SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }    
        public bool Is_Deleted { get; set; }
        public Guid CategoryId { get; set; }
    }
}
