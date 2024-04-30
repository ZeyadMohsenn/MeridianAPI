using System;

namespace StoreManagement.Bases.Domain.Model
{
    public class GetAllProductsFilter : PagingModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public bool Is_Deleted { get; set; }
    }
}
