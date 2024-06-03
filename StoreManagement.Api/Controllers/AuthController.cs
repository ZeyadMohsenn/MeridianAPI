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
    public class AuthController : ApiControllersBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<TokenDto>> Login(LoginDto credentials)
        {
            var response = await _authService.Login(credentials);
            if (!response.Success)
            {
                if (response.Message == "User not found")
                    return NotFound(response.Message);

                if (response.Message == "Invalid password")
                    return Unauthorized(response.Message);
            }

            return Ok(response.Data);
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var serviceResponse = await _authService.CreateUser(registerDto);

            if (!serviceResponse.Success)
                return BadRequest(serviceResponse.Message);

            return Ok();
        }
    }
}
