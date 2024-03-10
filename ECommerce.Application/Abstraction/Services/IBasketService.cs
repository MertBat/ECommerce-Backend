using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstraction.Services
{
    public interface IBasketService
    {
        public Task<List<BasketItem>> GetBasketItemsAsync();
        public Task AddItemToBasket(string productId, int quantity);
        public Task UpdateQuantity(string basketItemId, int quantity);
        public Task RemoveBasketItemAsync( string basketItemId);
        public Basket? GetUserActiveBasket { get; }
    }
}
