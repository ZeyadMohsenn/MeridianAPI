using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Application.Interfaces;
using StoreManagement.Application.Services;
using StoreManagement.Bases;
using StoreManagement.Domain.Dtos;
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

    }
}
