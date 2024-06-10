using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Domain.Dtos.Reports;

namespace StoreManagement.Application.Interfaces
{
    public interface IReportService
    {
        Task<ServiceResponse<UsersStatsDto>> UsersReport();
        Task<ServiceResponse<ProductStatsDto>> ProductsReport();

        Task<ServiceResponse<OrderStatsDto>> OrdersReport(OrderFilter ordersFilter);
    }
}
