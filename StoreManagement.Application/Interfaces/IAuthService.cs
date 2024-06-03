using StoreManagement.Bases;
using StoreManagement.Domain.Login_Token;

namespace StoreManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<bool>> CreateUser(RegisterDto registerDto);
        Task<ServiceResponse<TokenDto>> Login(LoginDto credentials);
    }
}
