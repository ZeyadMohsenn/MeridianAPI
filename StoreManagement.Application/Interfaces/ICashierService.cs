using StoreManagement.Bases;
using StoreManagement.Domain.Login_Token;

namespace StoreManagement.Application.Interfaces
{
    public interface ICashierService
    {
        Task<ServiceResponse<bool>> Register(RegisterDto addCashierDto);
        Task<ServiceResponse<TokenDto>> Login(LoginDto credentials);

    }
}
