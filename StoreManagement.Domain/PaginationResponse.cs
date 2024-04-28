using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Domain
{
    public class PaginationResponse<T>
    {
        public int Length { get; set; }
        public List<T> Collection { get; set; }
    }
}
