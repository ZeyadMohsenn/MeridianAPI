using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Domain.Login_Token;

namespace StoreManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CashierController : ApiControllersBase
    {

        private readonly ICashierService _cashierService;

        public CashierController(ICashierService cashierService)
        {
            _cashierService = cashierService;
        }


        [HttpPost]
        [Route("Cashier/login")]
        public async Task<ActionResult<TokenDto>> Login(LoginDto credentials)
        {
            var response = await _cashierService.Login(credentials);
            if (!response.Success)
            {
                if (response.Message == "User not found")
                    return NotFound(response.Message);

                if (response.Message == "Invalid password" || response.Message == "You are not a Cashier")
                    return Unauthorized(response.Message);
            }

            return Ok(response.Data);
        }


        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var serviceResponse = await _cashierService.Register(registerDto);

            if (!serviceResponse.Success)
                return BadRequest(serviceResponse.Message);

            return Ok();
        }
    }
}
