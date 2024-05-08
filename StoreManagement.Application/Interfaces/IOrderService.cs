using StoreManagement.Bases;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Dtos.Order;

namespace StoreManagement.Application.Interfaces;

public interface IOrderService
{
    Task<ServiceResponse<bool>> AddOrder(AddOrderDto addOrderDto);

}
