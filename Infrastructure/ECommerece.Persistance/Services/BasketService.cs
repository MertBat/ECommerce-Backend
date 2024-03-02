using ECommerce.Application.Abstraction.Services;
using ECommerce.Application.Repositories;
using ECommerce.Application.Repositories.Basket;
using ECommerce.Application.Repositories.BasketItem;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Identity;
using ECommerce.Persistance.Repositories.BasketItem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Persistance.Services
{
    public class BasketService : IBasketService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IOrderReadRepository _orderReadRepository;
        private readonly IBasketWriteRepository _basketWriteRepository;
        private readonly IBasketReadRepository _basketReadRepository;
        private readonly IBasketItemReadRespository _basketItemReadRespository;
        private readonly IBasketItemWriteRepository _basketItemWriteRepository;

        public BasketService(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IOrderReadRepository orderReadRepository, IBasketWriteRepository basketWriteRepository, IBasketReadRepository basketReadRepository, IBasketItemReadRespository basketItemReadRespository, IBasketItemWriteRepository basketItemWriteRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _orderReadRepository = orderReadRepository;
            _basketWriteRepository = basketWriteRepository;
            _basketReadRepository = basketReadRepository;
            _basketItemReadRespository = basketItemReadRespository;
            _basketItemWriteRepository = basketItemWriteRepository;
        }

        public async Task AddItemToBasket(string productId, int quantity)
        {
            Basket? basket = await ContextUser();
            if (basket != null)
            {
                BasketItem basketItem = await _basketItemReadRespository.GetSingleAsync(bi => bi.BasketId == basket.Id && bi.ProductId.ToString() == productId);
                if (basketItem != null)
                {
                    basketItem.Quantity++;
                }
                else
                {
                    await _basketItemWriteRepository.AddAsync(new()
                    {
                        BasketId = basket.Id,
                        ProductId = Guid.Parse(productId),
                        Quantity = quantity
                    });
                }
                    await _basketItemWriteRepository.saveAsync();                  
            }
        }

        public async Task<List<BasketItem>> GetBasketItemsAsync()
        {
            Basket? basket = await ContextUser();
            Basket? result = await _basketReadRepository.Table.Include(b => b.BasketItems).ThenInclude(bi => bi.Product).FirstOrDefaultAsync(b => b.Id == basket.Id);

            return result.BasketItems.ToList();
        }

        public async Task RemoveBasketItemAsync(string basketItemId)
        {
            BasketItem basketItem = await _basketItemReadRespository.GetById(basketItemId);

            if(basketItem != null)
            {
                _basketItemWriteRepository.Remove(basketItem);
                await _basketItemWriteRepository.saveAsync();
            }
        }

        public async Task UpdateQuantity(string basketItemId, int quantity)
        {
            BasketItem? basketItem = await _basketItemReadRespository.GetById(basketItemId);
            if(basketItem != null)
            {
                basketItem.Quantity = quantity;
                await _basketItemWriteRepository.saveAsync();
            }
        }

        private async Task<Basket?> ContextUser()
        {
            var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(userName))
            {
                AppUser? user = await _userManager.Users.Include(u => u.Baskets).FirstOrDefaultAsync(u => u.UserName == userName);

                var _basket = from basket in user.Baskets
                              join order in _orderReadRepository.Table
                              on basket.Id equals order.Id into BasketOrders
                              from o in BasketOrders.DefaultIfEmpty()
                              select new
                              {
                                  Basket = basket,
                                  Order = o
                              };

                Basket? targetBasket = null;
                if (_basket.Any(b => b.Order is null))
                {
                    targetBasket = _basket.FirstOrDefault(b => b.Order is null)?.Basket;
                }
                else
                {
                    targetBasket = new();
                    user.Baskets.Add(targetBasket);
                }

                await _basketWriteRepository.saveAsync();
                return targetBasket;
            }

            throw new Exception("Unexpected error");
        }
    }
}
