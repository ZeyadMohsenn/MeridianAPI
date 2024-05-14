using StoreManagement.Bases.Enums;
using System;

namespace StoreManagement.Bases.Domain.Model
{
    public class GetAllOrdersFilter : PagingModel
    {
        public OrderStatus? Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

    }
}
