using ECommerce.Application.Abstraction.Services;
using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Repositories;
using ECommerce.Application.Repositories.CompletedOrder;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Persistance.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderWriteRepository _orderWriteRepository;
        private readonly IOrderReadRepository _orderReadRepository;
        private readonly ICompletedOrderWriteRepository _comletedWriteRepository;
        private readonly ICompletedOrderReadRepository _completedOrderReadRepository;

        public OrderService(IOrderWriteRepository orderWriteRepository, IOrderReadRepository orderReadRepository, ICompletedOrderWriteRepository comletedWriteRepository, ICompletedOrderReadRepository completedOrderReadRepository)
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadRepository = orderReadRepository;
            _comletedWriteRepository = comletedWriteRepository;
            _completedOrderReadRepository = completedOrderReadRepository;
        }

        public async Task ComleteOrderAsync(string orderId, bool orderStatus)
        {
            Order order = await _orderReadRepository.GetById(orderId);
            if (order != null)
            {
                await _comletedWriteRepository.AddAsync(new()
                {
                    OrderId = Guid.Parse(orderId),
                    OrderStatus = orderStatus
                }
                );
            }
            await _comletedWriteRepository.saveAsync();
        }

        public async Task CreateOrderAsync(string? basketId, string address, string description)
        {
            await _orderWriteRepository.AddAsync(new()
            {
                Address = address,
                Description = description,
                Id = Guid.Parse(basketId),
                OrderCode = (Math.Round(new Random().NextDouble() * 100000000)).ToString()
            });

            await _orderWriteRepository.saveAsync();
        }

        public async Task<List<ListOrderDTO>> GetAllOrdersAsync(int page, int size)
        {
            var query = _orderReadRepository.Table.Include(o => o.Basket).ThenInclude(b => b.User)
                  .Include(o => o.Basket).ThenInclude(b => b.BasketItems).ThenInclude(bi => bi.Product).Skip(page * size).Take(size);


            var data = from order in query
                       join completedOrder in _completedOrderReadRepository.Table
                       on order.Id equals completedOrder.OrderId into co
                       from _co in co.DefaultIfEmpty()
                       select new
                       {
                           Id = order.Id,
                           CreatedDate = order.CreatedDate,
                           OrderCode = order.OrderCode,
                           Basket = order.Basket,
                           Description = order.Description,
                           Address = order.Address,
                           Completed = _co !=null,
                           OrderStatus = _co != null && _co.OrderStatus
                       };

            return await data.Select(o => new ListOrderDTO
            {
                Id = o.Id.ToString(),
                OrderCode = o.OrderCode,
                UserName = o.OrderCode,
                TotalPrice = o.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity),
                Description = o.Description,
                Address = o.Address,
                CreatedDate = o.CreatedDate,
                Completed = o.Completed,
                OrderStatus = o.OrderStatus
            }).ToListAsync();
        }

        public async Task<int> GetAllOrdersCountAsync()
        {
            return await _orderReadRepository.GetAll().CountAsync();
        }

        public async Task<SingleOrderDTO> GetOrderByIdAsync(string orderId)
        {
            var data = await _orderReadRepository.Table.Include(o => o.Basket).ThenInclude(b => b.BasketItems).ThenInclude(bi => bi.Product).Include(o=> o.CompletedOrder).FirstOrDefaultAsync(o => o.Id == Guid.Parse(orderId));

            return new()
            {
                Id = data.Id.ToString(),
                Address = data.Address,
                CreatedDate = data.CreatedDate,
                OrderCode = data.OrderCode,
                Description = data.Description,
                OrderStatus = data.CompletedOrder != null ? data.CompletedOrder.OrderStatus: false ,
                Completed = data.CompletedOrder != null ? true : false,
                BasketItems = data.Basket.BasketItems.Select(bi => new
                {
                    bi.Product.Name,
                    bi.Product.Price,
                    bi.Quantity
                })
            };
        }
    }
}
