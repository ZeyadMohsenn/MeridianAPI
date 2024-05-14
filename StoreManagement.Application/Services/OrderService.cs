using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Bases.Domain.Model;
using StoreManagement.Bases.Enums;
using StoreManagement.Domain;
using StoreManagement.Domain.Dtos.Order;
using StoreManagement.Domain.Entities;
using System.Linq.Expressions;

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

                decimal priceBeforeDiscount = totalOrderPrice;
                decimal remained = 0;
                decimal priceBeforeTax = 0;

                switch (addOrderDto.DiscountType)
                {
                    case DiscountEnum.Percentage:
                        totalOrderPrice -= addOrderDto.Discount * totalOrderPrice;
                        priceBeforeTax = totalOrderPrice;
                        totalOrderPrice += addOrderDto.TaxPercentage * totalOrderPrice;
                        addOrderDto.Discount = addOrderDto.Discount * 100;
                        break;
                    case DiscountEnum.Fixed:
                        totalOrderPrice -= addOrderDto.Discount;
                        priceBeforeTax = totalOrderPrice;
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

                var orderStatus = OrderStatus.Processing;
                if (remained == 0)
                    orderStatus = OrderStatus.Finished;

                var order = new Order
                {
                    Status = orderStatus,
                    OrderProducts = addOrderDto.OrderProducts.Select(op => new OrderProduct
                    {
                        ProductId = op.ProductId,
                        Quantity = op.Quantity
                    }).ToList(),
                    PriceBeforeDiscount = priceBeforeDiscount,
                    Discount = addOrderDto.Discount,
                    PriceBeforeTax = priceBeforeTax,
                    TaxPercentage = addOrderDto.TaxPercentage,
                    TotalPrice = totalOrderPrice,
                    PaidAmount = addOrderDto.PaidAmount,
                    RemainedAmount = remained,
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

        public async Task<ServiceResponse<GetOrderDto>> GetOrder(Guid id)
        {
            try
            {
                Expression<Func<Order, bool>> filterPredicate = p => p.Id == id;

                Order order = await _orderRepo.FindAsync(filterPredicate, Include: q => q.Include(o => o.OrderProducts).ThenInclude(p => p.Product), asNoTracking: true);

                if (order == null)
                    return new ServiceResponse<GetOrderDto>() { Success = false, Message = "Order Not Found" };

                var getOrderDto = new GetOrderDto
                {
                    Id = order.Id,
                    Status = order.Status.ToString(),
                    DateTime = order.DateTime,
                    PriceBeforeDiscount = order.PriceBeforeDiscount,
                    Discount = order.Discount,
                    PriceBeforeTax = order.PriceBeforeTax,
                    TaxPercentage = order.TaxPercentage,
                    NetPrice = order.TotalPrice,
                    PaidAmount = order.PaidAmount,
                    RemainedAmount = order.RemainedAmount,
                    NumberOfPieces = order.OrderProducts.Sum(op => op.Quantity),
                    Products = order.OrderProducts.Select(op => new ProductDto
                    {
                        Id = op.ProductId,
                        Name = op.Product.Name,
                        Price = op.Product.Price,
                        Quantity = op.Quantity,
                    }).ToList()
                };
                return new ServiceResponse<GetOrderDto>() { Data = getOrderDto, Success = true };

            }
            catch
            {
                return new ServiceResponse<GetOrderDto>() { Success = false, Message = "An error occurred while processing your request." };
            }
        }

        public async Task<ServiceResponse<PaginationResponse<GetOrdersDto>>> GetOrdersAsync(GetAllOrdersFilter ordersFilter)
        {
            try
            {
                IQueryable<Order> query = _orderRepo.GetAllQueryableAsync();

                if (ordersFilter.Status != null)
                    query = query.Where(o => o.Status == ordersFilter.Status);

                if (ordersFilter.From != null && ordersFilter.To != null)
                    query = query.Where(o => o.DateTime.Date >= ordersFilter.From.Value.Date && o.DateTime.Date <= ordersFilter.To.Value.Date);

                else if (ordersFilter.From != null)
                    query = query.Where(o => o.DateTime.Date >= ordersFilter.From.Value.Date);

                else if (ordersFilter.To != null)
                    query = query.Where(o => o.DateTime.Date <= ordersFilter.To.Value.Date);




                var count = await query.CountAsync();

                var orders = await query.Skip(ordersFilter.PageNumber - 1)
                                        .Take(ordersFilter.PageSize)
                                        .Select(o => new GetOrdersDto
                                        {
                                            Id = o.Id,
                                            DateTime = o.DateTime,
                                            Status = o.Status.ToString(),
                                            PriceBeforeDiscount = o.PriceBeforeDiscount,
                                            Discount = o.Discount,
                                            PriceBeforeTax = o.PriceBeforeTax,
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

        public async Task<ServiceResponse<bool>> Pay(Guid id, PayDto pay)
        {
            try
            {
                if (id == Guid.Empty)
                    return new ServiceResponse<bool>() { Success = false, Message = "Please Enter the id" };

                Order order = await _orderRepo.FindByIdAsync(id);

                if (pay.PaymentAmount > order.RemainedAmount)
                    return new ServiceResponse<bool>() { Success = false, Message = $"The Payment should not be more than: {order.RemainedAmount} " };

                if (pay.PaymentAmount < order.RemainedAmount)
                {
                    order.RemainedAmount -= pay.PaymentAmount;
                    order.PaidAmount += pay.PaymentAmount;
                    await _unitOfWork.CommitAsync();
                    return new ServiceResponse<bool> { Success = true, Message = $"The remaining amount now is {order.RemainedAmount}" };
                }

                order.RemainedAmount = 0;
                order.PaidAmount = order.TotalPrice;
                order.Status = OrderStatus.Finished;

                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool> { Success = true, Message = "All Money have been paid and Order process is finished" };

            }
            catch
            {
                return new ServiceResponse<bool> { Success = false, Message = "An error occurred " };
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
