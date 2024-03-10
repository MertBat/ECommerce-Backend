using ECommerce.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstraction.Services
{
    public interface IOrderService
    {
        Task CreateOrderAsync(string basketId, string address, string description);
        Task<List<ListOrderDTO>> GetAllOrdersAsync(int page, int size);
        Task<int> GetAllOrdersCountAsync();
    }
}
