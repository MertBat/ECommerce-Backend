using ECommerce.Application.Abstraction.Services;
using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Persistance.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderWriteRepository _orderWriteRepository;
        private readonly IOrderReadRepository _orderReadRepository;

        public OrderService(IOrderWriteRepository orderWriteRepository, IOrderReadRepository orderReadRepository)
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadRepository = orderReadRepository;
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
          return await _orderReadRepository.Table.Include(o => o.Basket).ThenInclude(b => b.User)
                .Include(o=> o.Basket).ThenInclude(b=> b.BasketItems).ThenInclude(bi=> bi.Product)
                .Select(o=> new ListOrderDTO
                {
                    Id = o.Id.ToString(),
                    Address = o.Address,
                    Description = o.Description,
                    CreatedDate = o.CreatedDate,
                    OrderCode = o.OrderCode,
                    TotalPrice = o.Basket.BasketItems.Sum(bi=>bi.Product.Price * bi.Quantity),
                    UserName = o.Basket.User.UserName

                }).Skip(page * size).Take(size).ToListAsync();
        }

        public async Task<int> GetAllOrdersCountAsync()
        {
            return await _orderReadRepository.GetAll().CountAsync();
        }

        public async Task<SingleOrderDTO> GetOrderByIdAsync(string orderId)
        {
            var data = await _orderReadRepository.Table.Include(o=> o.Basket).ThenInclude(b=> b.BasketItems).ThenInclude(bi=> bi.Product).FirstOrDefaultAsync(o=> o.Id == Guid.Parse(orderId));

            return new()
            {
                Id = data.Id.ToString(),
                Address = data.Address,
                CreatedDate = data.CreatedDate,
                OrderCode = data.OrderCode,
                Description = data.Description,
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
