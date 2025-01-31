﻿using AutoMapper;
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
        private readonly IBaseRepository<Client> _clientRepo = unitOfWork.GetRepository<Client>();



        public async Task<ServiceResponse<bool>> AddOrder(AddOrderDto addOrderDto)
        {
            try
            {
                if (addOrderDto.OrderProducts.Count == 0)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Please insert Products to your order",
                        Data = false
                    };
                }

                var existingClient = await _clientRepo.FindByIdAsync(addOrderDto.ClientId);
                if (existingClient == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "The Client does not exist",
                        Data = false
                    };
                }

                decimal totalOrderPrice = 0;
                decimal priceBeforeDiscount = 0;
                decimal totalProductLevelDiscount = 0;

                foreach (var orderProduct in addOrderDto.OrderProducts)
                {
                    var productDb = await _productRepo.FindByIdAsync(orderProduct.ProductId);

                    if (productDb == null)
                    {
                        return new ServiceResponse<bool>
                        {
                            Success = false,
                            Message = $"Product not found for ID: {orderProduct.ProductId}",
                            Data = false
                        };
                    }

                    if (productDb.StockQuantity < orderProduct.Quantity)
                    {
                        return new ServiceResponse<bool>
                        {
                            Success = false,
                            Message = $"Insufficient stock for product ID: {orderProduct.ProductId}",
                            Data = false
                        };
                    }

                    decimal productPriceBeforeDiscount = productDb.Price ?? 0;
                    decimal totalPriceForProductBeforeDiscount = productPriceBeforeDiscount * orderProduct.Quantity;
                    priceBeforeDiscount += totalPriceForProductBeforeDiscount;

                    decimal productPriceAfterDiscount = productPriceBeforeDiscount;
                    decimal discountAmountForProduct = 0;
                    if (orderProduct.ProductDiscount != 0)
                    {
                        discountAmountForProduct = productPriceBeforeDiscount * orderProduct.ProductDiscount;
                        productPriceAfterDiscount -= discountAmountForProduct;
                        totalProductLevelDiscount += discountAmountForProduct * orderProduct.Quantity;
                    }

                    decimal totalPriceForProductAfterDiscount = productPriceAfterDiscount * orderProduct.Quantity;
                    totalOrderPrice += totalPriceForProductAfterDiscount;

                    productDb.StockQuantity -= orderProduct.Quantity;
                    _productRepo.Update(productDb);
                }

                decimal orderLevelDiscount = 0;
                decimal discountAmount = totalProductLevelDiscount;
                switch (addOrderDto.DiscountType)
                {
                    case DiscountEnum.Percentage:
                        orderLevelDiscount = totalOrderPrice * addOrderDto.Discount;
                        totalOrderPrice -= orderLevelDiscount;
                        discountAmount += orderLevelDiscount;
                        break;
                    case DiscountEnum.Fixed:
                        orderLevelDiscount = addOrderDto.Discount;
                        totalOrderPrice -= orderLevelDiscount;
                        discountAmount += orderLevelDiscount;
                        break;
                }

                decimal priceBeforeTax = totalOrderPrice;
                totalOrderPrice += addOrderDto.TaxPercentage * totalOrderPrice;

                decimal remained = totalOrderPrice - addOrderDto.PaidAmount;
                if (remained < 0)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Recheck the Paid Amount because it's more than the total order price",
                        Data = false
                    };
                }

                var orderStatus = OrderStatus.Processing;
                if (remained == 0)
                    orderStatus = OrderStatus.Finished;

                Order order = _mapper.Map<Order>(addOrderDto);
                order.TotalPrice = totalOrderPrice;
                order.RemainedAmount = remained;
                order.Status = orderStatus;
                order.DateTime = DateTime.Now;
                order.Discount = discountAmount;

                await _orderRepo.AddAsync(order);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Order Created Successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                // Optionally, log the exception
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"An error occurred while adding the order: {ex.Message}",
                    Data = false
                };
            }
        }


        public async Task<ServiceResponse<GetOrderDto>> GetOrder(Guid id)
        {
            try
            {
                Expression<Func<Order, bool>> filterPredicate = p => p.Id == id;

                Order order = await _orderRepo.FindAsync(filterPredicate, Include: q => q.Include(o => o.Client)
                                                                                    .Include(o => o.OrderProducts)
                                                                                    .ThenInclude(p => p.Product), asNoTracking: true);

                if (order == null)
                    return new ServiceResponse<GetOrderDto>() { Success = false, Message = "Order Not Found" };
                
                var getOrderDto = _mapper.Map<GetOrderDto>(order);

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

                query = query.Include(p => p.OrderProducts);


                var orders = await query.Skip(ordersFilter.PageNumber - 1)
                                        .Take(ordersFilter.PageSize)
                                        .Select(o => _mapper.Map<GetOrdersDto>(o))
                                        .ToListAsync();

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
