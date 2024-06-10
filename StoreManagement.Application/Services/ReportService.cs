using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Domain.Dtos.Reports;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Services
{
    public class ReportService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : ServiceBase(), IReportService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseRepository<Client> _clientRepo = unitOfWork.GetRepository<Client>();
        private readonly IBaseRepository<Product> _productRepo = unitOfWork.GetRepository<Product>();
        private readonly IBaseRepository<Order> _orderRepo = unitOfWork.GetRepository<Order>();
        private readonly UserManager<ApplicationUser> _userManager = userManager;


        public async Task<ServiceResponse<UsersStatsDto>> UsersReport()
        {
            try
            {
                UsersStatsDto usersStatsDto = new UsersStatsDto();
                var clientQuery = _clientRepo.GetAllQueryableAsync();
                usersStatsDto.ClientsNumber = clientQuery.Count();

                var users = _userManager.Users.ToList();
                usersStatsDto.StaffNumber = users.Count;

                int adminsCount = 0;
                int cashiersCount = 0;

                foreach (ApplicationUser? user in users)
                {
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        adminsCount++;
                    }
                    if (await _userManager.IsInRoleAsync(user, "Cashier"))
                    {
                        cashiersCount++;
                    }
                }
                usersStatsDto.AdminsNumber = adminsCount;
                usersStatsDto.CashiersNumber = cashiersCount;
                usersStatsDto.AllUsersNumber = usersStatsDto.ClientsNumber + usersStatsDto.StaffNumber;

                return new ServiceResponse<UsersStatsDto> { Success = true, Data = usersStatsDto };

            }
            catch
            {

                return new ServiceResponse<UsersStatsDto> { Success = false, Message = "An Error happened while getting the report" };

            }
        }

        public async Task<ServiceResponse<ProductStatsDto>> ProductsReport()
        {
            try
            {
                ProductStatsDto productStatsDto = new ProductStatsDto();

                IQueryable<Product> query = _productRepo.GetAllQueryableAsync();
                var count = await query.CountAsync();

                productStatsDto.ProductCount = count;

                var products = await query
              .Select(p => new ProductQuantityDto
              {
                  ProductName = p.Name,
                  QuantityInStock = p.StockQuantity
              })
              .ToListAsync();

                productStatsDto.Products = products;

                return new ServiceResponse<ProductStatsDto>
                {
                    Success = true,
                    Data = productStatsDto,
                };
            }
            catch
            {
                return new ServiceResponse<ProductStatsDto> { Success = false, Message = "An Error happened while getting the report" };

            }
        }

        public async Task<ServiceResponse<OrderStatsDto>> OrdersReport(OrderFilter ordersFilter)
        {
            try
            {
                OrderStatsDto orderStatsDto = new OrderStatsDto();

                IQueryable<Order> query = _orderRepo.GetAllQueryableAsync();

                if (ordersFilter.Status != null)
                    query = query.Where(o => o.Status == ordersFilter.Status);


                if (ordersFilter != null)
                {
                    if (ordersFilter.From != null)
                    {
                        query = query.Where(o => o.DateTime >= ordersFilter.From);
                    }
                    if (ordersFilter.To != null)
                    {
                        query = query.Where(o => o.DateTime <= ordersFilter.To);
                    }
                }

                var count = await query.CountAsync();
                orderStatsDto.OrdersCount = count;

                var totalDiscount = await query.SumAsync(o => o.Discount);
                var totalPaid = await query.SumAsync(o => o.PaidAmount);
                var totalRemained = await query.SumAsync(o => o.RemainedAmount);

                orderStatsDto.TotalDiscount = totalDiscount;
                orderStatsDto.TotalPaid = totalPaid;
                orderStatsDto.TotalRemained = totalRemained;

                var ClientsCount = await query.Select(o => o.Client_Id).Distinct().CountAsync();
                orderStatsDto.ClientsCount = ClientsCount;


                var ordersByDate = await query
                    .Include(o => o.Client)
                    .GroupBy(o => o.DateTime.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Orders = g.Select(o => new OrderDto
                        {
                            Status = o.Status.ToString(),
                            OrderId = o.Id,
                            ClientName = o.Client.Name,
                            CraetedAt = o.DateTime,
                            TotalPrice = o.TotalPrice,
                            TotalDiscount = o.Discount,
                            Paid = o.PaidAmount,
                            Remained = o.RemainedAmount
                        }).ToList()
                    })
                    .ToListAsync();

                orderStatsDto.DailyOrders = ordersByDate.Select(g => new DailyOrderDto
                {
                    Date = g.Date,
                    Orders = g.Orders
                }).ToList();

                return new ServiceResponse<OrderStatsDto> { Success = true, Data = orderStatsDto };
            }

            catch
            {
                return new ServiceResponse<OrderStatsDto> { Success = false, Message = "An Error happened while getting the report" };

            }
        }
    }
}
