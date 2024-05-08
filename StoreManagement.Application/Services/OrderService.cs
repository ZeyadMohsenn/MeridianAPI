using AutoMapper;
using Microsoft.AspNetCore.Identity;
using StoreManagement.Application.Interfaces;
using StoreManagement.Bases;
using StoreManagement.Domain.Dtos;
using StoreManagement.Domain.Dtos.Order;
using StoreManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Application.Services
{
    public class OrderService(IUnitOfWork unitOfWork, IMapper mapper) : ServiceBase(), IOrderService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IBaseRepository<Order> _orderRepo = unitOfWork.GetRepository<Order>();
        private readonly IBaseRepository<Product> _productRepo = unitOfWork.GetRepository<Product>();
        //private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBaseRepository<OrderProduct> _orderProductRepo = unitOfWork.GetRepository<OrderProduct>();


        public async Task<ServiceResponse<bool>> AddOrder(AddOrderDto addOrderDto)
        {
            try
            {
                decimal totalOrderPrice = 0;

                foreach (var orderProduct in addOrderDto.OrderProducts)
                {
                    var productDb = await _productRepo.FindByIdAsync(orderProduct.ProductId);

                    if (productDb == null)
                        return new ServiceResponse<bool>() { Success = false, Message = $"Product not found for ID: {orderProduct.ProductId}" };
                    
                    if(productDb.StockQuantity < orderProduct.Quantity)
                        return new ServiceResponse<bool>() { Success = false, Message = $"Insufficient stock for product ID: {orderProduct.ProductId}" };
   
                    decimal totalPriceForProduct = (productDb.Price ?? 0) * orderProduct.Quantity;
                    totalOrderPrice += totalPriceForProduct;                    
                    productDb.StockQuantity -= orderProduct.Quantity;
                    _productRepo.Update(productDb);
                }
                totalOrderPrice = totalOrderPrice - (addOrderDto.TaxPercentage + addOrderDto.Discount) * totalOrderPrice;
                //decimal discountAmount = totalOrderPrice * addOrderDto.Discount;
                //decimal totalOrderPriceAfterDiscount = totalOrderPrice - discountAmount;
                //decimal totalOrderPriceAfterTax = totalOrderPriceAfterDiscount + (totalOrderPriceAfterDiscount * addOrderDto.TaxPercentage);

                var order = new Order
                {
                    //UserId = addOrderDto.UserId,   
                    Status = OrderStatus.Processing,
                    OrderProducts = addOrderDto.OrderProducts.Select(op => new OrderProduct
                    {
                        ProductId = op.ProductId,
                        Quantity = op.Quantity
                    }).ToList(),
                    TotalPrice = totalOrderPrice
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
            catch (Exception ) {
                return new ServiceResponse<bool>()
                {
                    Success = false,
                    Message = $"An error occurred while adding the order",
                    Data = false
                };
            }
        }
    }
}
