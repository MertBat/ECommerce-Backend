using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstraction.Hubs
{
    public interface IOrderHubServices
    {
        Task OrderAddedMessageAsync(string message);
    }
}
