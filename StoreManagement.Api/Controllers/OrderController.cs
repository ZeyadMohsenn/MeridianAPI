using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Bases.Enums;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos.Order;

namespace StoreManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]

    public class OrderController : ApiControllersBase
    {
        private readonly IOrderService _orderServices;
        public OrderController(IOrderService orderService)
        {
            _orderServices = orderService;
        }
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> AddOrder([FromBody] AddOrderDto addOrderDto)
        {
            var Order = await _orderServices.AddOrder(addOrderDto);
            return Ok(Order);
        }
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            ServiceResponse<GetOrderDto> order = await _orderServices.GetOrder(id);
            return Ok(order);
        }

        [HttpGet("GetAll"), AllowAnonymous]
        public async Task<IActionResult> GetOrders([FromQuery] GetAllOrdersFilter ordersFitler)
        {
            ServiceResponse<PaginationResponse<GetOrdersDto>> response = await _orderServices.GetOrdersAsync(ordersFitler);
            if (response.Success)
                return Ok(response);
           
            else
                return BadRequest(response);
        }

        [HttpPut("Pay"), AllowAnonymous]
        public async Task<IActionResult> Pay(Guid id, PayDto pay)
        {
            ServiceResponse<bool> order = await _orderServices.Pay(id, pay);
            return Ok(order);   
        }



        [HttpPut("UpdateOrderStatus"), AllowAnonymous]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, OrderStatus status)
        {
            ServiceResponse<bool> order = await _orderServices.UpdateOrderStatus(id, status);

            if (order.Success)
                return Ok(order);

            return BadRequest(order);
        }


    }
}
