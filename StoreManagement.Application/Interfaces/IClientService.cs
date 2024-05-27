using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos.Client;

namespace StoreManagement.Application.Interfaces
{
    public interface IClientService
    {
        Task<ServiceResponse<bool>> AddClient(AddClientDto addClientDto);
        Task<ServiceResponse<GetClientDto>> GetClient(Guid id);
        Task<ServiceResponse<PaginationResponse<GetClientsDto>>> GetClients(GetAllClientsFilter clientsFilter);
        Task<ServiceResponse<bool>> UpdateClient(UpdateClientDto clientDto, Guid id);
        Task<ServiceResponse<bool>> DeleteClient(Guid id);



    }
}
