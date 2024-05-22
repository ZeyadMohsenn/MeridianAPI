using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos.Client;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Entities.StoreManagement.Domain.Entities;
using System.Linq.Expressions;


namespace StoreManagement.Application.Services
{


    public class ClientService(IUnitOfWork unitOfWork, IMapper mapper) : ServiceBase(), IClientService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IBaseRepository<Client> _clientRepo = unitOfWork.GetRepository<Client>();
        private readonly IBaseRepository<Phone> _phoneRepo = unitOfWork.GetRepository<Phone>();


        public async Task<ServiceResponse<bool>> AddClient(AddClientDto addClientDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(addClientDto.Name))
                    return new ServiceResponse<bool>() { Success = false, Message = "Empty Name" };

                addClientDto.Name = addClientDto.Name.Trim();


                foreach (var phoneDto in addClientDto.Phones)
                {
                    var existingPhone = await _phoneRepo.FindAsync(p => p.Number == phoneDto.Phone);

                    if (existingPhone != null)
                        return new ServiceResponse<bool>() { Success = false, Message = $"Phone number {phoneDto.Phone} already exists." };
                }

                var existingMail = await _clientRepo.FindAsync(m => m.Email == addClientDto.EmailAddress);
                if (existingMail != null)
                    return new ServiceResponse<bool>() { Success = false, Message = $"Email is Already exists." };

                var client = new Client
                {
                    Name = addClientDto.Name,
                    Email = addClientDto.EmailAddress,
                    Address = addClientDto.Address,
                    Phones = addClientDto.Phones.Select(p => new Phone { Number = p.Phone }).ToList()
                };

                await _clientRepo.AddAsync(client);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>() { Success = true, Data = true };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>() { Success = false, Message = ex.Message };
            }
        }


        public async Task<ServiceResponse<GetClientDto>> GetClient(Guid id)
        {
            try
            {
                Expression<Func<Client, bool>> filterPredicate = p => p.Id == id;

                Client client = await _clientRepo.FindAsync(filterPredicate, Include: q => q.Include(c => c.Orders).Include(c => c.Phones), asNoTracking: true);

                if (client == null)
                    return new ServiceResponse<GetClientDto>() { Success = false, Message = "Client Not Found" };

                var getClientDto = new GetClientDto
                {
                    Name = client.Name,
                    EmailAddress = client.Email,
                    Address = client.Address,
                    Phones = client.Phones.Select(p => new PhonesDto
                    {
                        Phone = p.Number
                    }).ToList(),

                    Orders = client.Orders.Select(p => new OrderDto
                    {
                        OrderId = p.Id,
                        DateTime = p.DateTime,
                        Status = p.Status.ToString(),
                        NetPrice = p.TotalPrice,
                        PaidAmount = p.PaidAmount,
                        RemainedAmount = p.RemainedAmount,

                    }).ToList(),
                    TotalOrders = client.Orders.Count,
                    TotalNetPrice = client.Orders.Sum(o => o.TotalPrice),
                    TotalPaidAmount = client.Orders.Sum(o => o.PaidAmount),
                    TotalRemainedAmount = client.Orders.Sum(o => o.RemainedAmount)

                };
                return new ServiceResponse<GetClientDto>() { Data = getClientDto, Success = true };
            }
            catch
            {
                return new ServiceResponse<GetClientDto>() { Success = false, Message = "An error occurred while processing your request." };
            }
        }

        public async Task<ServiceResponse<PaginationResponse<GetClientsDto>>> GetClients(GetAllClientsFilter clientsFilter)
        {
            try
            {
                IQueryable<Client> query = _clientRepo.GetAllQueryableAsync().Include(a => a.Phones);

                if (clientsFilter.Name != null)
                    query = query.Where(c => c.Name == clientsFilter.Name);

                if (clientsFilter.Phone != null)
                    query = query.Where(c => c.Phones.Any(p => p.Number == clientsFilter.Phone));


                var count = await query.CountAsync();

                var clients = await query.Skip(clientsFilter.PageNumber - 1)
                                        .Take(clientsFilter.PageSize)
                                        .Select(o => new GetClientsDto
                                        {
                                            Id = o.Id,
                                            Name = o.Name,
                                            Address = o.Address,
                                            EmailAddress = o.Email,
                                            Phones = o.Phones.Select(p => new PhonesDto
                                            {
                                                Phone = p.Number
                                            }).ToList()
                                        }).ToListAsync();

                var paginationResponse = new PaginationResponse<GetClientsDto>
                {
                    Length = count,
                    Collection = clients
                };
                return new ServiceResponse<PaginationResponse<GetClientsDto>>
                {
                    Data = paginationResponse,
                    Message = "Clients retrieved successfully",
                    Success = true
                };
            }
            catch
            {
                return new ServiceResponse<PaginationResponse<GetClientsDto>> { Data = null, Success = false, Message = "An error occurred while retrieving Orders: " };
            }
        }
        public async Task<ServiceResponse<bool>> UpdateClient(UpdateClientDto clientDto, Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return new ServiceResponse<bool> { Success = false, Message = "Please enter a valid ID." };

                Expression<Func<Client, bool>> filterPredicate = p => p.Id == id;

                Client dbClient = await _clientRepo.FindAsync(filterPredicate, Include: q => q.Include(c => c.Phones));


                if (dbClient == null)
                    return new ServiceResponse<bool> { Success = false, Message = "No client found with this ID." };

                if (!string.IsNullOrEmpty(clientDto.Name))
                    dbClient.Name = clientDto.Name.Trim();

                if (!string.IsNullOrEmpty(clientDto.Address))
                    dbClient.Address = clientDto.Address.Trim();

                if (!string.IsNullOrEmpty(clientDto.EmailAddress))
                    dbClient.Email = clientDto.EmailAddress;

                var newPhoneNumbers = clientDto.Phones.Select(p => p.Phone).ToList();

                foreach (var phone in newPhoneNumbers)
                {
                    var existingPhone = await _phoneRepo.FindAsync(p => p.Number == phone && p.ClientId != id);
                    if (existingPhone != null)
                        return new ServiceResponse<bool> { Success = false, Message = $"Phone number {phone} already exists." };
                }

                var existingPhones = dbClient.Phones.ToList();
                foreach (var phone in existingPhones)
                {
                    dbClient.Phones.Remove(phone);
                    _phoneRepo.Remove(phone); 
                }

                foreach (var phoneDto in clientDto.Phones)
                {
                    dbClient.Phones.Add(new Phone
                    {
                        Number = phoneDto.Phone,
                        ClientId = dbClient.Id
                    });
                }
                _clientRepo.Update(dbClient);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool> { Success = true, Message = "Client updated successfully." };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> { Success = false, Message = $"An error occurred while updating the client: {ex.Message}" };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteClient(Guid id)
        {      
            try
            {
                if (id == Guid.Empty)
                    return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

                Client client = _clientRepo.FindByID(id);

                if (client == null)
                    return new ServiceResponse<bool> { Success = false, Message = "Client not found" };

                _clientRepo.Delete(client);

                var affectedRows = await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool> { Success = affectedRows > 0, Message = "Client deleted successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> { Success = false, Message = "An error occurred while deleting the Client" };
            }
        }


    }
}

