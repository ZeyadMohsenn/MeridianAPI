using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Bases.Enums;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos.Order;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Application.Services
{
    public class OrderService(IUnitOfWork unitOfWork, IMapper mapper) : ServiceBase(), IOrderService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IBaseRepository<Order> _orderRepo = unitOfWork.GetRepository<Order>();
        private readonly IBaseRepository<Product> _productRepo = unitOfWork.GetRepository<Product>();
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IBaseRepository<OrderProduct> _orderProductRepo = unitOfWork.GetRepository<OrderProduct>();


        public async Task<ServiceResponse<bool>> AddOrder(AddOrderDto addOrderDto)
        {
            try
            {
                if (addOrderDto.OrderProducts.Count == 0)
                    return new ServiceResponse<bool>()
                    {
                        Success = false,
                        Message = $"Please insert Products to your order",
                        Data = false
                    };

                decimal totalOrderPrice = 0;

                foreach (var orderProduct in addOrderDto.OrderProducts)
                {
                    var productDb = await _productRepo.FindByIdAsync(orderProduct.ProductId);

                    if (productDb == null)
                        return new ServiceResponse<bool>() { Success = false, Message = $"Product not found for ID: {orderProduct.ProductId}" };

                    if (productDb.StockQuantity < orderProduct.Quantity)
                        return new ServiceResponse<bool>() { Success = false, Message = $"Insufficient stock for product ID: {orderProduct.ProductId}" };

                    decimal totalPriceForProduct = (productDb.Price ?? 0) * orderProduct.Quantity;
                    totalOrderPrice += totalPriceForProduct;
                    productDb.StockQuantity -= orderProduct.Quantity;
                    _productRepo.Update(productDb);
                }

                decimal remained = 0;
                switch (addOrderDto.DiscountType)
                {
                    case DiscountEnum.Percentage:
                        totalOrderPrice -= addOrderDto.Discount * totalOrderPrice;
                        totalOrderPrice += addOrderDto.TaxPercentage * totalOrderPrice;
                        addOrderDto.Discount = addOrderDto.Discount * 100;
                        break;
                    case DiscountEnum.Fixed:
                        totalOrderPrice -= addOrderDto.Discount;
                        totalOrderPrice += addOrderDto.TaxPercentage * totalOrderPrice;
                        break;
                }
                remained = totalOrderPrice - addOrderDto.PaidAmount;


                if (remained < 0)
                {
                    return new ServiceResponse<bool>()
                    {
                        Success = false,
                        Message = $"Recheck the Paid Amount because it's more than the total order price",
                        Data = false
                    };
                }

                var order = new Order
                {
                    //UserId = addOrderDto.UserId,   
                    Status = OrderStatus.Processing,
                    OrderProducts = addOrderDto.OrderProducts.Select(op => new OrderProduct
                    {
                        ProductId = op.ProductId,
                        Quantity = op.Quantity
                    }).ToList(),
                    TotalPrice = totalOrderPrice,
                    PaidAmount = addOrderDto.PaidAmount,
                    RemainedAmount = remained,
                    TaxPercentage = addOrderDto.TaxPercentage,
                    Discount = addOrderDto.Discount,
                    DateTime = DateTime.UtcNow,
                };
                await _orderRepo.AddAsync(order);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>()
                {
                    Success = true,
                    Message = "Order Created Successfully",
                    Data = true,
                };

            }
            catch (Exception)
            {
                return new ServiceResponse<bool>()
                {
                    Success = false,
                    Message = $"An error occurred while adding the order",
                    Data = false
                };
            }
        }

        public async Task<ServiceResponse<PaginationResponse<GetOrdersDto>>> GetOrdersAsync(GetAllOrdersFilter ordersFilter)
        {
            try
            {
                IQueryable<Order> query = _orderRepo.GetAllQueryableAsync();

                if (ordersFilter.OrderId != Guid.Empty)
                    query = query.Where(o => o.Id == ordersFilter.OrderId);

                if (ordersFilter.Status != null)
                    query = query.Where(o => o.Status == ordersFilter.Status);

                var count = await query.CountAsync();

                var orders = await query.Skip(ordersFilter.PageNumber - 1)
                                        .Take(ordersFilter.PageSize)
                                        .Select(o => new GetOrdersDto
                                        {
                                            Id = o.Id,
                                            Status = o.Status.ToString(),
                                            Discount = o.Discount,
                                            TaxPercentage = o.TaxPercentage,
                                            NetPrice = o.TotalPrice,
                                            PaidAmount = o.PaidAmount,
                                            RemainedAmount = o.RemainedAmount,
                                            NumberOfPieces = o.OrderProducts.Sum(op => op.Quantity)
                                        }).ToListAsync();

                var paginationResponse = new PaginationResponse<GetOrdersDto>
                {
                    Length = count,
                    Collection = orders
                };
                return new ServiceResponse<PaginationResponse<GetOrdersDto>>
                {
                    Data = paginationResponse,
                    Message = "Orders retrieved successfully",
                    Success = true
                };

            }
            catch (Exception)
            {
                return new ServiceResponse<PaginationResponse<GetOrdersDto>> { Data = null, Success = false, Message = "An error occurred while retrieving Orders: " };
            }
        }

        public async Task<ServiceResponse<bool>> UpdateOrderStatus(Guid id, OrderStatus orderStatus)
        {

            try
            {

                if (id == Guid.Empty)
                    return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

                Order? order = _orderRepo.FindByID(id);

                if (order == null)
                    return new ServiceResponse<bool> { Success = false, Message = "Order not found" };

                order.Status = orderStatus;
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool> { Success = true };

            }
            catch (Exception)
            {
                return new ServiceResponse<bool> { Success = false, Message = "An error occurred " };

            }
        }
    }
}
