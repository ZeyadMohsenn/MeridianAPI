using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Bases.Enums;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos.Order;

namespace StoreManagement.Application.Interfaces;

public interface IOrderService
{
    Task<ServiceResponse<bool>> AddOrder(AddOrderDto addOrderDto);
    Task<ServiceResponse<PaginationResponse<GetOrdersDto>>> GetOrdersAsync(GetAllOrdersFilter ordersFilter);
    Task<ServiceResponse<bool>> UpdateOrderStatus(Guid id, OrderStatus orderStatus);
    Task<ServiceResponse<GetOrderDto>> GetOrder(Guid id);
    Task<ServiceResponse<bool>> Pay(Guid id, PayDto pay);

}
