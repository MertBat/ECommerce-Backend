using ECommerce.SignalR.Hubs;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.SignalR
{
    public static class HubRegistration
    {
        public static void Hubs(this WebApplication webApplication)
        {
            webApplication.MapHub<ProductHub>("/products-hub");
            webApplication.MapHub<OrderHub>("/orders-hub");
        }
    }
}
