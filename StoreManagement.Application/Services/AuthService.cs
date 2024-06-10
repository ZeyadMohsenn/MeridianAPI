using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Domain.Const;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Login_Token;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoreManagement.Application.Services
{
    public class AuthService : ServiceBase, IAuthService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IOptionsSnapshot<SecretKey> _secretOptions;

        public AuthService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,IConfiguration configuration ,IOptionsSnapshot<SecretKey> secretOption)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _configuration = configuration;
            _secretOptions = secretOption;

        }

        public async Task<ServiceResponse<bool>> CreateUser(RegisterDto registerDto)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                if (string.IsNullOrWhiteSpace(registerDto.Name))
                {
                    response.Success = false;
                    response.Message = "Empty Name";
                    return response;
                }

                registerDto.Name = registerDto.Name.Trim();

                var user = new ApplicationUser
                {
                    Full_Name = registerDto.Name,
                    PhoneNumber = registerDto.PhoneNumber,
                    UserName = registerDto.PhoneNumber,
                    Creation_Time = DateTime.Now,
                    Is_Active = true,
                    Is_Deleted = false,
                };

                var creationResult = await _userManager.CreateAsync(user, registerDto.Password);
                if (!creationResult.Succeeded)
                {
                    response.Success = false;
                    response.Message = string.Join(", ", creationResult.Errors.Select(e => e.Description));
                    return response;
                }

                var role = registerDto.Role.ToString();

                await _userManager.AddToRoleAsync(user, role);

                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<TokenDto>> Login(LoginDto credentials)
        {
            var response = new ServiceResponse<TokenDto>();

            var user = await _userManager.FindByNameAsync(credentials.PhoneNumber);
            if (user is null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, credentials.Password);
            if (!isPasswordCorrect)
            {
                response.Success = false;
                response.Message = "Invalid password";
                return response;
            }

            var claimsList = await _userManager.GetClaimsAsync(user);
            var roleClaim = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (roleClaim == null)
            {
                response.Success = false;
                response.Message = "User role not found";
                return response;
            }

            if (roleClaim.Value != "Cashier" && roleClaim.Value != "Admin")
            {
                response.Success = false;
                response.Message = "Unauthorized role";
                return response;
            }

            string? secretKey = /*_secretOptions.Value.Key;*/ _configuration.GetValue<string>("TokenSettings:SecretKey");

            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException(nameof(secretKey), "Secret key cannot be null or empty");

            var algorithm = SecurityAlgorithms.HmacSha256Signature;
            var keyInBytes = Encoding.ASCII.GetBytes(secretKey);
            var key = new SymmetricSecurityKey(keyInBytes);
            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                claims: claimsList,
                signingCredentials: signingCredentials,
                expires: DateTime.Now.AddHours(9));

            var tokenHandler = new JwtSecurityTokenHandler();

            response.Data = new TokenDto
            {
                Token = tokenHandler.WriteToken(token)
            };
            response.Success = true;
            response.Message = $"You're Logged in with {credentials.PhoneNumber}";

            return response;
        }

    }
}
