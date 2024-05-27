using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Login_Token;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoreManagement.Application.Services
{
    public class CashierService : ServiceBase, ICashierService
    {
     
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IBaseRepository<Cashier> _cashierRepo;

        public CashierService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _cashierRepo = _unitOfWork.GetRepository<Cashier>();
        }

        public async Task<ServiceResponse<bool>> Register(RegisterDto addCashierDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(addCashierDto.Name))
                    return new ServiceResponse<bool> { Success = false, Message = "Empty Name" };

                addCashierDto.Name = addCashierDto.Name.Trim();

                var user = new ApplicationUser
                {
                    Full_Name = addCashierDto.Name,
                    PhoneNumber = addCashierDto.PhoneNumber,
                    UserName = addCashierDto.PhoneNumber,
                    Creation_Time = DateTime.Now,
                    Is_Active = true,
                    Is_Deleted = false,
                };

                var creationResult = await _userManager.CreateAsync(user, addCashierDto.Password);
                if (!creationResult.Succeeded)
                    return new ServiceResponse<bool> { Success = false, Message = string.Join(", ", creationResult.Errors.Select(e => e.Description)) };

                var claimsList = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, "Cashier"),
                };

                await _userManager.AddClaimsAsync(user, claimsList);

                var cashier = _mapper.Map<Cashier>(addCashierDto);
                cashier.User = user;

                await _cashierRepo.AddAsync(cashier);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool> { Success = true, Data = true };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> { Success = false, Message = ex.Message };
            }
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

            if (roleClaim?.Value != "Cashier")
            {
                response.Success = false;
                response.Message = "You are not a Cashier";
                return response;
            }

            string? secretKey = _configuration.GetValue<string>("TokenSettings:SecretKey");
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException(nameof(secretKey), "Secret key cannot be null or empty");
            }

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

            return response;
        }


    }
}
