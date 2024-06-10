using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain.Dtos.Reports;

namespace StoreManagement.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ReportController : ApiControllersBase
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("Users")]
        public async Task<IActionResult> UsersReport()
        {
            return Ok(await _reportService.UsersReport());    
        }

        [HttpGet("Product")]
        public async Task<IActionResult> ProductReport()
        {
            return Ok(await _reportService.ProductsReport());
        }

        [HttpGet("Order")]
        public async Task<IActionResult> OrderReport([FromQuery] OrderFilter ordersFilter)
        {
            return Ok(await _reportService.OrdersReport(ordersFilter));
        }
    }
}
