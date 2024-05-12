using StoreManagement.Bases.Enums;
using System;

namespace StoreManagement.Bases.Domain.Model
{
    public class GetAllOrdersFilter : PagingModel
    {
        public Guid OrderId { get; set; }
        public OrderStatus? Status { get; set; }

    }
}
